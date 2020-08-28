extern alias R3;
extern alias R4;

using R3Model = R3::Hl7.Fhir.Model;
using R4Model = R4::Hl7.Fhir.Model;
using R3Serialization = R3::Hl7.Fhir.Serialization;
using R4Serialization = R4::Hl7.Fhir.Serialization;

namespace FhirTool.Core.FhirWrappers
{
    internal class ResourceWrapper
    {
        public FhirVersion FhirVersion { get; }
        private R3Model.Resource R3Resource;
        private R4Model.Resource R4Resource;

        public ResourceWrapper(R3Model.Resource resource)
        {
            FhirVersion = FhirVersion.R3;
            R3Resource = resource;
        }

        public ResourceWrapper(R4Model.Resource resource)
        {
            FhirVersion = FhirVersion.R4;
            R4Resource = resource;
        }

        public string Id
        {
            get
            {
                switch (FhirVersion)
                {
                    case FhirVersion.R3:
                        return R3Resource.Id;
                    case FhirVersion.R4:
                        return R4Resource.Id;
                    default:
                        return default;
                }
            }
        }

        public string Serialize()
        {
            switch(FhirVersion)
            {
                case FhirVersion.R3:
                    return new R3Serialization.FhirJsonSerializer(new R3Serialization.SerializerSettings { Pretty = true }).SerializeToString(R3Resource);
                case FhirVersion.R4:
                    return new R4Serialization.FhirJsonSerializer(new R4Serialization.SerializerSettings { Pretty = true }).SerializeToString(R4Resource);
                default:
                    return default;
            }
        }
    }
}
