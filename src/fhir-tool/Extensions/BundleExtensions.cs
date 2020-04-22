using Hl7.Fhir.Model;
using System;

namespace FhirTool.Extensions
{
    internal static class BundleExtensions
    {
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
    }
}
