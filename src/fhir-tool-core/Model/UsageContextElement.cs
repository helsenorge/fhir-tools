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
