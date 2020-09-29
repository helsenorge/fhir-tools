extern alias R3;
extern alias R4;

using EnsureThat;
using R3::Hl7.Fhir.Model;
using R3::Hl7.Fhir.Serialization;

using R4Model = R4::Hl7.Fhir.Model;
using R4Serialization = R4::Hl7.Fhir.Serialization;

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

        public static void SerializeResourceToDisk(this Resource resource, string path, string mimeType, bool pretty = true)
        {
            switch(mimeType.ToLowerInvariant())
            {
                case "xml":
                    resource.SerializeResourceToDiskAsXml(path);
                    break;
                case "json":
                    resource.SerializeResourceToDiskAsJson(path);
                    break;
                default:
                    throw new UnknownMimeTypeException(mimeType);
            }
        }

        public static void SerializeResourceToDiskAsXml(this Resource resource, string path, bool pretty = true)
        {
            EnsureArg.IsNotNullOrWhiteSpace(path, nameof(path));

            using XmlWriter writer = new XmlTextWriter(new StreamWriter(path));
            var serializer = new FhirXmlSerializer(new SerializerSettings { Pretty = pretty });
            serializer.Serialize(resource, writer);
        }

        public static void SerializeResourceToDiskAsJson(this Resource resource, string path, bool pretty = true)
        {
            EnsureArg.IsNotNullOrWhiteSpace(path, nameof(path));

            using JsonWriter writer = new JsonTextWriter(new StreamWriter(path));
            var serializer = new FhirJsonSerializer(new SerializerSettings { Pretty = pretty });
            serializer.Serialize(resource, writer);
        }

        public static R4Model.Bundle ToBundle(this IEnumerable<R4Model.Resource> resources, R4Model.Bundle.BundleType type = R4Model.Bundle.BundleType.Collection, Uri base_ = null)
        {
            var bundle = new R4Model.Bundle
            {
                Id = $"urn:uuid:{Guid.NewGuid():N}",
                Type = type
            };
            foreach (var resource in resources)
            {
                // TODO: Investigate further when it is appropriate to add request method entries.
                if (type == R4Model.Bundle.BundleType.Transaction)
                {
                    if (base_ != null)
                        resource.ResourceBase = base_;
                    // Make sure that resources without id's are posted.
                    if (resource.Id != null)
                    {
                        bundle.Append(R4Model.Bundle.HTTPVerb.PUT, resource);
                    }
                    else
                    {
                        bundle.Append(R4Model.Bundle.HTTPVerb.POST, resource);
                    }
                }
                else
                {
                    bundle.Append(resource);
                }
            }

            return bundle;
        }

        public static Key ExtractKey(this R4Model.Resource resource)
        {
            string _base = (resource.ResourceBase != null) ? resource.ResourceBase.ToString() : null;
            Key key = new Key(_base, resource.TypeName, resource.Id, resource.VersionId);
            return key;
        }

        public static void SerializeResourceToDisk(this R4Model.Resource resource, string path, string mimeType, bool pretty = true)
        {
            switch (mimeType.ToLowerInvariant())
            {
                case "xml":
                    resource.SerializeResourceToDiskAsXml(path);
                    break;
                case "json":
                    resource.SerializeResourceToDiskAsJson(path);
                    break;
                default:
                    throw new UnknownMimeTypeException(mimeType);
            }
        }

        public static void SerializeResourceToDiskAsXml(this R4Model.Resource resource, string path, bool pretty = true)
        {
            EnsureArg.IsNotNullOrWhiteSpace(path, nameof(path));

            using XmlWriter writer = new XmlTextWriter(new StreamWriter(path));
            var serializer = new R4Serialization.FhirXmlSerializer(new R4Serialization.SerializerSettings { Pretty = pretty });
            serializer.Serialize(resource, writer);
        }

        public static void SerializeResourceToDiskAsJson(this R4Model.Resource resource, string path, bool pretty = true)
        {
            EnsureArg.IsNotNullOrWhiteSpace(path, nameof(path));

            using JsonWriter writer = new JsonTextWriter(new StreamWriter(path));
            var serializer = new R4Serialization.FhirJsonSerializer(new R4Serialization.SerializerSettings { Pretty = pretty });
            serializer.Serialize(resource, writer);
        }
    }
}
