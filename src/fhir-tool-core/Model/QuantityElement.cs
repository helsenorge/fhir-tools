/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

namespace FhirTool.Core.Model
{
    public class QuantityElement
    {
        public decimal? Value { get; set; }
        public string Unit { get; set; }
        public string System { get; set; }
        public string Code { get; set; }
    }
}