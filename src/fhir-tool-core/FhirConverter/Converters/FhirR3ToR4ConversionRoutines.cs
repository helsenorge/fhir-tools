extern alias R3;
extern alias R4;

using System.Linq;
using System.Collections.Generic;
using Hl7.Fhir.Utility;

using TargetModel = R4::Hl7.Fhir.Model;
using SourceModel = R3::Hl7.Fhir.Model;
using System;

namespace FhirTool.Conversion.Converters
{
    internal partial class FhirR3ToR4ConversionRoutines : BaseConverter
    {
        public FhirR3ToR4ConversionRoutines() : base()
        {
            Map.Add<TargetModel.QuestionnaireResponse, SourceModel.QuestionnaireResponse>(ConvertQuestionnaireResponse);
            Map.Add<TargetModel.Questionnaire.ItemComponent, SourceModel.Questionnaire.ItemComponent>(ConvertQuestionnaireItemComponent);
            Map.Add<TargetModel.Questionnaire.EnableWhenComponent, SourceModel.Questionnaire.EnableWhenComponent>(ConvertQuestionnaireEnableWhenComponent);
            Map.Add<TargetModel.Money, SourceModel.Money>(ConvertMoney);
            Map.Add<TargetModel.Attachment, SourceModel.Attachment>(ConvertAttachment);
            Map.Add<TargetModel.RelatedArtifact, SourceModel.RelatedArtifact>(ConvertRelatedArtifact);
            Map.Add<TargetModel.DataRequirement, SourceModel.DataRequirement>(ConvertDataRequirement);
            Map.Add<TargetModel.DataRequirement.CodeFilterComponent, SourceModel.DataRequirement.CodeFilterComponent>(ConvertDataRequirementCodeFilterComponent);
            Map.Add<TargetModel.Meta, SourceModel.Meta>(ConvertMeta);
            Map.Add<TargetModel.ElementDefinition.TypeRefComponent, SourceModel.ElementDefinition.TypeRefComponent>(ConvertElementDefinitionTypeRefComponent);
            Map.Add<TargetModel.ElementDefinition.ConstraintComponent, SourceModel.ElementDefinition.ConstraintComponent>(ConvertElementDefinitionConstraintComponent);
            Map.Add<TargetModel.ElementDefinition.ElementDefinitionBindingComponent, SourceModel.ElementDefinition.ElementDefinitionBindingComponent>(ConvertElementDefinitionBindingComponent);
            Map.Add<TargetModel.ValueSet.ConceptSetComponent, SourceModel.ValueSet.ConceptSetComponent>(ConvertValueSetConceptSetComponent);
            Map.Add<TargetModel.ValueSet.FilterComponent, SourceModel.ValueSet.FilterComponent>(ConvertValueSetFilterComponent);
            Map.Add<TargetModel.Annotation, SourceModel.Annotation>(ConvertAnnotation);
            Map.Add<TargetModel.Timing.RepeatComponent, SourceModel.Timing.RepeatComponent>(ConvertTimingRepeatComponent);
            Map.Add<TargetModel.ParameterDefinition, SourceModel.ParameterDefinition>(ConvertParameterDefinition);
            Map.Add<TargetModel.Signature, SourceModel.Signature>(ConvertSignature);
            Map.Add<TargetModel.Range, SourceModel.Range>(ConvertRange);
            Map.Add<TargetModel.Dosage, SourceModel.Dosage>(ConvertDosage);
        }

        private static void ConvertQuestionnaireResponse(TargetModel.QuestionnaireResponse to, SourceModel.QuestionnaireResponse from, FhirConverter converter)
        {
            to.QuestionnaireElement = ConvertResourceReferenceToCanonical(from.Questionnaire, converter);
        }

        private static void ConvertQuestionnaireItemComponent(TargetModel.Questionnaire.ItemComponent to, SourceModel.Questionnaire.ItemComponent from, FhirConverter converter)
        {
            to.Initial = from.Initial == null ? to.Initial : new List<TargetModel.Questionnaire.InitialComponent> { ConvertElementToInitialComponent(from.Initial, converter) };
            to.AnswerValueSetElement = ConvertResourceReferenceToCanonical(from.Options, converter);
        }

        private static void ConvertQuestionnaireEnableWhenComponent(TargetModel.Questionnaire.EnableWhenComponent to, SourceModel.Questionnaire.EnableWhenComponent from, FhirConverter converter)
        {
            if (from.HasAnswer.HasValue)
            {
                to.Operator = TargetModel.Questionnaire.QuestionnaireItemOperator.Exists;
                to.Answer = new TargetModel.FhirBoolean(from.HasAnswer);
            }
            else if (from.Answer != null)
            {
                to.Operator = TargetModel.Questionnaire.QuestionnaireItemOperator.Equal;
            }
        }

