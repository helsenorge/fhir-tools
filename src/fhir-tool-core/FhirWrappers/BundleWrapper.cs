extern alias R3;
extern alias R4;

using System;
using System.Linq;
using System.Collections.Generic;

using R3Model = R3::Hl7.Fhir.Model;
using R4Model = R4::Hl7.Fhir.Model;

namespace FhirTool.Core.FhirWrappers
{
    internal class BundleWrapper
    {
        public FhirVersion FhirVersion { get; }
        public R3Model.Bundle R3Bundle { get; }
        public R4Model.Bundle R4Bundle { get; }

        public BundleWrapper(R3Model.Bundle bundle)
        {
            FhirVersion = FhirVersion.R3;
            R3Bundle = bundle;
        }

        public BundleWrapper(R4Model.Bundle bundle)
        {
            FhirVersion = FhirVersion.R4;
            R4Bundle = bundle;
        }

        public IEnumerable<ResourceWrapper> GetResources()
        {
            switch(FhirVersion)
            {
                case FhirVersion.R3:
                    return R3Model.BundleExtensions.GetResources(R3Bundle).Select(it => new ResourceWrapper(it));
                case FhirVersion.R4:
                    return R4Model.BundleExtensions.GetResources(R4Bundle).Select(it => new ResourceWrapper(it));
                default:
                    return default;
            }
        }

        internal BundleWrapper UpdateLinks(Uri baseUrl)
        {
            switch(FhirVersion)
            {
                case FhirVersion.R3:
                    UpdateLinksR3(baseUrl);
                    break;
                case FhirVersion.R4:
                    UpdateLinksR4(baseUrl);
                    break;
            }

            return this;
        }

        private void UpdateLinksR3(Uri baseUrl)
        {
            foreach(var link in R3Bundle.Link)
            {
                link.Url = FixUrl(new Uri(link.Url), baseUrl);
            }
        }

        private void UpdateLinksR4(Uri baseUrl)
        {
            foreach (var link in R4Bundle.Link)
            {
                link.Url = FixUrl(new Uri(link.Url), baseUrl);
            }
        }

        private string FixUrl(Uri url, Uri baseUrl)
        {
            var urlBuilder = new UriBuilder(new Uri(baseUrl, url.Segments.Last()));
            urlBuilder.Query = url.Query;
            urlBuilder.Fragment = url.Fragment;

            return urlBuilder.ToString();
        }
    }
}
