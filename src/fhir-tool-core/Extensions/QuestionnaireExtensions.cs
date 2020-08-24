using EnsureThat;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using System.Linq;

namespace FhirTool.Core
{
    internal static class QuestionnaireExtensions
    {
        public static bool IsItemControlOfType(this Questionnaire.ItemComponent item, params string[] itemControlType)
        {
            EnsureArg.IsNotNull(itemControlType, nameof(itemControlType));

            IEnumerable<Extension> extensions = item.GetExtensions(Constants.ItemControlUri);
            foreach (Extension extension in extensions)
            {
                if (!(extension.Value is CodeableConcept codeableConcept)) continue;
                foreach (Coding coding in codeableConcept.Coding)
                    if (itemControlType.Contains(coding.Code)) return true;
            }

            return false;
        }

        public static bool HasContainedValueSet(this Questionnaire questionnaire, ResourceReference reference)
        {
            EnsureArg.IsNotNull(reference, nameof(reference));

            string literal = reference.Reference.IndexOf("#") == 0 ? reference.Reference.Substring(1) : string.Empty;

            return questionnaire.Contained.Any(r => r.ResourceType == ResourceType.ValueSet && r.Id == literal);
        }

        public static IEnumerable<Issue> VerifyItemValidation(this Questionnaire questionnaire, Questionnaire.ItemComponent item)
        {
            EnsureArg.IsNotNull(item, nameof(item));

            var issues = new List<Issue>();

            issues.AddRange(VerifyStringAndTextValidation(item));
            issues.AddRange(VerifyRepeatingItemValidation(item));
            issues.AddRange(VerifyMaxOrMinValueIsSet(item));
            issues.AddRange(VerifyAttachmentValidation(item));
            issues.AddRange(VerifyOptionsReferenceExists(questionnaire, item));

            foreach (Questionnaire.ItemComponent itm in item.Item)
            {
                issues.AddRange(questionnaire.VerifyItemValidation(itm));
            }

            return issues;
        }

        private static IEnumerable<Issue> VerifyOptionsReferenceExists(Questionnaire questionnaire, Questionnaire.ItemComponent item)
        {
            if (item.Type != Questionnaire.QuestionnaireItemType.Choice && item.Type != Questionnaire.QuestionnaireItemType.OpenChoice)
                return new Issue[0];
            if(item.Options == null)
                return new Issue[0];

            var issues = new List<Issue>();

            if (!questionnaire.HasContainedValueSet(item.Options))
            {
                issues.Add(new Issue
                {
                    LinkId = item.LinkId,
                    Severity = IssueSeverityEnum.Error,
                    Details = $"Cannot find the reference to ValueSet {item.Options?.Reference}"
                });
            }

            return issues;
        }

        private static IEnumerable<Issue> VerifyAttachmentValidation(Questionnaire.ItemComponent item)
        {
            if (item.Type != Questionnaire.QuestionnaireItemType.Attachment) return new Issue[0];

            var issues = new List<Issue>();

            if (item.GetExtensions(Constants.QuestionnaireAttachmentMaxSize).Count() == 0)
            {
                issues.Add(new Issue
                {
                    LinkId = item.LinkId,
                    Severity = IssueSeverityEnum.Warning,
                    Details = $"An item of type '{item.Type}' is missing the 'maxSize' attribute. Consider setting the 'maxSize' attribute."
                });
            }

            return issues;
        }

        private static IEnumerable<Issue> VerifyMaxOrMinValueIsSet(Questionnaire.ItemComponent item)
        {
            if (!(item.Type == Questionnaire.QuestionnaireItemType.Date
                || item.Type == Questionnaire.QuestionnaireItemType.DateTime
                || item.Type == Questionnaire.QuestionnaireItemType.Time
                || item.Type == Questionnaire.QuestionnaireItemType.Integer
                || item.Type == Questionnaire.QuestionnaireItemType.Decimal))
            {
                return new Issue[0];
            }

            // If this is a read-only field and it contains a fhirpath for data extraction do not suggest any validation
            if (item.ReadOnly == true && item.GetExtensions(Constants.FhirPathUri).Any())
            {
                return new Issue[0];
            }

            var issues = new List<Issue>();
            if (item.GetExtensions(Constants.MaxValueUri).Count() == 0 || item.GetExtensions(Constants.MinValueUri).Count() == 0)
            {
                issues.Add(new Issue
                {
                    LinkId = item.LinkId,
                    Severity = IssueSeverityEnum.Information,
                    Details = $"Consider setting the 'maxValue' and 'minValue' attribute for items of type '{item.Type}'."
                });
            }

            return issues;
        }

        private static IEnumerable<Issue> VerifyRepeatingItemValidation(Questionnaire.ItemComponent item)
        {
            var issues = new List<Issue>();
            if (item.Repeats == true)
            {
                if (item.GetExtensions(Constants.MaxOccursUri).Count() == 0)
                {
                    issues.Add(new Issue
                    {
                        LinkId = item.LinkId,
                        Severity = IssueSeverityEnum.Warning,
                        Details = $"An item where the attribute 'repeats' is set to 'true' a 'maxOccurs' is recommended to be set."
                    });
                }
            }

            return issues;
        }

        private static IEnumerable<Issue> VerifyStringAndTextValidation(Questionnaire.ItemComponent item)
        {
            if ((item.Type != Questionnaire.QuestionnaireItemType.String && item.Type != Questionnaire.QuestionnaireItemType.Text)
                || !item.IsItemControlOfType("help", "help-link", "inline"))
            {
                return new Issue[0];
            }
            // If this is a read-only field and it contains a fhirpath for data extraction do not suggest any validation
            if(item.ReadOnly == true && item.GetExtensions(Constants.FhirPathUri).Any())
            {
                return new Issue[0];
            }

            var issues = new List<Issue>();
            // Issues an error if a maxLength has NOT been set
            if (!item.MaxLength.HasValue)
            {
                issues.Add(new Issue
                {
                    LinkId = item.LinkId,
                    Severity = IssueSeverityEnum.Error,
                    Details = $"An item of type '{item.Type}' must have the 'maxLength' attribute set."
                });
            }
            // Issues an error if a regex has NOT been set
            if (item.GetExtensions(Constants.RegexUri).Count() == 0)
            {
                issues.Add(new Issue
                {
                    LinkId = item.LinkId,
                    Severity = IssueSeverityEnum.Error,
                    Details = $"An item of type '{item.Type}' must have the 'regex' attribute set."
                });
            }
            if (item.Type == Questionnaire.QuestionnaireItemType.String)
            {
                // Issues an error for ItemType == 'string' if a maxLength of no more than 250 characters has been set.
                if (item.MaxLength.HasValue && item.MaxLength.Value > 250)
                {
                    issues.Add(new Issue
                    {
                        LinkId = item.LinkId,
                        Severity = IssueSeverityEnum.Error,
                        Details = $"An item of type '{item.Type}' must have a 'maxLength' of no more than 250 characters."
                    });
                }
            }
            if (item.Type == Questionnaire.QuestionnaireItemType.Text)
            {
                // Issues a warning for ItemType == 'text' if a maxLength of more than 2500 characters has been set.
                if (item.MaxLength.HasValue && item.MaxLength.Value > 2500)
                {
                    issues.Add(new Issue
                    {
                        LinkId = item.LinkId,
                        Severity = IssueSeverityEnum.Warning,
                        Details = $"Item field of '{item.Type}' has a 'maxLength' of more than 2500 characters. This is allowed, but not recommended."
                    });
                }
            }

            return issues;
        }
    }
}
