/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;

namespace FhirTool.Conversion
{
    internal abstract class BaseConverter
    {
        protected Dictionary<(Type, Type), Delegate> Map { get; set; } = new Dictionary<(Type, Type), Delegate>();
        protected Dictionary<Type, Type> ComponentTargetToSourceTypeMap { get; set; }
        protected Dictionary<Type, Type> ComponentSourceToTargetTypeMap { get; set; }

        public BaseConverter()
        {
            InitComponentTypeMap();
        }

        protected abstract void InitComponentTypeMap();

        public abstract Type GetTargetCodeType();
        public abstract Type GetSourceCodeType();
        public abstract Type GetTargetFhirElementAttributeType();
        public abstract Type GetSourceFhirElementAttributeType();

        protected abstract string GetFhirTypeNameForTargetType(Type targetType);

        protected abstract Type GetTargetTypeForFhirTypeName(string targetTypeName);

        protected abstract string GetFhirTypeNameForSourceType(Type sourceType);

        protected abstract Type GetSourceTypeForFhirTypeName(string sourceTypeName);

        private Type GetSourceFhirComponentType(Type targetType)
        {
            return ComponentTargetToSourceTypeMap.GetOrDefault(targetType);
        }

        private Type GetTargetFhirComponentType(Type sourceType)
        {
            return ComponentSourceToTargetTypeMap.GetOrDefault(sourceType);
        }

        private Type GetSourceStandardFhirType(Type targetType)
        {
            var name = GetFhirTypeNameForTargetType(targetType);
            return name != null ? GetSourceTypeForFhirTypeName(name) : null;
        }

        private Type GetTargetStandardFhirType(Type sourceType)
        {
            var name = GetFhirTypeNameForSourceType(sourceType);
            return name != null ? GetTargetTypeForFhirTypeName(name) : null;
        }

        public Type GetSourceFhirType(Type targetType)
        {
            return GetSourceStandardFhirType(targetType) ?? GetSourceFhirComponentType(targetType);
        }

        public Type GetTargetFhirType(Type sourceType)
        {
            return GetTargetStandardFhirType(sourceType) ?? GetTargetFhirComponentType(sourceType);
        }

        public bool IsTargetFhirType(Type type)
        {
            return GetTargetFhirType(type) != null;
        }

        public bool IsSourceFhirType(Type type)
        {
            return GetSourceFhirType(type) != null;
        }

        public void Convert(Base to, Base from, FhirConverter converter)
        {
            var toType = to.GetType();
            var fromType = from.GetType();
            var key = (toType, fromType);
            if (Map.TryGetValue(key, out var convert))
            {
                convert.DynamicInvoke(to, from, converter);
            }
        }

        public TTo? ConvertEnum<TTo>(Enum from) where TTo : struct
        {
            if (from == null) return default;

            return EnumUtility.ParseLiteral<TTo>(from.GetLiteral());
        }

        public Enum ConvertEnum(Enum from, Type toType)
        {
            if (from == null) return null;
            return EnumUtility.ParseLiteral(from.GetLiteral(), toType) as Enum;
        }
    }
}
