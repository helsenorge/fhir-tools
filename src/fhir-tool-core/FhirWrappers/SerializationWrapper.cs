/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

extern alias R3;
extern alias R4;

using R3Model = R3::Hl7.Fhir.Model;
using R4Model = R4::Hl7.Fhir.Model;
using R3Serialization = R3::Hl7.Fhir.Serialization;
using R4Serialization = R4::Hl7.Fhir.Serialization;

using Hl7.Fhir.Utility;
using Hl7.Fhir.Model;
using FhirTool.Core.Utils;
using Hl7.Fhir.Serialization;

namespace FhirTool.Core.FhirWrappers
{
    public enum FhirMimeType
    {
        [EnumLiteral("xml")]
        Xml = 1,
        [EnumLiteral("json")]
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
            return Serialize(resourceWrapper.Resource, type);
        }

        public string Serialize(Base resource, FhirMimeType type = FhirMimeType.Json)
        {
            switch(Version)
            {
                case FhirVersion.R3:
                    var settingsR3 = new SerializerSettings { Pretty = true };
                    return type == FhirMimeType.Json
                        ? new R3Serialization.FhirJsonSerializer(settingsR3).SerializeToString(resource)
                        : new R3Serialization.FhirXmlSerializer(settingsR3).SerializeToString(resource);
                case FhirVersion.R4:
                    var settingsR4 = new SerializerSettings { Pretty = true };
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
                    var settingsR3 = new ParserSettings { PermissiveParsing = permissiveParsing };
                    var resourceR3 = type == FhirMimeType.Json
                         ? new R3Serialization.FhirJsonParser(settingsR3).Parse<Resource>(content)
                        : new R3Serialization.FhirXmlParser(settingsR3).Parse<Resource>(content);
                    return new ResourceWrapper(resourceR3, FhirVersion.R3);
                case FhirVersion.R4:
                    var settingsR4 = new ParserSettings { PermissiveParsing = permissiveParsing };
                    var resourceR4 = type == FhirMimeType.Json
                        ? new R4Serialization.FhirJsonParser(settingsR4).Parse<Resource>(content)
                        : new R4Serialization.FhirXmlParser(settingsR4).Parse<Resource>(content);
                    return new ResourceWrapper(resourceR4, FhirVersion.R4);
                default:
                    return default;
            }
        }
    }
}
