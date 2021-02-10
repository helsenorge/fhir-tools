/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using FhirTool.Core;
using System.Collections.Generic;

namespace FhirTool
{
    internal static class FhirVersionInternal
    {
        private const string R2_MONIKER = "R2";
        private const string R3_MONIKER = "R3";
        private const string R4_MONIKER = "R4";
        private const string R2_OFFICAL_MONIKER = "DSTU2";
        private const string R3_OFFICAL_MONIKER = "STU3";
        private const string R4_OFFICAL_MONIKER = "R4";

        public const string R2_VERSION = "1.0";
        public const string R3_VERSION = "3.0";
        public const string R4_VERSION = "4.0";

        public static string ConvertToFhirVersion(string moniker)
        {
            switch(moniker)
            {
                case R2_OFFICAL_MONIKER:
                case R2_MONIKER:
                    return R2_VERSION;
                case R3_OFFICAL_MONIKER:
                case R3_MONIKER:
                    return R3_VERSION;
                case R4_MONIKER: // Do not need R4_OFFICAL_MONIKER, both the offical and alternative moniker are the same.
                    return R4_VERSION;
                default:
                    throw new UnknownFhirMonikerException(moniker);
            }
        }

        // Currently only support for R3 and R4 versions
        public static IEnumerable<string> GetSupportedFhirVersions() => new[] { R3_VERSION, R4_VERSION };
    }
}
