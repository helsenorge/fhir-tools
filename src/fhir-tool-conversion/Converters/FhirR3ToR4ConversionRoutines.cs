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
        }

        private static void ConvertQuestionnaireResponse(FhirConverter converter, TargetModel.QuestionnaireResponse to, SourceModel.QuestionnaireResponse from)
        {
            to.QuestionnaireElement = ConvertResourceReferenceToCanonical(from.Questionnaire, converter);
        }

        private static void ConvertQuestionnaireItemComponent(FhirConverter converter, TargetModel.Questionnaire.ItemComponent to, SourceModel.Questionnaire.ItemComponent from)
        {
            to.Initial = from.Initial == null ? to.Initial : new List<TargetModel.Questionnaire.InitialComponent> { ConvertElementToInitialComponent(from.Initial, converter) };
            to.AnswerValueSetElement = ConvertResourceReferenceToCanonical(from.Options, converter);
        }

        private static void ConvertQuestionnaireEnableWhenComponent(FhirConverter converter, TargetModel.Questionnaire.EnableWhenComponent to, SourceModel.Questionnaire.EnableWhenComponent from)
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

        private static void ConvertMoney(FhirConverter converter, TargetModel.Money to, SourceModel.Money from)
        {
            if (from != null)
            {
                to.CurrencyElement = ConvertCodeToCodeMoney(from.CodeElement, converter);
                to.ValueElement = converter.Convert<TargetModel.FhirDecimal, SourceModel.FhirDecimal>(from.ValueElement);
            }
        }

        private static void ConvertAttachment(FhirConverter converter, TargetModel.Attachment to, SourceModel.Attachment from)
        {
            to.UrlElement = ConvertFhirUriToFhirUrl(from.UrlElement, converter);
        }

        private static void ConvertRelatedArtifact(FhirConverter converter, TargetModel.RelatedArtifact to, SourceModel.RelatedArtifact from)
        {
            to.UrlElement = ConvertFhirUriToFhirUrl(from.UrlElement, converter);
            to.Citation = ConvertFhirStringToMarkdown(from.CitationElement, converter);
            to.ResourceElement = ConvertResourceReferenceToCanonical(from.Resource, converter);
        }

        private static void ConvertDataRequirement(FhirConverter converter, TargetModel.DataRequirement to, SourceModel.DataRequirement from)
        {
            to.ProfileElement = from.ProfileElement.Select(e => ConvertFhirUriToCanonical(e, converter)).ToList();
        }

        private static void ConvertDataRequirementCodeFilterComponent(FhirConverter converter, TargetModel.DataRequirement.CodeFilterComponent to, SourceModel.DataRequirement.CodeFilterComponent from)
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

        private static void ConvertMeta(FhirConverter converter, TargetModel.Meta to, SourceModel.Meta from)
        {
            to.ProfileElement = from.ProfileElement.Select(e => ConvertFhirUriToCanonical(e, converter)).ToList();
        }

        private static void ConvertElementDefinitionTypeRefComponent(FhirConverter converter, TargetModel.ElementDefinition.TypeRefComponent to, SourceModel.ElementDefinition.TypeRefComponent from)
        {
            to.ProfileElement = from.ProfileElement == null ? to.ProfileElement : new List<TargetModel.Canonical> { ConvertFhirUriToCanonical(from.ProfileElement, converter) };
            to.TargetProfileElement = from.TargetProfileElement == null ? to.TargetProfileElement : new List<TargetModel.Canonical> { ConvertFhirUriToCanonical(from.TargetProfileElement, converter) };
        }

        private static void ConvertElementDefinitionConstraintComponent(FhirConverter converter, TargetModel.ElementDefinition.ConstraintComponent to, SourceModel.ElementDefinition.ConstraintComponent from)
        {
            to.SourceElement = ConvertFhirUriToCanonical(from.SourceElement, converter);
        }

        private static void ConvertElementDefinitionBindingComponent(FhirConverter converter, TargetModel.ElementDefinition.ElementDefinitionBindingComponent to, SourceModel.ElementDefinition.ElementDefinitionBindingComponent from)
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

        private static void ConvertValueSetConceptSetComponent(FhirConverter converter, TargetModel.ValueSet.ConceptSetComponent to, SourceModel.ValueSet.ConceptSetComponent from)
        {
            to.ValueSetElement = from.ValueSetElement.Select(e => ConvertFhirUriToCanonical(e, converter)).ToList();
        }

        private static void ConvertValueSetFilterComponent(FhirConverter converter, TargetModel.ValueSet.FilterComponent to, SourceModel.ValueSet.FilterComponent from)
        {
            to.ValueElement = ConvertCodeToFhirString(from.ValueElement, converter);
        }

        private static void ConvertAnnotation(FhirConverter converter, TargetModel.Annotation to, SourceModel.Annotation from)
        {
            to.Text = ConvertFhirStringToMarkdown(from.TextElement, converter);
        }

        private static void ConvertTimingRepeatComponent(FhirConverter converter, TargetModel.Timing.RepeatComponent to, SourceModel.Timing.RepeatComponent from)
        {
            to.CountElement = ConvertIntegerToPositiveInt(from.CountElement, converter);
            to.CountMaxElement = ConvertIntegerToPositiveInt(from.CountMaxElement, converter);
            to.FrequencyElement = ConvertIntegerToPositiveInt(from.FrequencyElement, converter);
            to.FrequencyMaxElement = ConvertIntegerToPositiveInt(from.FrequencyMaxElement, converter);
        }

        private static void ConvertParameterDefinition(FhirConverter converter, TargetModel.ParameterDefinition to, SourceModel.ParameterDefinition from)
        {
            to.ProfileElement = ConvertResourceReferenceToCanonical(from.Profile, converter);
        }

        private static void ConvertSignature(FhirConverter converter, TargetModel.Signature to, SourceModel.Signature from)
        {
            if (from.Who != null)
            {
                if (from.Who is SourceModel.ResourceReference fromResourceReference)
                {
                    to.Who = converter.Convert<TargetModel.ResourceReference, SourceModel.ResourceReference>(fromResourceReference);
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
                    to.OnBehalfOf = converter.Convert<TargetModel.ResourceReference, SourceModel.ResourceReference>(fromResourceReference);
                }
                else if (from.OnBehalfOf is SourceModel.FhirUri fromFhirUri)
                {
                    to.OnBehalfOf = ConvertFhirUriToResourceReference(fromFhirUri, converter);
                }
            }
        }

        private static void ConvertRange(FhirConverter converter, TargetModel.Range to, SourceModel.Range from)
        {
            to.High = ConvertQuantityToSimpleQuantity(from.High, converter);
            to.Low = ConvertQuantityToSimpleQuantity(from.Low, converter);
        }

        // Helpers
        private static TargetModel.Code<TargetModel.Money.Currencies> ConvertCodeToCodeMoney(SourceModel.Code from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.Code<TargetModel.Money.Currencies>
            {
                Value = string.IsNullOrEmpty(from.Value) ? null : EnumUtility.ParseLiteral<TargetModel.Money.Currencies>(from.Value),
                Extension = converter.Convert<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.FhirUrl ConvertFhirUriToFhirUrl(SourceModel.FhirUri from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.FhirUrl
            {
                Value = from.Value,
                Extension = converter.Convert<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.Questionnaire.InitialComponent ConvertElementToInitialComponent(SourceModel.Element from, FhirConverter converter)
        {
            return new TargetModel.Questionnaire.InitialComponent
            {
                Value = converter.Convert<TargetModel.Element, SourceModel.Element>(from),
            };
        }

        private static TargetModel.Canonical ConvertFhirUriToCanonical(SourceModel.FhirUri from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.Canonical
            {
                Value = from.Value,
                Extension = converter.Convert<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.Canonical ConvertResourceReferenceToCanonical(SourceModel.ResourceReference from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.Canonical
            {
                Value = from.Reference,
                Extension = converter.Convert<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.Canonical ConvertFhirStringToCanonical(SourceModel.FhirString from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.Canonical
            {
                Value = from.Value,
                Extension = converter.Convert<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.Markdown ConvertFhirStringToMarkdown(SourceModel.FhirString from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.Markdown
            {
                Value = from.Value,
                Extension = converter.Convert<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.FhirString ConvertCodeToFhirString(SourceModel.Code from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.FhirString
            {
                Value = from.Value,
                Extension = converter.Convert<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.ResourceReference ConvertFhirUriToResourceReference(SourceModel.FhirUri from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.ResourceReference
            {
                Reference = from.Value,
                Extension = converter.Convert<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.PositiveInt ConvertIntegerToPositiveInt(SourceModel.Integer from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.PositiveInt
            {
                Value = from.Value,
                Extension = converter.Convert<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
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
                Extension = converter.Convert<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }
    }
}
