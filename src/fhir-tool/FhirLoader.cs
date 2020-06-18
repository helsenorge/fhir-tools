using EnsureThat;
using FhirTool.Core;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace FhirTool
{
    internal static class FhirLoader
    {
        public static bool IsKnownResourceType(string data)
        {
            EnsureArg.IsNotNull(data, nameof(data));

            if (SerializationUtil.ProbeIsJson(data))
            {
                JObject resource = JObject.Parse(data);
                return resource != null
                    && resource.ContainsKey("resourceType")
                    && !string.IsNullOrEmpty(resource["resourceType"].ToString())
                    && ModelInfo.IsKnownResource(resource["resourceType"].ToString());
            }
            else if(SerializationUtil.ProbeIsXml(data))
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
        
        public static IEnumerable<Resource> ImportFolder(string path)
        {
            List<Resource> resources = new List<Resource>();

            IEnumerable<string> paths = Directory.EnumerateFileSystemEntries(path);
            foreach (string p in paths)
            {
                resources.AddRange(IOHelpers.IsDirectory(p) ? ImportFolder(p) : ImportFile(p));
            }

            return resources;
        }

        public static IEnumerable<Resource> ImportFile(string filename)
        {
            EnsureArg.IsNotNullOrWhiteSpace(filename, nameof(filename));

            string data = File.ReadAllText(filename);
            return ImportData(data);
        }

        public static IEnumerable<string> ExtractZipEntries(this byte[] buffer)
        {
            using (Stream stream = new MemoryStream(buffer))
            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    StreamReader reader = new StreamReader(entry.Open());
                    string data = reader.ReadToEnd();
                    yield return data;
                }
            }
        }

        public static IEnumerable<Resource> ExtractResourcesFromZip(this byte[] buffer)
        {
            return buffer.ExtractZipEntries().SelectMany(ImportData);
        }

        public static IEnumerable<Resource> ImportZip(string filename)
        {
            EnsureArg.IsNotNullOrWhiteSpace(filename, nameof(filename));

            return File.ReadAllBytes(filename).ExtractZipEntries().SelectMany(ImportData); ;
        }

    }
}