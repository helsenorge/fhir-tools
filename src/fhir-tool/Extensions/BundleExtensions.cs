using EnsureThat;
using Hl7.Fhir.Model;
using System;

namespace FhirTool.Extensions
{
    internal static class BundleExtensions
    {
        public static void Append(this Bundle bundle, Resource resource)
        {
            EnsureArg.IsNotNull(resource, nameof(resource));

            bundle.Entry.Add(CreateEntryForResource(resource));
        }

        public static void Append(this Bundle bundle, Bundle.HTTPVerb method, Resource resource)
        {
            EnsureArg.IsNotNull(resource, nameof(resource));

            Bundle.EntryComponent entry = CreateEntryForResource(resource);

            if (entry.Request == null) entry.Request = new Bundle.RequestComponent();
            entry.Request.Method = method;
            bundle.Entry.Add(entry);
        }

        private static Bundle.EntryComponent CreateEntryForResource(Resource resource)
        {
            var entry = new Bundle.EntryComponent
            {
                Resource = resource,
                FullUrl = resource.ResourceBase == null 
                    ? $"urn:uuid:{Guid.NewGuid().ToString("D")}" 
                    : resource.ExtractKey().ToUriString()
            };

            return entry;
        }
    }
}
