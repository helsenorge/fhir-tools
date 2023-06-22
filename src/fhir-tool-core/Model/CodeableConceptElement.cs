/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using System.Collections.Generic;

namespace FhirTool.Core.Model
{
    public class CodeableConceptElement
    {
        public List<CodingElement> Coding { get; set; }
        public string Text { get; set; }
    }
}
