/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

extern alias R3;
extern alias R4;

using System.IO;

using R3Model = R3::Hl7.Fhir.Model;
using R4Model = R4::Hl7.Fhir.Model;
using R3Serialization = R3::Hl7.Fhir.Serialization;
using R4Serialization = R4::Hl7.Fhir.Serialization;
using EnsureThat;
using Newtonsoft.Json;
using System.Xml;

namespace FhirTool.Conversion.Tests
{
    public abstract class FhirTestsBase
    {
        private readonly R3Serialization.FhirJsonParser _r3JsonParser = new R3Serialization.FhirJsonParser(new R3Serialization.ParserSettings { PermissiveParsing = true });
        private readonly R4Serialization.FhirJsonParser _r4JsonParser = new R4Serialization.FhirJsonParser(new R4Serialization.ParserSettings { PermissiveParsing = true });

        protected R3Model.Resource ReadR3Resource(string path)
        {
            string json;
            using (TextReader reader = new StreamReader(path))
                json = reader.ReadToEnd();
            return (R3Model.Resource)_r3JsonParser.Parse(json);
        }

        protected R4Model.Resource ReadR4Resource(string path)
        {
            string json;
            using (TextReader reader = new StreamReader(path))
                json = reader.ReadToEnd();
            return (R4Model.Resource)_r4JsonParser.Parse(json);
        }

        protected void SerializeR4ResourceToDiskAsJson(R4Model.Resource resource, string path, bool pretty = true)
        {
            EnsureArg.IsNotNullOrWhiteSpace(path, nameof(path));

            using (JsonWriter writer = new JsonTextWriter(new StreamWriter(path)))
            {
                var serializer = new R4Serialization.FhirJsonSerializer(new R4Serialization.SerializerSettings { Pretty = pretty });
                serializer.Serialize(resource, writer);
            }
        }

        protected void SerializeR4ResourceToDiskAsXml(R4Model.Resource resource, string path, bool pretty = true)
        {
            EnsureArg.IsNotNullOrWhiteSpace(path, nameof(path));

            using (XmlWriter writer = new XmlTextWriter(new StreamWriter(path)))
            {
                var serializer = new R4Serialization.FhirXmlSerializer(new R4Serialization.SerializerSettings { Pretty = pretty });
                serializer.Serialize(resource, writer);
            }
        }

        protected void SerializeR3ResourceToDiskAsJson(R3Model.Resource resource, string path, bool pretty = true)
        {
            EnsureArg.IsNotNullOrWhiteSpace(path, nameof(path));

            using (JsonWriter writer = new JsonTextWriter(new StreamWriter(path)))
            {
                var serializer = new R3Serialization.FhirJsonSerializer(new R3Serialization.SerializerSettings { Pretty = pretty });
                serializer.Serialize(resource, writer);
            }
        }

        protected void SerializeR3ResourceToDiskAsXml(R3Model.Resource resource, string path, bool pretty = true)
        {
            EnsureArg.IsNotNullOrWhiteSpace(path, nameof(path));

            using (XmlWriter writer = new XmlTextWriter(new StreamWriter(path)))
            {
                var serializer = new R3Serialization.FhirXmlSerializer(new R3Serialization.SerializerSettings { Pretty = pretty });
                serializer.Serialize(resource, writer);
            }
        }
    }
}
