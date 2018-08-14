using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace FhirTool.Extensions
{
    public static class FhirExtensionMethods
    {
        public static Bundle ToBundle(this IEnumerable<Resource> resources, Bundle.BundleType type = Bundle.BundleType.Collection, Uri base_ = null)
        {
            Bundle bundle = new Bundle
            {
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
            Key a = new Key();
            return key;
        }

        public static void Append(this Bundle bundle, Resource resource)
        {
            bundle.Entry.Add(CreateEntryForResource(resource));
        }

        public static void Append(this Bundle bundle, Bundle.HTTPVerb method, Resource resource)
        {
            Bundle.EntryComponent entry = CreateEntryForResource(resource);

            if (entry.Request == null) entry.Request = new Bundle.RequestComponent();
            entry.Request.Method = method;
            bundle.Entry.Add(entry);
        }

        private static Bundle.EntryComponent CreateEntryForResource(Resource resource)
        {
            var entry = new Bundle.EntryComponent();
            entry.Resource = resource;
            //            entry.FullUrl = resource.ResourceIdentity().ToString();
            if (resource.ResourceBase == null)
                entry.FullUrl = $"urn:uuid:{Guid.NewGuid().ToString("D")}";
            else
                entry.FullUrl = resource.ExtractKey().ToUriString();

            return entry;
        }
        
        public static void SerializeResourceToDiskAsXml(this Resource resource, string path)
        {
            using (XmlWriter writer = new XmlTextWriter(new StreamWriter(path)))
            {
                var serializer = new FhirXmlSerializer();
                serializer.Serialize(resource, writer);
            }
        }

        public static void SerializeResourceToDiskAsJson(this Resource resource, string path)
        {
            using (JsonWriter writer = new JsonTextWriter(new StreamWriter(path)))
            {
                var serializer = new FhirJsonSerializer();
                serializer.Serialize(resource, writer);
            }
        }

        public static Narrative GenerateNarrative(this ValueSet valueSet)
        {
            StringBuilder div = new StringBuilder();
            div.Append("<div xmlns=\"http://www.w3.org/1999/xhtml\">");
            div.Append($"<p>Value set \"{valueSet.Id}: {valueSet.Name}\"</p>");
            div.Append($"<p>Developed by: {valueSet.Publisher}</p>");
            div.Append($"<p>Published for testing purposes on {valueSet.Date}</p>");
            div.Append($"<p>This value set contains the following codes from code system: {valueSet.Id}</p>");
            div.Append("<ol>");
            foreach (ValueSet.ConceptSetComponent conceptSet in valueSet.Compose.Include)
            {
                foreach (ValueSet.ConceptReferenceComponent concept in conceptSet.Concept)
                {
                    div.Append($"<li>{concept.Code}: {concept.Display}</li>");
                }
            }
            div.Append("</ol>");
            div.Append("</div>");

            return new Narrative
            {
                Status = Narrative.NarrativeStatus.Generated,
                Div = div.ToString()
            };
        }

        public static void GenerateAndSetNarrative(this ValueSet valueSet)
        {
            StringBuilder div = new StringBuilder();
            div.Append("<div xmlns=\"http://www.w3.org/1999/xhtml\">");
            div.Append($"<p>Value set \"{valueSet.Id}: {valueSet.Name}\"</p>");
            div.Append($"<p>Developed by: {valueSet.Publisher}</p>");
            div.Append($"<p>Published for testing purposes on {valueSet.Date}</p>");
            div.Append($"<p>This value set contains the following codes from code system: {valueSet.Id}</p>");
            div.Append("<ol>");
            foreach (ValueSet.ConceptSetComponent conceptSet in valueSet.Compose.Include)
            {
                foreach (ValueSet.ConceptReferenceComponent concept in conceptSet.Concept)
                {
                    div.Append($"<li>{concept.Code}: {concept.Display}</li>");
                }
            }
            div.Append("</ol>");
            div.Append("</div>");

            valueSet.Text = GenerateNarrative(valueSet);
        }
    }
}
