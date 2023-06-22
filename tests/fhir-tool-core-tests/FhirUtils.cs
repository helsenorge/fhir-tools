/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

extern alias R3;
extern alias R4;

using System.IO;

using R3Serialization = R3::Hl7.Fhir.Serialization;
using R4Serialization = R4::Hl7.Fhir.Serialization;
using EnsureThat;
using Newtonsoft.Json;
using System.Xml;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace FhirTool.Conversion.Tests
{
    public abstract class FhirTestsBase
    {
        private readonly R3Serialization.FhirJsonParser _r3JsonParser = new R3Serialization.FhirJsonParser(new ParserSettings { PermissiveParsing = true });
        private readonly R4Serialization.FhirJsonParser _r4JsonParser = new R4Serialization.FhirJsonParser(new ParserSettings { PermissiveParsing = true });

        protected Resource ReadR3Resource(string path)
        {
            string json;
            using (TextReader reader = new StreamReader(path))
                json = reader.ReadToEnd();
            return (Resource)_r3JsonParser.Parse(json);
        }

        protected Resource ReadR4Resource(string path)
        {
            string json;
            using (TextReader reader = new StreamReader(path))
                json = reader.ReadToEnd();
            return (Resource)_r4JsonParser.Parse(json);
        }

        protected void SerializeR4ResourceToDiskAsJson(Resource resource, string path, bool pretty = true)
        {
            EnsureArg.IsNotNullOrWhiteSpace(path, nameof(path));

            using (JsonWriter writer = new JsonTextWriter(new StreamWriter(path)))
            {
                var serializer = new R4Serialization.FhirJsonSerializer(new SerializerSettings { Pretty = pretty });
                serializer.Serialize(resource, writer);
            }
        }

        protected void SerializeR4ResourceToDiskAsXml(Resource resource, string path, bool pretty = true)
        {
            EnsureArg.IsNotNullOrWhiteSpace(path, nameof(path));

            using (XmlWriter writer = new XmlTextWriter(new StreamWriter(path)))
            {
                var serializer = new R4Serialization.FhirXmlSerializer(new SerializerSettings { Pretty = pretty });
                serializer.Serialize(resource, writer);
            }
        }

        protected void SerializeR3ResourceToDiskAsJson(Resource resource, string path, bool pretty = true)
        {
            EnsureArg.IsNotNullOrWhiteSpace(path, nameof(path));

            using (JsonWriter writer = new JsonTextWriter(new StreamWriter(path)))
            {
                var serializer = new R3Serialization.FhirJsonSerializer(new SerializerSettings { Pretty = pretty });
                serializer.Serialize(resource, writer);
            }
        }

        protected void SerializeR3ResourceToDiskAsXml(Resource resource, string path, bool pretty = true)
        {
            EnsureArg.IsNotNullOrWhiteSpace(path, nameof(path));

            using (XmlWriter writer = new XmlTextWriter(new StreamWriter(path)))
            {
                var serializer = new R3Serialization.FhirXmlSerializer(new SerializerSettings { Pretty = pretty });
                serializer.Serialize(resource, writer);
            }
        }
    }
}
