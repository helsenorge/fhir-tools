/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

extern alias R3;
extern alias R4;

using R3Model = R3::Hl7.Fhir.Model;
using R4Model = R4::Hl7.Fhir.Model;

namespace FhirTool.Core.FhirWrappers
{
    public class RequestComponentWrapper
    {
        public FhirVersion FhirVersion { get; }
        public R4Model.Bundle.RequestComponent R4Object { get; }
        public R3Model.Bundle.RequestComponent R3Object { get; }

        public string Url
        {
            get => GetUrl();
            set => SetUrl(value);
        }

        public HTTPVerbWrapper Method
        {
            get => GetMethod();
            set => SetMethod(value);
        }

        public RequestComponentWrapper(FhirVersion fhirVersion)
        {
            FhirVersion = fhirVersion;
            switch (fhirVersion)
            {
                case FhirVersion.R3:
                    R3Object = new R3Model.Bundle.RequestComponent();
                    break;
                case FhirVersion.R4:
                    R4Object = new R4Model.Bundle.RequestComponent();
                    break;
            }
        }

        public RequestComponentWrapper(R3Model.Bundle.RequestComponent requestComponent)
        {
            FhirVersion = FhirVersion.R3;
            R3Object = requestComponent;
        }

        public RequestComponentWrapper(R4Model.Bundle.RequestComponent requestComponent)
        {
            FhirVersion = FhirVersion.R4;
            R4Object = requestComponent;
        }

        private void SetMethod(HTTPVerbWrapper value)
        {
            switch (FhirVersion)
            {
                case FhirVersion.R3:
                    R3Object.Method = value.ToR3();
                    break;
                case FhirVersion.R4:
                    R4Object.Method = value.ToR4();
                    break;
            }
        }

        private HTTPVerbWrapper GetMethod()
        {
            switch(FhirVersion)
            {
                case FhirVersion.R3:
                    return R3Object.Method.Wrap();
                case FhirVersion.R4:
                    return R4Object.Method.Wrap();
                default:
                    return default;
            }
        }

        private void SetUrl(string value)
        {
            switch (FhirVersion)
            {
                case FhirVersion.R3:
                    R3Object.Url = value;
                    break;
                case FhirVersion.R4:
                    R4Object.Url = value;
                    break;
            }
        }

        private string GetUrl()
        {
            switch (FhirVersion)
            {
                case FhirVersion.R3:
                    return R3Object.Url;
                case FhirVersion.R4:
                    return R4Object.Url;
                default:
                    return default;
            }
        }
    }
}
