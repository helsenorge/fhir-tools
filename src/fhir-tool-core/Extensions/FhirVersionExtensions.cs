/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using System.Linq;

namespace FhirTool.Core
{
    public static class FhirVersionExtensions
    {
        public static string GetFhirVersionAsString(this FhirVersion version)
        {
            return FhirConstants.KnownFhirVersions[version];
        }
    }
}
