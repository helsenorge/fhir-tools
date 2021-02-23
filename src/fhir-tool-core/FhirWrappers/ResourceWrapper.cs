/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

extern alias R3;
extern alias R4;

using R3Serialization = R3::Hl7.Fhir.Serialization;
using R4Serialization = R4::Hl7.Fhir.Serialization;

using R3Extension = R3::Hl7.Fhir.Model.ModelInfoExtensions;
using R4Extension = R4::Hl7.Fhir.Model.ModelInfoExtensions;

using Hl7.Fhir.Model;
using System.Reflection;
using Hl7.Fhir.Serialization;

namespace FhirTool.Core.FhirWrappers
{
    public class ResourceWrapper
    {
        public FhirVersion FhirVersion { get; }
        public Resource Resource { get; set; }

        public ResourceWrapper(Resource resource, FhirVersion version)
        {
            FhirVersion = version;
            Resource = resource;
        }

        public string Id
        {
            get => Resource.Id;
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
                    R3Extension.TryDeriveResourceType(Resource, out var r3Type);
                    return r3Type.Wrap();
                case FhirVersion.R4:
                    R4Extension.TryDeriveResourceType(Resource, out var r4Type);
                    return r4Type.Wrap();
                default:
                    return default;
            }
        }

        public string Serialize()
        {
            switch(FhirVersion)
            {
                case FhirVersion.R3:
                    return new R3Serialization.FhirJsonSerializer(new SerializerSettings { Pretty = true }).SerializeToString(Resource);
                case FhirVersion.R4:
                    return new R4Serialization.FhirJsonSerializer(new SerializerSettings { Pretty = true }).SerializeToString(Resource);
                default:
                    return default;
            }
        }

        public void SetProperty(string name, object value)
        {
            Resource.GetType().InvokeMember(name, BindingFlags.SetProperty, null, Resource, new[] { value });
        }
    }
}
