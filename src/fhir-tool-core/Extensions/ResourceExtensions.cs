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

using EnsureThat;
using Newtonsoft.Json;
using System.IO;
using System.Xml;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using FhirTool.Core.FhirWrappers;

namespace FhirTool.Core
{
    public static class ResourceExtensions
    {
        public static Key ExtractKey(this Resource resource)
        {
            string _base = (resource.ResourceBase != null) ? resource.ResourceBase.ToString() : null;
            Key key = new Key(_base, resource.TypeName, resource.Id, resource.VersionId);
            return key;
        }

        public static void SerializeResourceToDisk(this Resource resource, FhirVersion version, string path, string mimeType, bool pretty = true)
        {
            switch(mimeType.ToLowerInvariant())
            {
                case "xml":
                    resource.SerializeResourceToDiskAsXml(path, version);
                    break;
                case "json":
                    resource.SerializeResourceToDiskAsJson(path, version);
                    break;
                default:
                    throw new UnknownMimeTypeException(mimeType);
            }
        }

        public static void SerializeResourceToDiskAsXml(this Resource resource, string path, FhirVersion version, bool pretty = true)
        {
            switch (version)
            {
                case FhirVersion.R3:
                    SerializeResourceToDiskAsXmlR3(resource, path, pretty);
                    break;
                case FhirVersion.R4:
                    SerializeResourceToDiskAsXmlR4(resource, path, pretty);
                    break;
            }
        }

        private static void SerializeResourceToDiskAsXmlR3(this Resource resource, string path, bool pretty = true)
        {
            EnsureArg.IsNotNullOrWhiteSpace(path, nameof(path));

            using XmlWriter writer = new XmlTextWriter(new StreamWriter(path));
            var serializer = new R3Serialization.FhirXmlSerializer(new SerializerSettings { Pretty = pretty });
            serializer.Serialize(resource, writer);
        }

        private static void SerializeResourceToDiskAsXmlR4(this Resource resource, string path, bool pretty = true)
        {
            EnsureArg.IsNotNullOrWhiteSpace(path, nameof(path));

            using XmlWriter writer = new XmlTextWriter(new StreamWriter(path));
            var serializer = new R4Serialization.FhirXmlSerializer(new SerializerSettings { Pretty = pretty });
            serializer.Serialize(resource, writer);
        }

        public static void SerializeResourceToDiskAsJson(this Resource resource, string path, FhirVersion version, bool pretty = true)
        {
            switch (version)
            {
                case FhirVersion.R3:
                    SerializeResourceToDiskAsJsonR3(resource, path, pretty);
                    break;
                case FhirVersion.R4:
                    SerializeResourceToDiskAsJsonR4(resource, path, pretty);
                    break;
            }
        }
        private static void SerializeResourceToDiskAsJsonR3(this Resource resource, string path, bool pretty = true)
        {
            EnsureArg.IsNotNullOrWhiteSpace(path, nameof(path));

            using JsonWriter writer = new JsonTextWriter(new StreamWriter(path));
            var serializer = new R3Serialization.FhirJsonSerializer(new SerializerSettings { Pretty = pretty });
            serializer.Serialize(resource, writer);
        }

        private static void SerializeResourceToDiskAsJsonR4(this Resource resource, string path, bool pretty = true)
        {
            EnsureArg.IsNotNullOrWhiteSpace(path, nameof(path));

            using JsonWriter writer = new JsonTextWriter(new StreamWriter(path));
            var serializer = new R4Serialization.FhirJsonSerializer(new SerializerSettings { Pretty = pretty });
            serializer.Serialize(resource, writer);
        }

        public static ResourceTypeWrapper ResourceType(this Resource it, FhirVersion v)
        {
            var wrapper = new ResourceWrapper(it, v);
            return wrapper.ResourceType;
        }
    }
}
