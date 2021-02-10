/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

namespace FhirTool.Core.Model
{
    public class UsageContextElement
    {
        public CodingElement Code { get; set; }
        public CodeableConceptElement ValueCodeableConcept { get; set; }
        public QuantityElement ValueQuantity { get; set; }
        public ReferenceElement ValueReference { get; set; }
    }
}
