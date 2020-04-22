using Hl7.Fhir.Model;
using System.Collections.Generic;
using System.Linq;

namespace FhirTool.Extensions
{
    internal static class ItemComponentExtensions
    {
        public static bool IsItemControlOfType(this Questionnaire.ItemComponent item, params string[] itemControlType)
        {
            IEnumerable<Extension> extensions = item.GetExtensions(Constants.ItemControlUri);
            foreach (Extension extension in extensions)
            {
                if (!(extension.Value is CodeableConcept codeableConcept)) continue;
                foreach (Coding coding in codeableConcept.Coding)
                    if (itemControlType.Contains(coding.Code)) return true;
            }

            return false;
        }
    }
}
