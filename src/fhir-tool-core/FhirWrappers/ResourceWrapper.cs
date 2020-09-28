extern alias R3;
extern alias R4;

using R3Model = R3::Hl7.Fhir.Model;
using R4Model = R4::Hl7.Fhir.Model;
using R3Serialization = R3::Hl7.Fhir.Serialization;
using R4Serialization = R4::Hl7.Fhir.Serialization;
using Hl7.Fhir.Model;
using System.Reflection;

namespace FhirTool.Core.FhirWrappers
{
    public class ResourceWrapper
    {
        public FhirVersion FhirVersion { get; }
        public R3Model.Resource R3Resource { get; set; }
        public R4Model.Resource R4Resource { get; set; }

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
            get => GetId();
        }

        public ResourceTypeWrapper ResourceType
        {
            get => GetResourceType();
        }

        private ResourceTypeWrapper GetResourceType()
        {
            switch(FhirVersion)
            {
                case FhirVersion.R3:
                    return R3Resource.ResourceType.Wrap();
                case FhirVersion.R4:
                    return R4Resource.ResourceType.Wrap();
                default:
                    return default;
            }
        }

        private string GetId()
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

        public Base ToBase()
        {
            switch(FhirVersion)
            {
                case FhirVersion.R3:
                    return R3Resource as Base;
                case FhirVersion.R4:
                    return R4Resource as Base;
                default:
                    return default;
            }
        }

        public void SetProperty(string name, object value)
        {
            switch (FhirVersion)
            {
                case FhirVersion.R3:
                    R3Resource.GetType().InvokeMember(name, BindingFlags.SetProperty, null, R3Resource, new[] { value });
                    break;
                case FhirVersion.R4:
                    R4Resource.GetType().InvokeMember(name, BindingFlags.SetProperty, null, R4Resource, new[] { value });
                    break;
            }
        }
    }
}
