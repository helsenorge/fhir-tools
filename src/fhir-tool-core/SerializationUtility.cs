/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

extern alias R4;

using EnsureThat;
using R4::Hl7.Fhir.Model;
using R4::Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Tasks = System.Threading.Tasks;
using Hl7.Fhir.Model;

namespace FhirTool.Core
{
    internal static class SerializationUtility
    {
        public static async Tasks.Task<T> DeserializeResource<T>(string path)
            where T : Base
        {
            T resource = default;
            using StreamReader reader = new StreamReader(path);
            string data = await reader.ReadToEndAsync();
            if (SerializationUtil.ProbeIsJson(data))
            {
                resource = DeserializeJsonResource<T>(data);
            }
            else if(SerializationUtil.ProbeIsXml(data))
            {
                resource = DeserializeXmlResource<T>(data);
            }
            return resource;
        }

        public static bool ProbeIsJsonOrXml(string path)
        {
            using StreamReader reader = new StreamReader(path);
            string data = reader.ReadToEnd();
            return SerializationUtil.ProbeIsJson(data) 
                || SerializationUtil.ProbeIsXml(data);
        }

        private static T DeserializeJsonResource<T>(string data)
            where T : Base
        {
            FhirJsonParser parser = new FhirJsonParser(new ParserSettings { PermissiveParsing = false });
            return parser.Parse<T>(data);
        }

        private static T DeserializeXmlResource<T>(string data)
            where T : Base
        {
            FhirXmlParser parser = new FhirXmlParser(new ParserSettings { PermissiveParsing = false });
            return parser.Parse<T>(data);
        }

        public static bool IsKnownResourceType(string data)
        {
            EnsureArg.IsNotNull(data, nameof(data));

            if (SerializationUtil.ProbeIsJson(data))
            {
                JObject resource = JObject.Parse(data);
                return resource != null
                    && resource.ContainsKey("resourceType")
                    && !string.IsNullOrWhiteSpace(resource["resourceType"].ToString())
                    && ModelInfo.IsKnownResource(resource["resourceType"].ToString());
            }
            else if (SerializationUtil.ProbeIsXml(data))
            {
                XDocument resource = XDocument.Parse(data);
                return resource != null
                    && resource.Root != null
                    && ModelInfo.IsKnownResource(resource.Root.Name.LocalName);
            }

            return false;
        }

        private static Resource ParseResource(string data)
        {
            if (SerializationUtil.ProbeIsJson(data))
            {
                return new FhirJsonParser().Parse<Resource>(data);
            }
            else if (SerializationUtil.ProbeIsXml(data))
            {
                return new FhirXmlParser().Parse<Resource>(data);
            }
            else
            {
                throw new FormatException("Data is neither Json nor Xml");
            }
        }

        public static IEnumerable<Resource> ImportData(string data)
        {
            EnsureArg.IsNotNull(data, nameof(data));

            if (!IsKnownResourceType(data)) return new Resource[] { }; ;

            Resource resource = ParseResource(data);
            if (resource is Bundle)
            {
                Bundle bundle = (resource as Bundle);
                return bundle.GetResources();
            }
            else
            {
                return new Resource[] { resource };
            }
        }

        public static IEnumerable<Resource> ImportPath(string path)
        {
            if (IOUtility.IsDirectory(path))
            {
                return ImportFolder(path);
            }
            else
            {
                return ImportFile(path);
            }
        }

        public static IEnumerable<Resource> ImportFolder(string path)
        {
            List<Resource> resources = new List<Resource>();

            IEnumerable<string> paths = Directory.EnumerateFileSystemEntries(path);
            foreach (string p in paths)
            {
                resources.AddRange(IOUtility.IsDirectory(p) ? ImportFolder(p) : ImportFile(p));
            }

            return resources;
        }

        public static IEnumerable<Resource> ImportFile(string filename)
        {
            EnsureArg.IsNotNullOrWhiteSpace(filename, nameof(filename));

            string data = File.ReadAllText(filename);
            return ImportData(data);
        }
    }
}
