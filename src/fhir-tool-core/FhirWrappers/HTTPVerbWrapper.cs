/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

extern alias R3;
extern alias R4;

using System.Linq;
using System.Collections.Generic;
using R3Model = R3::Hl7.Fhir.Model;
using R4Model = R4::Hl7.Fhir.Model;

namespace FhirTool.Core.FhirWrappers
{
    public static class HTTPVerbWrapperExtensions
    {
        private static Dictionary<HTTPVerbWrapper, R3Model.Bundle.HTTPVerb> R3Map =
            new Dictionary<HTTPVerbWrapper, R3Model.Bundle.HTTPVerb>
            {
                { HTTPVerbWrapper.GET, R3Model.Bundle.HTTPVerb.GET },
                { HTTPVerbWrapper.POST, R3Model.Bundle.HTTPVerb.POST },
                { HTTPVerbWrapper.PUT, R3Model.Bundle.HTTPVerb.PUT },
                { HTTPVerbWrapper.DELETE, R3Model.Bundle.HTTPVerb.DELETE }
            };
        private static Dictionary<HTTPVerbWrapper, R4Model.Bundle.HTTPVerb> R4Map =
            new Dictionary<HTTPVerbWrapper, R4Model.Bundle.HTTPVerb>
            {
                { HTTPVerbWrapper.GET, R4Model.Bundle.HTTPVerb.GET },
                { HTTPVerbWrapper.POST, R4Model.Bundle.HTTPVerb.POST },
                { HTTPVerbWrapper.PUT, R4Model.Bundle.HTTPVerb.PUT },
                { HTTPVerbWrapper.DELETE, R4Model.Bundle.HTTPVerb.DELETE }
            };

        public static R3Model.Bundle.HTTPVerb ToR3(this HTTPVerbWrapper me)
        {
            return R3Map[me];
        }

        public static R4Model.Bundle.HTTPVerb ToR4(this HTTPVerbWrapper me)
        {
            return R4Map[me];
        }

        public static HTTPVerbWrapper Wrap(this R3Model.Bundle.HTTPVerb? me)
        {
            return me.HasValue ? R3Map.ToDictionary(keySelector => keySelector.Value, valueSelector => valueSelector.Key)[me.Value] : default ;
        }

        public static HTTPVerbWrapper Wrap(this R4Model.Bundle.HTTPVerb? me)
        {
            return me.HasValue ? R4Map.ToDictionary(keySelector => keySelector.Value, valueSelector => valueSelector.Key)[me.Value] : default;
        }
    }

    public enum HTTPVerbWrapper
    {
        GET = 0,
        POST = 1,
        PUT = 2,
        DELETE = 3
    }
}