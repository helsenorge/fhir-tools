extern alias R3;
extern alias R4;

using System.Linq;
using System.Collections.Generic;
using R3Model = R3::Hl7.Fhir.Model;
using R4Model = R4::Hl7.Fhir.Model;

namespace FhirTool.Core.FhirWrappers
{
    public static class ResourceTypeWrapperExtensions
    {
        private static Dictionary<ResourceTypeWrapper, R3Model.ResourceType> R3Map =
            new Dictionary<ResourceTypeWrapper, R3Model.ResourceType>
            {
                { ResourceTypeWrapper.Binary, R3Model.ResourceType.Binary },
                { ResourceTypeWrapper.CodeSystem, R3Model.ResourceType.CodeSystem },
                { ResourceTypeWrapper.DocumentReference, R3Model.ResourceType.DocumentReference },
                { ResourceTypeWrapper.Endpoint, R3Model.ResourceType.Endpoint },
                { ResourceTypeWrapper.Questionnaire, R3Model.ResourceType.Questionnaire },
                { ResourceTypeWrapper.StructureDefinition, R3Model.ResourceType.StructureDefinition },
                { ResourceTypeWrapper.ValueSet, R3Model.ResourceType.ValueSet }
            };
        private static Dictionary<ResourceTypeWrapper, R4Model.ResourceType> R4Map =
            new Dictionary<ResourceTypeWrapper, R4Model.ResourceType>
            {
                { ResourceTypeWrapper.Binary, R4Model.ResourceType.Binary },
                { ResourceTypeWrapper.CodeSystem, R4Model.ResourceType.CodeSystem },
                { ResourceTypeWrapper.DocumentReference, R4Model.ResourceType.DocumentReference },
                { ResourceTypeWrapper.Endpoint, R4Model.ResourceType.Endpoint },
                { ResourceTypeWrapper.Questionnaire, R4Model.ResourceType.Questionnaire },
                { ResourceTypeWrapper.StructureDefinition, R4Model.ResourceType.StructureDefinition },
                { ResourceTypeWrapper.ValueSet, R4Model.ResourceType.ValueSet }
            };

        public static R3Model.ResourceType ToR3(this ResourceTypeWrapper me)
        {
            return R3Map[me];
        }

        public static R4Model.ResourceType ToR4(this ResourceTypeWrapper me)
        {
            return R4Map[me];
        }

        public static ResourceTypeWrapper Wrap(this R3Model.ResourceType me)
        {
            return R3Map.ToDictionary(keySelector => keySelector.Value, valueSelector => valueSelector.Key)[me];
        }

        public static ResourceTypeWrapper Wrap(this R4Model.ResourceType me)
        {
            return R4Map.ToDictionary(keySelector => keySelector.Value, valueSelector => valueSelector.Key)[me];
        }
    }

    public enum ResourceTypeWrapper
    {
        Binary = 8,
        CodeSystem = 18,
        DocumentReference = 37,
        Endpoint = 42,
        Questionnaire = 93,
        StructureDefinition = 108,
        ValueSet = 117
    }
}
