/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using EnsureThat;
using FhirTool.Conversion.Converters;
using FhirTool.Core;
using FhirTool.Core.FhirWrappers;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using NuGet.Versioning;

namespace FhirTool.Conversion
{
    public class FhirConverter
    {
        private static readonly FhirVersion[] SUPPORTED_VERSIONS = new[] { FhirVersion.R3, FhirVersion.R4 };
        // Only one version hop forward or backward is allowed on every run.
        private const int MAX_VERSION_HOPS = 1;
        private FhirVersion FromVersion { get; set; }
        private FhirVersion ToVersion { get; set; }
        private BaseConverter Converter { get; }
        private FhirPath Path { get; set; }

        public FhirConverter(FhirVersion to, FhirVersion from)
        {
            ToVersion = to;
            FromVersion = from;

            if (!IsVersionSupported(FromVersion)) throw new FhirVersionNotSupportedException(FromVersion);
            if (!IsVersionSupported(ToVersion)) throw new FhirVersionNotSupportedException(ToVersion);
            if (!CanConvertBetweenVersions(FromVersion, ToVersion))
            {
                throw new FhirConversionException($"Cannot convert between FHIR version '{FromVersion}' and '{ToVersion}'. Maximum allowed version hops in one iteration is '{MAX_VERSION_HOPS}'.");
            }


            if (ToVersion == FhirVersion.R3 && FromVersion == FhirVersion.R4)
            {
                Converter = new FhirR4ToR3ConversionRoutines();
            }
            else if (ToVersion == FhirVersion.R4 && FromVersion == FhirVersion.R3)
            {
                Converter = new FhirR3ToR4ConversionRoutines();
            }
        }

        internal static FhirVersion GetFhirVersion(Type fhirType)
        {
            var modelInfoType = fhirType.Assembly.GetTypes()
                .Where(t => t.Name.Equals("ModelInfo"))
                .FirstOrDefault();
            var version = modelInfoType
                .GetProperty("Version", BindingFlags.Static | BindingFlags.Public)
                .GetValue(null) as string;

            FhirVersion? fhirVersion = default;
            if (SemanticVersion.TryParse(version, out SemanticVersion semanticVersion))
            {
                fhirVersion = EnumUtility.ParseLiteral<FhirVersion>($"{semanticVersion.Major}.{semanticVersion.Minor}");
            }

            return fhirVersion ?? FhirVersion.None;
        }

        internal static string GetMajorVersion(string version)
        {
            if (version == null) return string.Empty;

            int index = version.IndexOf('.');
            return index == -1 ? string.Empty : version.Substring(0, index);
        }

        public static IEnumerable<FhirVersion> SupportedVersions { get { return SUPPORTED_VERSIONS; } }

        public static bool IsVersionSupported(FhirVersion version)
        {
            return SupportedVersions.Contains(version);
        }

        public static bool CanConvertBetweenVersions(FhirVersion from, FhirVersion to)
        {
            int versionHops = Math.Abs(to - from);
            return MAX_VERSION_HOPS == versionHops;
        }

        public string Convert(string fromString)
        {
            var parser = new SerializationWrapper(FromVersion);
            var serializer = new SerializationWrapper(ToVersion);

            var fromObject = parser.Parse(fromString);
            var converted = Convert<Base, Base>(fromObject.Resource);
                 
            return serializer.Serialize(converted);
        }

        public TTo Convert<TTo, TFrom>(TFrom fromObject)
            where TFrom : Base
            where TTo : Base
        {
            try
            {
                Path = new FhirPath();
                Path.Push(fromObject.GetType().Name);
                return ConvertElement<TTo, TFrom>(fromObject);
            }
            finally
            {
                Path = null;
            }
        }

        internal TTo ConvertElement<TTo, TFrom>(TFrom fromObject)
            where TFrom : Base
            where TTo : Base
        {
            return Convert(typeof(TTo), typeof(TFrom), fromObject) as TTo;
        }

