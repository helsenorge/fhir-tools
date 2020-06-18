extern alias R3;
extern alias R4;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnsureThat;
using FhirTool.Core;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using NuGet.Versioning;
using R3Introspection = R3::Hl7.Fhir.Introspection;
using R3Model = R3::Hl7.Fhir.Model;
using R4Introspection = R4::Hl7.Fhir.Introspection;
using R4Model = R4::Hl7.Fhir.Model;

namespace FhirTool.Conversion
{
    public class FhirConverter
    {
        private static readonly FhirVersion[] SUPPORTED_VERSIONS = new[] { FhirVersion.R3, FhirVersion.R4 };
        // Only one version hop forward or backward is allowed on every run.
        private const int MAX_VERSION_HOPS = 1;

        internal FhirVersion GetFhirVersion(Type fhirType)
        {
            var modelInfoType = fhirType.Assembly.GetTypes()
                .Where(t => t.Name.Equals("ModelInfo"))
                .FirstOrDefault();
            var version = modelInfoType
                .GetProperty("Version", BindingFlags.Static | BindingFlags.Public)
                .GetValue(null) as string;
            
            FhirVersion? fhirVersion = default;
            if(SemanticVersion.TryParse(version, out SemanticVersion semanticVersion))
            {
                fhirVersion = EnumUtility.ParseLiteral<FhirVersion>($"{semanticVersion.Major}.{semanticVersion.Minor}");
            }

            return fhirVersion ?? FhirVersion.None;
        }

        internal string GetMajorVersion(string version)
        {
            if (version == null) return string.Empty;

            int index = version.IndexOf('.');
            return index == -1 ? string.Empty : version.Substring(0, index);
        }

        public IEnumerable<FhirVersion> SupportedVersions { get { return SUPPORTED_VERSIONS; } }

        public bool IsVersionSupported(FhirVersion version)
        {
            return SupportedVersions.Contains(version);
        }

        public bool CanConvertBetweenVersions(FhirVersion from, FhirVersion to)
        {
            int versionHops = Math.Abs(to - from);
            return MAX_VERSION_HOPS == versionHops;
        }

        public TTo ConvertResource<TTo, TFrom>(TFrom from)
            where TTo : Base, new()
            where TFrom : Base
        {
            EnsureArg.IsNotNull(from, nameof(from));

            FhirConversionInfo conversionInfo = new FhirConversionInfo
            {
                FromVersion = GetFhirVersion(typeof(TFrom)),
                ToVersion = GetFhirVersion(typeof(TTo)),
            };
            if(!IsVersionSupported(conversionInfo.FromVersion)) throw new FhirVersionNotSupportedException(conversionInfo.FromVersion);
            if (!IsVersionSupported(conversionInfo.ToVersion)) throw new FhirVersionNotSupportedException(conversionInfo.ToVersion);
            if (!CanConvertBetweenVersions(conversionInfo.FromVersion, conversionInfo.ToVersion))
            {
                throw new FhirConversionException($"Cannot convert between FHIR version '{conversionInfo.FromVersion}' and '{conversionInfo.ToVersion}'. Maximum allowed version hops in one iteration is '{MAX_VERSION_HOPS}'.");
            }

            string typeNameFrom = typeof(TFrom).Name;
            var targetType = typeof(TTo);
            string typeNameTo = targetType.Name;
            if (typeNameFrom != typeNameTo) throw new ArgumentException($"Cannot convert from type '{typeNameFrom}' to  '{typeNameTo}'.");

            return ConvertResource(from, targetType, conversionInfo) as TTo;
        }

