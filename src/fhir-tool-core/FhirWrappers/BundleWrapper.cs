/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

extern alias R3;
extern alias R4;

using System;
using System.Linq;
using System.Collections.Generic;

using R3Model = R3::Hl7.Fhir.Model;
using R4Model = R4::Hl7.Fhir.Model;
using Hl7.Fhir.Model;

namespace FhirTool.Core.FhirWrappers
{
    public class BundleWrapper
    {
        public FhirVersion FhirVersion { get; }
        public R3Model.Bundle R3Bundle { get; }
        public R4Model.Bundle R4Bundle { get; }
        public BundleTypeWrapper Type
        {
            get => GetTypeValue();
            set => SetTypeValue(value);
        }

        public BundleWrapper(FhirVersion version)
        {
            FhirVersion = version;
            switch(version)
            {
                case FhirVersion.R3:
                    R3Bundle = new R3Model.Bundle();
                    break;
                case FhirVersion.R4:
                    R4Bundle = new R4Model.Bundle();
                    break;
            }
        }

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

        public EntryComponentWrapper AddResourceEntry(ResourceWrapper resource, string fullUri)
        {
            switch(FhirVersion)
            {
                case FhirVersion.R3:
                    return new EntryComponentWrapper(R3Model.BundleExtensions.AddResourceEntry(R3Bundle, resource.Resource, fullUri));
                case FhirVersion.R4:
                    return new EntryComponentWrapper(R4Model.BundleExtensions.AddResourceEntry(R4Bundle, resource.Resource, fullUri));
                default:
                    return default;
            }
        }

        public IEnumerable<ResourceWrapper> GetResources()
        {
            switch(FhirVersion)
            {
                case FhirVersion.R3:
                    return R3Model.BundleExtensions.GetResources(R3Bundle).Select(it => new ResourceWrapper(it, FhirVersion.R3));
                case FhirVersion.R4:
                    return R4Model.BundleExtensions.GetResources(R4Bundle).Select(it => new ResourceWrapper(it, FhirVersion.R4));
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

        public Base ToBase()
        {
            switch (FhirVersion)
            {
                case FhirVersion.R3:
                    return R3Bundle as Base;
                case FhirVersion.R4:
                    return R4Bundle as Base;
                default:
                    return default;
            }
        }

        private void SetTypeValue(BundleTypeWrapper value)
        {
            switch (FhirVersion)
            {
                case FhirVersion.R3:
                    R3Bundle.Type = value.ToR3();
                    break;
                case FhirVersion.R4:
                    R4Bundle.Type = value.ToR4();
                    break;
            }
        }

        private BundleTypeWrapper GetTypeValue()
        {
            switch (FhirVersion)
            {
                case FhirVersion.R3:
                    return R3Bundle.Type.Wrap();
                case FhirVersion.R4:
                    return R4Bundle.Type.Wrap();
                default:
                    return default;
            }
        }

    }
}