        internal IEnumerable<TTo> ConvertList<TTo, TFrom>(IEnumerable<TFrom> fromList)
            where TFrom : Base
            where TTo : Base
        {
            EnsureArg.IsNotNull(fromList, nameof(fromList));

            return fromList.Select(it => ConvertElement<TTo, TFrom>(it));
        }

        private Base Convert(Type targetType, Type sourceType, Base sourceObject)
        {
            if (sourceObject == null) return default;

            if (targetType.IsAbstract)
            {
                var actualSourceType = sourceObject.GetType();
                var actualTargetType = Converter.GetTargetFhirType(actualSourceType);

                sourceType = actualSourceType;
                targetType = actualTargetType;
            }

            var targetObject = Activator.CreateInstance(targetType) as Base;

            var handledProperties = ConvertTypesWithChanges(targetObject, sourceObject);
            var targetProperties = targetObject.GetType().GetProperties()
                .Where(prop => Attribute.IsDefined(prop, Converter.GetTargetFhirElementAttributeType()));

            foreach (var targetProperty in targetProperties)
            {
                dynamic elementAttribute = targetProperty.GetCustomAttribute(Converter.GetTargetFhirElementAttributeType());
                Path.Push(elementAttribute?.Name ?? targetProperty.Name);

                if (handledProperties.Contains(targetProperty.Name)) { Path.Pop(); continue; }
                var sourceProperty = sourceType.GetProperty(targetProperty.Name);
                if (sourceProperty == null) { Path.Pop(); continue; }

                var value = ConvertProperty(targetProperty, sourceProperty, targetObject, sourceObject);
                if (value != null)
                {
                    targetProperty.SetValue(targetObject, value);
                }
                Path.Pop();
            }

            return targetObject;
        }

        private Stack<HashSet<string>> HandledProperties { get; } = new Stack<HashSet<string>>();

        private ISet<string> ConvertTypesWithChanges(Base targetObject, Base sourceObject)
        {
            HandledProperties.Push(new HashSet<string>());
            PropertyChangedEventHandler propertyListener = delegate (object o, PropertyChangedEventArgs e)
            {
                HandledProperties.Peek().Add(e.PropertyName);
            };

            targetObject.PropertyChanged += propertyListener;
            Converter.Convert(targetObject, sourceObject, this);
            targetObject.PropertyChanged -= propertyListener;

            return HandledProperties.Pop();
        }

        private object ConvertGenericProperty(PropertyInfo targetProperty, PropertyInfo sourceProperty, Base targetObject, Base sourceObject, object sourceValue)
        {
            var sourcePropertyType = sourceProperty.PropertyType;

            // NOTE: Currently only allowing one generic argument
            var sourceGenericDefinition = sourcePropertyType.GetGenericTypeDefinition();
            if (sourceGenericDefinition == typeof(List<>))
            {
                return ConvertGenericListProperty(targetProperty, sourceProperty, sourceValue);
            }
            else if (sourceGenericDefinition == Converter.GetSourceCodeType())
            {
                return ConvertGenericCodeProperty(targetProperty, sourceProperty, sourceValue);
            }
            else if (sourceGenericDefinition == typeof(Nullable<>))
            {
                return ConvertGenericNullableProperty(targetProperty, sourceProperty, sourceValue);
            }
            else
            {
                throw new UnknownFhirTypeException(sourcePropertyType, Path);
            }
        }

        private object ConvertGenericNullableProperty(PropertyInfo targetProperty, PropertyInfo sourceProperty, object sourceValue)
        {
            var sourcePropertyType = sourceProperty.PropertyType;
            if (Nullable.GetUnderlyingType(sourcePropertyType)?.IsEnum ?? false)
            {
                var converted = Converter.ConvertEnum(sourceValue as Enum, Nullable.GetUnderlyingType(targetProperty.PropertyType));
                return converted;
            }

            return sourceValue;
        }