        internal dynamic ConvertResource(Base from, Type targetType, FhirConversionInfo conversionInfo)
        {
            dynamic conversionRoutines;
            var to = Activator.CreateInstance(targetType);
            Type codeType;
            Type resourceTargetType;
            Type fhirElementAttributeType;
            Func<string, Type> getTypeForFhirType;
            // FromVersion == R3 and ToVersion == R4
            if (conversionInfo.FromVersion == FhirVersion.R3)
            {
                conversionRoutines = new FhirR3ToR4ConversionRoutines();
                codeType = typeof(R4Model.Code<>);
                resourceTargetType = typeof(R4Model.Resource);
                fhirElementAttributeType = typeof(R3Introspection.FhirElementAttribute);
                getTypeForFhirType = R4Model.ModelInfo.GetTypeForFhirType;
            }
            // FromVersion == R4 and ToVersion == R3
            else
            {
                conversionRoutines = new FhirR4ToR3ConversionRoutines();
                codeType = typeof(R3Model.Code<>);
                resourceTargetType = typeof(R3Model.Resource);
                fhirElementAttributeType = typeof(R4Introspection.FhirElementAttribute);
                getTypeForFhirType = R3Model.ModelInfo.GetTypeForFhirType;
            }

            var properties = from.GetType()
            .GetProperties()
            .Where(prop => Attribute.IsDefined(prop, fhirElementAttributeType));
            foreach (var property in properties)
            {
                var sourcePropType = property.PropertyType;
                if (sourcePropType.IsGenericType)
                {
                    if (sourcePropType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        var targetProp = targetType.GetProperty(property.Name);
                        if (targetProp == null) continue;

                        var targetPropType = targetProp.PropertyType;
                        // NOTE: Currently only allowing one generic argument
                        var targetOuterGenericArg = targetPropType.GetGenericArguments().First();
                        // If outer generic argument is generic type, then retrieve the inner generic argument.
                        Type targetInnerGenericArg = null;

                        if (targetOuterGenericArg.IsGenericType && targetOuterGenericArg.GetGenericTypeDefinition() == codeType)
                        {
                            // NOTE: Also only allowing one generic argument for the inner generic argument
                            targetInnerGenericArg = targetOuterGenericArg.GetGenericArguments().First();
                        }

                        // Retrieve the IEnumerable and iterate over it
                        IList values = property.GetValue(from) as IList;
                        if (values.Count == 0) continue;
                        // Create a generic list type and then instantiate it
                        var targetListType = (typeof(List<>)).MakeGenericType(targetOuterGenericArg);
                        IList targetList = Activator.CreateInstance(targetListType) as IList;
                        foreach (var value in values)
                        {
                            // TODO: Consider branching the if-else differently, the else-part is the one that will be hit most of the time

                            // Hitting a contained resource in a List<T>
                            if(targetOuterGenericArg == resourceTargetType)
                            {
                                var targetValue = ConvertResource(value as Base, getTypeForFhirType(value.GetType().Name), conversionInfo);
                                targetList.Add(targetValue);
                            }
                            // Types of Code<T>, where T is an Enum type
                            else if (targetInnerGenericArg != null)
                            {
                                var targetValue = conversionRoutines.ConvertPrimitive(value as Base, targetInnerGenericArg);
                                targetList.Add(targetValue);
                            }
                            // Ordinary elements part of List<T>
                            else
                            {
                                // Convert element in list R3 -> R4 and add it to the target list.
                                var targetValue = conversionRoutines.ConvertElement(value as Base);
                                targetList.Add(targetValue);
                            }
                        }
                        if (targetList.Count == 0) continue;
                        if (targetProp == null) continue;
                        targetProp.SetValue(to, targetList);
                    }
                    else if (sourcePropType.GetGenericTypeDefinition() == codeType)
                    {
                        var targetProp = targetType.GetProperty(property.Name);
                        if (targetProp == null) continue;

                        var value = property.GetValue(from);
                        var targetValue = conversionRoutines.ConvertPrimitive(value as Base, targetProp.PropertyType.GetGenericArguments().FirstOrDefault());
                        if (targetValue == null) continue;
                        targetProp.SetValue(to, targetValue);
                    }
                }
                else
                {
                    var value = property.GetValue(from) as Base;
                    var targetValue = conversionRoutines.ConvertElement(value);
                    if (targetValue == null) continue;
                    var targetProp = targetType.GetProperty(property.Name);
                    if (targetProp == null) continue;
                    targetProp.SetValue(to, targetValue);
                }
            }

            return to;
        }
    }
}
