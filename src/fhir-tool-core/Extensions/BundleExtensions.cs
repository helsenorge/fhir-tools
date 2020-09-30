extern alias R3;
extern alias R4;

using EnsureThat;
using R3::Hl7.Fhir.Model;
using R4Model = R4::Hl7.Fhir.Model;
using System;

namespace FhirTool.Core
{
    public static class BundleExtensions
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

        public static void Append(this R4Model.Bundle bundle, R4Model.Resource resource)
        {
            EnsureArg.IsNotNull(resource, nameof(resource));

            bundle.Entry.Add(CreateEntryForResource(resource));
        }

        public static void Append(this R4Model.Bundle bundle, R4Model.Bundle.HTTPVerb method, R4Model.Resource resource)
        {
            EnsureArg.IsNotNull(resource, nameof(resource));

            var entry = CreateEntryForResource(resource);

            if (entry.Request == null) entry.Request = new R4Model.Bundle.RequestComponent();
            entry.Request.Method = method;
            bundle.Entry.Add(entry);
        }

        private static R4Model.Bundle.EntryComponent CreateEntryForResource(R4Model.Resource resource)
        {
            var entry = new R4Model.Bundle.EntryComponent
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
