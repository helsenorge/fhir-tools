extern alias R3;
extern alias R4;

using R3Model = R3::Hl7.Fhir.Model;
using R4Model = R4::Hl7.Fhir.Model;
using R3Serialization = R3::Hl7.Fhir.Serialization;
using R4Serialization = R4::Hl7.Fhir.Serialization;

using Hl7.Fhir.Utility;
using Hl7.Fhir.Model;
using FhirTool.Core.Utils;

namespace FhirTool.Core.FhirWrappers
{
    public enum FhirMimeType
    {
        Xml = 1,
        Json = 2
    }

    public class SerializationWrapper
    {
        public FhirVersion Version { get; }

        public SerializationWrapper(FhirVersion version)
        {
            Version = version;
        }

        public string Serialize(ResourceWrapper resourceWrapper, FhirMimeType type = FhirMimeType.Json)
        {
            switch(Version)
            {
                case FhirVersion.R3:
                    return Serialize(resourceWrapper.R3Resource, type);
                case FhirVersion.R4:
                    return Serialize(resourceWrapper.R4Resource, type);
                default:
                    return default;
            }
        }

        public string Serialize(Base resource, FhirMimeType type = FhirMimeType.Json)
        {
            switch(Version)
            {
                case FhirVersion.R3:
                    var settingsR3 = new R3Serialization.SerializerSettings { Pretty = true };
                    return type == FhirMimeType.Json
                        ? new R3Serialization.FhirJsonSerializer(settingsR3).SerializeToString(resource)
                        : new R3Serialization.FhirXmlSerializer(settingsR3).SerializeToString(resource);
                case FhirVersion.R4:
                    var settingsR4 = new R4Serialization.SerializerSettings { Pretty = true };
                    return type == FhirMimeType.Json

                        ? new R4Serialization.FhirJsonSerializer(settingsR4).SerializeToString(resource)
                        : new R4Serialization.FhirXmlSerializer(settingsR4).SerializeToString(resource);
                default:
                    return default;
            }
        }

        public ResourceWrapper Parse(string content, FhirMimeType? type = null, bool permissiveParsing = false)
        {
            if (!type.HasValue)
            {
                type = MimeTypeUtils.ProbeFhirMimeType(content);
            }
            
            if(!type.HasValue)
            {
                return null;
            }

            switch(Version)
            {
                case FhirVersion.R3:
                    var settingsR3 = new R3Serialization.ParserSettings { PermissiveParsing = permissiveParsing };
                    var resourceR3 = type == FhirMimeType.Json
                        ? new R3Serialization.FhirJsonParser(settingsR3).Parse<R3Model.Resource>(content)
                        : new R3Serialization.FhirXmlParser(settingsR3).Parse<R3Model.Resource>(content);
                    return new ResourceWrapper(resourceR3);
                case FhirVersion.R4:
                    var settingsR4 = new R4Serialization.ParserSettings { PermissiveParsing = permissiveParsing };
                    var resourceR4 = type == FhirMimeType.Json
                        ? new R4Serialization.FhirJsonParser(settingsR4).Parse<R4Model.Resource>(content)
                        : new R4Serialization.FhirXmlParser(settingsR4).Parse<R4Model.Resource>(content);
                    return new ResourceWrapper(resourceR4);
                default:
                    return default;
            }
        }
    }
}
