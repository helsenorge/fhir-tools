/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using FhirTool.Core.FhirWrappers;
using Hl7.Fhir.Utility;

namespace FhirTool.Core.Utils
{
    public static class MimeTypeUtils
    {
        public static FhirMimeType? ProbeFhirMimeType(string content)
        {
            if (SerializationUtil.ProbeIsJson(content))
            {
                return FhirMimeType.Json;
            }
            if (SerializationUtil.ProbeIsXml(content))
            {
                return FhirMimeType.Xml;
            }

            return null;
        }
    }
}