        private object ConvertGenericCodeProperty(PropertyInfo targetProperty, PropertyInfo sourceProperty, object sourceValue)
        {
            var actualSourceType = sourceProperty.PropertyType;

            var enumType = targetProperty.PropertyType.GetGenericArguments().FirstOrDefault();
            var actualTargetType = Converter.GetTargetCodeType().MakeGenericType(enumType);
            var convertedValue = Convert(actualTargetType, actualSourceType, sourceValue as Base);
            return convertedValue;
        }

        private object ConvertGenericListProperty(PropertyInfo targetProperty, PropertyInfo sourceProperty, object sourceValue)
        {
            // Retrieve the IEnumerable and iterate over it
            var sourceValueList = sourceValue as IList;
            if (sourceValueList.Count == 0) return null;

            var targetPropertyOuterGenericArg = targetProperty.PropertyType.GetGenericArguments().First();
            var sourcePropertyOuterGenericArg = sourceProperty.PropertyType.GetGenericArguments().First();

            // Create a generic list type and then instantiate it
            var targetListType = (typeof(List<>)).MakeGenericType(targetPropertyOuterGenericArg);
            IList targetList = Activator.CreateInstance(targetListType) as IList;

            // If outer generic argument is generic type, then retrieve the inner generic argument.
            Type targetInnerGenericArg = null;
            if (targetPropertyOuterGenericArg.IsGenericType && targetPropertyOuterGenericArg.GetGenericTypeDefinition() == Converter.GetTargetCodeType())
            {
                // NOTE: Also only allowing one generic argument for the inner generic argument
                targetInnerGenericArg = targetPropertyOuterGenericArg.GetGenericArguments().First();
            }

            foreach (var value in sourceValueList)
            {
                // TODO: Consider branching the if-else differently, the else-part is the one that will be hit most of the time
                if (targetInnerGenericArg != null)
                {
                    // Types of Code<T>, where T is an Enum type
                    var actualTargetType = Converter.GetTargetCodeType().MakeGenericType(targetInnerGenericArg);
                    var convertedValue = Convert(actualTargetType, value.GetType(), value as Base);

                    targetList.Add(convertedValue);
                }
                else
                {
                    // Ordinary elements part of List<T>
                    if (sourcePropertyOuterGenericArg.IsAbstract)
                    {
                        var actualSourceType = value.GetType();
                        var actualTargetType = Converter.GetTargetFhirType(actualSourceType);
                        var converted = Convert(actualTargetType, actualSourceType, value as Base);
                        targetList.Add(converted);
                    }
                    else
                    {
                        var converted = Convert(targetPropertyOuterGenericArg, sourcePropertyOuterGenericArg, value as Base);
                        targetList.Add(converted);
                    }
                }
            }
            return targetList;
        }

        private object ConvertNonGenericProperty(PropertyInfo targetProperty, PropertyInfo sourceProperty, Base targetObject, Base sourceObject, object sourceValue)
        {
            var actualSourceType = sourceValue.GetType();
            if (Converter.IsTargetFhirType(actualSourceType))
            {
                var actualTargetType = Converter.GetTargetFhirType(actualSourceType);
                return Convert(actualTargetType, actualSourceType, sourceValue as Base);
            }

            return sourceValue;
        }

        private object ConvertProperty(PropertyInfo targetProperty, PropertyInfo sourceProperty, Base targetObject, Base sourceObject)
        {
            var sourceValue = sourceProperty.GetValue(sourceObject);
            if (sourceValue == null) return null;

            var sourcePropertyType = sourceProperty.PropertyType;
            if (sourcePropertyType.IsGenericType)
            {
                return ConvertGenericProperty(targetProperty, sourceProperty, targetObject, sourceObject, sourceValue);
            }
            else
            {
                return ConvertNonGenericProperty(targetProperty, sourceProperty, targetObject, sourceObject, sourceValue);
            }
        }
    }
}
