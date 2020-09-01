extern alias R3;
extern alias R4;

using R3Serialization = R3::Hl7.Fhir.Serialization;
using R4Serialization = R4::Hl7.Fhir.Serialization;

using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;

namespace FhirTool.Core.FhirWrappers
{
    public enum FhirMimeType
    {
        FhirXml = 1,
        FhirJson = 2
    }

    public class SerializationWrapper
    {
        public FhirVersion Version { get; }

        public SerializationWrapper(FhirVersion version)
        {
            Version = version;
        }

        public string Serialize(Base resource, FhirMimeType type = FhirMimeType.FhirJson)
        {
            switch(Version)
            {
                case FhirVersion.R3:
                    var settingsR3 = new R3Serialization.SerializerSettings { Pretty = true };
                    return type == FhirMimeType.FhirJson
                        ? new R3Serialization.FhirJsonSerializer(settingsR3).SerializeToString(resource)
                        : new R3Serialization.FhirXmlSerializer(settingsR3).SerializeToString(resource);
                case FhirVersion.R4:
                    var settingsR4 = new R4Serialization.SerializerSettings { Pretty = true };
                    return type == FhirMimeType.FhirJson

                        ? new R4Serialization.FhirJsonSerializer(settingsR4).SerializeToString(resource)
                        : new R4Serialization.FhirXmlSerializer(settingsR4).SerializeToString(resource);
                default:
                    return default;
            }
        }

        public Base Parse(string content, FhirMimeType? type = null)
        {
            if (!type.HasValue)
            {
                type = ProbeFhirMimeType(content);
            }
            
            if(!type.HasValue)
            {
                return null;
            }

            switch(Version)
            {
                case FhirVersion.R3:
                    var settingsR3 = new R3Serialization.ParserSettings { PermissiveParsing = false };
                    return type == FhirMimeType.FhirJson
                        ? new R3Serialization.FhirJsonParser(settingsR3).Parse(content)
                        : new R3Serialization.FhirXmlParser(settingsR3).Parse(content);
                case FhirVersion.R4:
                    var settingsR4 = new R4Serialization.ParserSettings { PermissiveParsing = false };
                    return type == FhirMimeType.FhirJson
                        ? new R4Serialization.FhirJsonParser(settingsR4).Parse(content)
                        : new R4Serialization.FhirXmlParser(settingsR4).Parse(content);
                default:
                    return default;
            }
        }

        private FhirMimeType? ProbeFhirMimeType(string content)
        {
            if(SerializationUtil.ProbeIsJson(content))
            {
                return FhirMimeType.FhirJson;
            }
            if(SerializationUtil.ProbeIsXml(content))
            {
                return FhirMimeType.FhirXml;
            }

            return null;
        }
    }
}
