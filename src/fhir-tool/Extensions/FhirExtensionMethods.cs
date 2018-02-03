using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Xml;

namespace FhirTool.Extensions
{
    public static class FhirExtensionMethods
    {
        public static void SerializeResourceToDiskAsXml(this Resource resource, string path)
        {
            using (XmlWriter writer = new XmlTextWriter(new StreamWriter(path)))
            {
                FhirSerializer.SerializeResource(resource, writer);
            }
        }

        public static void SerializeResourceToDiskAsJson(this Resource resource, string path)
        {
            using (JsonWriter writer = new JsonTextWriter(new StreamWriter(path)))
            {
                FhirSerializer.SerializeResource(resource, writer);
            }
        }

        public static Narrative GenerateNarrative(this ValueSet valueSet)
        {
            StringBuilder div = new StringBuilder();
            div.Append("<div xmlns=\"http://www.w3.org/1999/xhtml\">");
            div.Append($"<p>Value set \"{valueSet.Id}: {valueSet.Name}\"</p>");
            div.Append($"<p>Developed by: {valueSet.Publisher}</p>");
            div.Append($"<p>Published for testing purposes on {valueSet.Date}</p>");
            div.Append($"<p>This value set contains the following codes from code system: {valueSet.Id}</p>");
            div.Append("<ol>");
            foreach (ValueSet.ConceptSetComponent conceptSet in valueSet.Compose.Include)
            {
                foreach (ValueSet.ConceptReferenceComponent concept in conceptSet.Concept)
                {
                    div.Append($"<li>{concept.Code}: {concept.Display}</li>");
                }
            }
            div.Append("</ol>");
            div.Append("</div>");

            return new Narrative
            {
                Status = Narrative.NarrativeStatus.Generated,
                Div = div.ToString()
            };
        }

        public static void GenerateAndSetNarrative(this ValueSet valueSet)
        {
            StringBuilder div = new StringBuilder();
            div.Append("<div xmlns=\"http://www.w3.org/1999/xhtml\">");
            div.Append($"<p>Value set \"{valueSet.Id}: {valueSet.Name}\"</p>");
            div.Append($"<p>Developed by: {valueSet.Publisher}</p>");
            div.Append($"<p>Published for testing purposes on {valueSet.Date}</p>");
            div.Append($"<p>This value set contains the following codes from code system: {valueSet.Id}</p>");
            div.Append("<ol>");
            foreach (ValueSet.ConceptSetComponent conceptSet in valueSet.Compose.Include)
            {
                foreach (ValueSet.ConceptReferenceComponent concept in conceptSet.Concept)
                {
                    div.Append($"<li>{concept.Code}: {concept.Display}</li>");
                }
            }
            div.Append("</ol>");
            div.Append("</div>");

            valueSet.Text = GenerateNarrative(valueSet);
        }
    }
}
