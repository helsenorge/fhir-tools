using System.Collections.Generic;

namespace FhirTool.Model
{
    public class CodeableConceptElement
    {
        public List<CodingElement> Coding { get; set; }
        public string Text { get; set; }
    }
}
