/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

extern alias R3;
extern alias R4;

using FhirTool.Core;

namespace FhirTool.Conversion
{
    public class FhirConversionInfo
    {
        public FhirVersion FromVersion { get; set; }
        public FhirVersion ToVersion { get; set; }
    }
}
