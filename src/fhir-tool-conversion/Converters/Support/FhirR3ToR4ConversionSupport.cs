extern alias R3;
extern alias R4;

using System;
using FhirTool.Conversion.Converters.Support.TypeMaps;

using SourceModel = R3::Hl7.Fhir.Model;
using TargetModel = R4::Hl7.Fhir.Model;

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
