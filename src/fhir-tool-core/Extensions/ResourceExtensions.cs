using EnsureThat;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace FhirTool.Core
{
    public static class ResourceExtensions
    {
        public static Bundle ToBundle(this IEnumerable<Resource> resources, Bundle.BundleType type = Bundle.BundleType.Collection, Uri base_ = null)
        {
            Bundle bundle = new Bundle
            {
                Id = $"urn:uuid:{Guid.NewGuid():N}",
                Type = type
            };
            foreach (Resource resource in resources)
            {
                // TODO: Investigate further when it is appropriate to add request method entries.
                if (type == Bundle.BundleType.Transaction)
                {
                    if (base_ != null)
                        resource.ResourceBase = base_;
                    // Make sure that resources without id's are posted.
                    if (resource.Id != null)
                    {
                        bundle.Append(Bundle.HTTPVerb.PUT, resource);
                    }
                    else
                    {
                        bundle.Append(Bundle.HTTPVerb.POST, resource);
                    }
                }
                else
                {
                    bundle.Append(resource);
                }
            }

            return bundle;
        }

        public static Key ExtractKey(this Resource resource)
        {
            string _base = (resource.ResourceBase != null) ? resource.ResourceBase.ToString() : null;
            Key key = new Key(_base, resource.TypeName, resource.Id, resource.VersionId);
            return key;
        }

        public static void SerializeResourceToDiskAsXml(this Resource resource, string path, bool pretty = true)
        {
            EnsureArg.IsNotNullOrWhiteSpace(path, nameof(path));

            using (XmlWriter writer = new XmlTextWriter(new StreamWriter(path)))
            {
                var serializer = new FhirXmlSerializer(new SerializerSettings { Pretty = pretty });
                serializer.Serialize(resource, writer);
            }
        }

        public static void SerializeResourceToDiskAsJson(this Resource resource, string path, bool pretty = true)
        {
            EnsureArg.IsNotNullOrWhiteSpace(path, nameof(path));

            using (JsonWriter writer = new JsonTextWriter(new StreamWriter(path)))
            {
                var serializer = new FhirJsonSerializer(new SerializerSettings { Pretty = pretty });
                serializer.Serialize(resource, writer);
            }
        }
    }
}