        private static void ConvertMoney(TargetModel.Money to, SourceModel.Money from, FhirConverter converter)
        {
            if (from != null)
            {
                to.CurrencyElement = ConvertCodeToCodeMoney(from.CodeElement, converter);
                to.ValueElement = converter.ConvertElement<TargetModel.FhirDecimal, SourceModel.FhirDecimal>(from.ValueElement);
            }
        }

        private static void ConvertAttachment(TargetModel.Attachment to, SourceModel.Attachment from, FhirConverter converter)
        {
            to.UrlElement = ConvertFhirUriToFhirUrl(from.UrlElement, converter);
        }

        private static void ConvertRelatedArtifact(TargetModel.RelatedArtifact to, SourceModel.RelatedArtifact from, FhirConverter converter)
        {
            to.UrlElement = ConvertFhirUriToFhirUrl(from.UrlElement, converter);
            to.Citation = ConvertFhirStringToMarkdown(from.CitationElement, converter);
            to.ResourceElement = ConvertResourceReferenceToCanonical(from.Resource, converter);
        }

        private static void ConvertDataRequirement(TargetModel.DataRequirement to, SourceModel.DataRequirement from, FhirConverter converter)
        {
            to.ProfileElement = from.ProfileElement.Select(e => ConvertFhirUriToCanonical(e, converter)).ToList();
        }

        private static void ConvertDataRequirementCodeFilterComponent(TargetModel.DataRequirement.CodeFilterComponent to, SourceModel.DataRequirement.CodeFilterComponent from, FhirConverter converter)
        {
            if (from.ValueSet is SourceModel.ResourceReference fromResourceReference)
            {
                to.ValueSetElement = ConvertResourceReferenceToCanonical(fromResourceReference, converter);
            }
            else if (from.ValueSet is SourceModel.FhirString fromFhirString)
            {
                to.ValueSetElement = ConvertFhirStringToCanonical(fromFhirString, converter);
            }
        }

        private static void ConvertMeta(TargetModel.Meta to, SourceModel.Meta from, FhirConverter converter)
        {
            to.ProfileElement = from.ProfileElement.Select(e => ConvertFhirUriToCanonical(e, converter)).ToList();
        }

        private static void ConvertElementDefinitionTypeRefComponent(TargetModel.ElementDefinition.TypeRefComponent to, SourceModel.ElementDefinition.TypeRefComponent from, FhirConverter converter)
        {
            to.ProfileElement = from.ProfileElement == null ? to.ProfileElement : new List<TargetModel.Canonical> { ConvertFhirUriToCanonical(from.ProfileElement, converter) };
            to.TargetProfileElement = from.TargetProfileElement == null ? to.TargetProfileElement : new List<TargetModel.Canonical> { ConvertFhirUriToCanonical(from.TargetProfileElement, converter) };
        }

        private static void ConvertElementDefinitionConstraintComponent(TargetModel.ElementDefinition.ConstraintComponent to, SourceModel.ElementDefinition.ConstraintComponent from, FhirConverter converter)
        {
            to.SourceElement = ConvertFhirUriToCanonical(from.SourceElement, converter);
        }

        private static void ConvertElementDefinitionBindingComponent(TargetModel.ElementDefinition.ElementDefinitionBindingComponent to, SourceModel.ElementDefinition.ElementDefinitionBindingComponent from, FhirConverter converter)
        {
            if (from.ValueSet is SourceModel.FhirUri fromFhirUri)
            {
                to.ValueSetElement = ConvertFhirUriToCanonical(fromFhirUri, converter);
            }
            else if (from.ValueSet is SourceModel.ResourceReference fromResourceReference)
            {
                to.ValueSetElement = ConvertResourceReferenceToCanonical(fromResourceReference, converter);
            }
        }

        private static void ConvertValueSetConceptSetComponent(TargetModel.ValueSet.ConceptSetComponent to, SourceModel.ValueSet.ConceptSetComponent from, FhirConverter converter)
        {
            to.ValueSetElement = from.ValueSetElement.Select(e => ConvertFhirUriToCanonical(e, converter)).ToList();
        }

        private static void ConvertValueSetFilterComponent(TargetModel.ValueSet.FilterComponent to, SourceModel.ValueSet.FilterComponent from, FhirConverter converter)
        {
            to.ValueElement = ConvertCodeToFhirString(from.ValueElement, converter);
        }

        private static void ConvertAnnotation(TargetModel.Annotation to, SourceModel.Annotation from, FhirConverter converter)
        {
            to.Text = ConvertFhirStringToMarkdown(from.TextElement, converter);
        }

        private static void ConvertTimingRepeatComponent(TargetModel.Timing.RepeatComponent to, SourceModel.Timing.RepeatComponent from, FhirConverter converter)
        {
            to.CountElement = ConvertIntegerToPositiveInt(from.CountElement, converter);
            to.CountMaxElement = ConvertIntegerToPositiveInt(from.CountMaxElement, converter);
            to.FrequencyElement = ConvertIntegerToPositiveInt(from.FrequencyElement, converter);
            to.FrequencyMaxElement = ConvertIntegerToPositiveInt(from.FrequencyMaxElement, converter);
        }

