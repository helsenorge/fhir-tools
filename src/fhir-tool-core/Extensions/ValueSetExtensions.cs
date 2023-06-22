/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

extern alias R3;
extern alias R4;

using R3::Hl7.Fhir.Model;
using R4Model = R4::Hl7.Fhir.Model;
using System.Text;
using Hl7.Fhir.Model;

namespace FhirTool.Core
{
    public static class ValueSetExtensions
    {
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

        public static Narrative GenerateNarrative(this R4Model.ValueSet valueSet)
        {
            StringBuilder div = new StringBuilder();
            div.Append("<div xmlns=\"http://www.w3.org/1999/xhtml\">");
            div.Append($"<p>Value set \"{valueSet.Id}: {valueSet.Name}\"</p>");
            div.Append($"<p>Developed by: {valueSet.Publisher}</p>");
            div.Append($"<p>Published for testing purposes on {valueSet.Date}</p>");
            div.Append($"<p>This value set contains the following codes from code system: {valueSet.Id}</p>");
            div.Append("<ol>");
            foreach (var conceptSet in valueSet.Compose.Include)
            {
                foreach (var concept in conceptSet.Concept)
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

        public static void GenerateAndSetNarrative(this R4Model.ValueSet valueSet)
        {
            StringBuilder div = new StringBuilder();
            div.Append("<div xmlns=\"http://www.w3.org/1999/xhtml\">");
            div.Append($"<p>Value set \"{valueSet.Id}: {valueSet.Name}\"</p>");
            div.Append($"<p>Developed by: {valueSet.Publisher}</p>");
            div.Append($"<p>Published for testing purposes on {valueSet.Date}</p>");
            div.Append($"<p>This value set contains the following codes from code system: {valueSet.Id}</p>");
            div.Append("<ol>");
            foreach (var conceptSet in valueSet.Compose.Include)
            {
                foreach (var concept in conceptSet.Concept)
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
