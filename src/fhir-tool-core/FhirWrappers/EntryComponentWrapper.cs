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
    public class EntryComponentWrapper
    {
        public FhirVersion FhirVersion { get; }
        public R3Model.Bundle.EntryComponent R3EntryComponent { get; set; }
        public R4Model.Bundle.EntryComponent R4EntryComponent { get; set; }
        public RequestComponentWrapper Request
        {
            get => GetRequest();
            set => SetRequest(value);
        }

        public EntryComponentWrapper(R3Model.Bundle.EntryComponent entryComponent)
        {
            R3EntryComponent = entryComponent;
            FhirVersion = FhirVersion.R3;
        }

        public EntryComponentWrapper(R4Model.Bundle.EntryComponent entryComponent)
        {
            R4EntryComponent = entryComponent;
            FhirVersion = FhirVersion.R4;
        }


        private void SetRequest(RequestComponentWrapper value)
        {
            switch (FhirVersion)
            {
                case FhirVersion.R3:
                    R3EntryComponent.Request = value.R3Object;
                    break;
                case FhirVersion.R4:
                    R4EntryComponent.Request = value.R4Object;
                    break;
            }
        }

        private RequestComponentWrapper GetRequest()
        {
            switch (FhirVersion)
            {
                case FhirVersion.R3:
                    return new RequestComponentWrapper(R3EntryComponent.Request);
                case FhirVersion.R4:
                    return new RequestComponentWrapper(R4EntryComponent.Request);
                default:
                    return default;
            }
        }

    }
}