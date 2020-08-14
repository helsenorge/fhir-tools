extern alias R3;
extern alias R4;

using System;
using FhirTool.Conversion.Converters.Support.TypeMaps;

using TargetModel = R3::Hl7.Fhir.Model;
using SourceModel = R4::Hl7.Fhir.Model;

using TargetIntrospection = R3::Hl7.Fhir.Introspection;
using SourceIntrospection = R4::Hl7.Fhir.Introspection;

namespace FhirTool.Conversion.Converters
{
    internal partial class FhirR4ToR3ConversionRoutines : BaseConverter
    {
        protected override void InitComponentTypeMap()
        {
            ComponentSourceToTargetTypeMap = R4ToR3TypeMappings.Map;
            ComponentTargetToSourceTypeMap = R4ToR3TypeMappings.Map.Reverse();
        }

        public override Type GetTargetCodeType()
        {
            return typeof(TargetModel.Code<>);
        }

        public override Type GetSourceCodeType()
        {
            return typeof(SourceModel.Code<>);
        }

        public override Type GetTargetFhirElementAttribute()
        {
            return typeof(TargetIntrospection.FhirElementAttribute);
        }

        public override Type GetSourceFhirElementAttribute()
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