        private static void ConvertParameterDefinition(TargetModel.ParameterDefinition to, SourceModel.ParameterDefinition from, FhirConverter converter)
        {
            to.ProfileElement = ConvertResourceReferenceToCanonical(from.Profile, converter);
        }

        private static void ConvertSignature(TargetModel.Signature to, SourceModel.Signature from, FhirConverter converter)
        {
            if (from.Who != null)
            {
                if (from.Who is SourceModel.ResourceReference fromResourceReference)
                {
                    to.Who = converter.ConvertElement<TargetModel.ResourceReference, SourceModel.ResourceReference>(fromResourceReference);
                }
                else if (from.Who is SourceModel.FhirUri fromFhirUri)
                {
                    to.Who = ConvertFhirUriToResourceReference(fromFhirUri, converter);
                }
            }

            if (from.OnBehalfOf != null)
            {
                if (from.OnBehalfOf is SourceModel.ResourceReference fromResourceReference)
                {
                    to.OnBehalfOf = converter.ConvertElement<TargetModel.ResourceReference, SourceModel.ResourceReference>(fromResourceReference);
                }
                else if (from.OnBehalfOf is SourceModel.FhirUri fromFhirUri)
                {
                    to.OnBehalfOf = ConvertFhirUriToResourceReference(fromFhirUri, converter);
                }
            }
        }

        private static void ConvertRange(TargetModel.Range to, SourceModel.Range from, FhirConverter converter)
        {
            to.High = ConvertQuantityToSimpleQuantity(from.High, converter);
            to.Low = ConvertQuantityToSimpleQuantity(from.Low, converter);
        }

        private static void ConvertDosage(TargetModel.Dosage to, SourceModel.Dosage from, FhirConverter converter)
        {
            if (from.Dose != null)
            {
                to.DoseAndRate.Add(new TargetModel.Dosage.DoseAndRateComponent { Dose = converter.ConvertElement<TargetModel.Element, SourceModel.Element>(from.Dose) });
            }
            if (from.Rate != null)
            {
                to.DoseAndRate.Add(new TargetModel.Dosage.DoseAndRateComponent { Rate = converter.ConvertElement<TargetModel.Element, SourceModel.Element>(from.Rate) });
            }
        }

        // Helpers
        private static TargetModel.Code<TargetModel.Money.Currencies> ConvertCodeToCodeMoney(SourceModel.Code from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.Code<TargetModel.Money.Currencies>
            {
                Value = string.IsNullOrWhiteSpace(from.Value) ? null : EnumUtility.ParseLiteral<TargetModel.Money.Currencies>(from.Value),
                Extension = converter.ConvertList<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.FhirUrl ConvertFhirUriToFhirUrl(SourceModel.FhirUri from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.FhirUrl
            {
                Value = from.Value,
                Extension = converter.ConvertList<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.Questionnaire.InitialComponent ConvertElementToInitialComponent(SourceModel.Element from, FhirConverter converter)
        {
            return new TargetModel.Questionnaire.InitialComponent
            {
                Value = converter.ConvertElement<TargetModel.Element, SourceModel.Element>(from),
            };
        }

        private static TargetModel.Canonical ConvertFhirUriToCanonical(SourceModel.FhirUri from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.Canonical
            {
                Value = from.Value,
                Extension = converter.ConvertList<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.Canonical ConvertResourceReferenceToCanonical(SourceModel.ResourceReference from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.Canonical
            {
                Value = from.Reference,
                Extension = converter.ConvertList<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.Canonical ConvertFhirStringToCanonical(SourceModel.FhirString from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.Canonical
            {
                Value = from.Value,
                Extension = converter.ConvertList<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.Markdown ConvertFhirStringToMarkdown(SourceModel.FhirString from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.Markdown
            {
                Value = from.Value,
                Extension = converter.ConvertList<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.FhirString ConvertCodeToFhirString(SourceModel.Code from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.FhirString
            {
                Value = from.Value,
                Extension = converter.ConvertList<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.ResourceReference ConvertFhirUriToResourceReference(SourceModel.FhirUri from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.ResourceReference
            {
                Reference = from.Value,
                Extension = converter.ConvertList<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.PositiveInt ConvertIntegerToPositiveInt(SourceModel.Integer from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.PositiveInt
            {
                Value = from.Value,
                Extension = converter.ConvertList<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.SimpleQuantity ConvertQuantityToSimpleQuantity(SourceModel.Quantity from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.SimpleQuantity
            {
                System = from.System,
                Code = from.Code,
                Value = from.Value,
                Unit = from.Unit,
                Extension = converter.ConvertList<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }
    }
}
