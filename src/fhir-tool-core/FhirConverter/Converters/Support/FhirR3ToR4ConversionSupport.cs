/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

extern alias R3;
extern alias R4;

using System;
using FhirTool.Conversion.Converters.Support.TypeMaps;

using TargetModel = R4::Hl7.Fhir.Model;
using SourceModel = R3::Hl7.Fhir.Model;

using TargetIntrospection = R4::Hl7.Fhir.Introspection;
using SourceIntrospection = R3::Hl7.Fhir.Introspection;

namespace FhirTool.Conversion.Converters
{
    internal partial class FhirR3ToR4ConversionRoutines : BaseConverter
    {
        protected override void InitComponentTypeMap()
        {
            ComponentSourceToTargetTypeMap = R4ToR3TypeMappings.Map.Reverse();
            ComponentTargetToSourceTypeMap = R4ToR3TypeMappings.Map;
        }

        public override Type GetTargetCodeType()
        {
            return typeof(TargetModel.Code<>);
        }

        public override Type GetSourceCodeType()
        {
            return typeof(SourceModel.Code<>);
        }

        public override Type GetTargetFhirElementAttributeType()
        {
            return typeof(TargetIntrospection.FhirElementAttribute);
        }

        public override Type GetSourceFhirElementAttributeType()
        {
            return typeof(SourceIntrospection.FhirElementAttribute);
        }

        protected override string GetFhirTypeNameForTargetType(Type targetType)
        {
            return TargetModel.ModelInfo.GetFhirTypeNameForType(targetType);
        }

        protected override Type GetTargetTypeForFhirTypeName(string targetTypeName)
        {
            return TargetModel.ModelInfo.GetTypeForFhirType(targetTypeName);
        }

        protected override string GetFhirTypeNameForSourceType(Type sourceType)
        {
            return SourceModel.ModelInfo.GetFhirTypeNameForType(sourceType);
        }

        protected override Type GetSourceTypeForFhirTypeName(string sourceTypeName)
        {
            return SourceModel.ModelInfo.GetTypeForFhirType(sourceTypeName);
        }
    }
}
