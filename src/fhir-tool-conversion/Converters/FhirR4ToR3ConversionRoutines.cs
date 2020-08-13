extern alias R3;
extern alias R4;

using System;
using System.Linq;
using Hl7.Fhir.Utility;

using TargetModel = R3::Hl7.Fhir.Model;
using SourceModel = R4::Hl7.Fhir.Model;

namespace FhirTool.Conversion.Converters
{
    internal partial class FhirR4ToR3ConversionRoutines : BaseConverter
    {
        public FhirR4ToR3ConversionRoutines() : base()
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
            Map.Add<TargetModel.Dosage, SourceModel.Dosage>(ConvertDosage);
        }

        private static void ConvertQuestionnaireResponse(FhirConverter converter, TargetModel.QuestionnaireResponse to, SourceModel.QuestionnaireResponse from)
        {
            to.Questionnaire = new TargetModel.ResourceReference(from.Questionnaire);
        }

        private static void ConvertQuestionnaireItemComponent(FhirConverter converter, TargetModel.Questionnaire.ItemComponent to, SourceModel.Questionnaire.ItemComponent from)
        {
            to.Initial = converter.Convert<TargetModel.Element, SourceModel.Element>(from.Initial.FirstOrDefault()?.Value);
            to.Options = ConvertCanonicalToResourceReference(from.AnswerValueSetElement, converter);
        }

        private static void ConvertQuestionnaireEnableWhenComponent(FhirConverter converter, TargetModel.Questionnaire.EnableWhenComponent to, SourceModel.Questionnaire.EnableWhenComponent from)
        {
            if (from.Operator == SourceModel.Questionnaire.QuestionnaireItemOperator.Exists && from.Answer is SourceModel.FhirBoolean answer)
            {
                to.HasAnswer = answer.Value;
            }
        }

        private static void ConvertMoney(FhirConverter converter, TargetModel.Money to, SourceModel.Money from)
        {
            if (from.Currency.HasValue)
            {
                to.System = "urn:iso:std:iso:4217";
                to.CodeElement = new TargetModel.Code
                {
                    Value = from.Currency.Value.GetLiteral(),
                    Extension = converter.Convert<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
                };
            }
        }

        private static void ConvertAttachment(FhirConverter converter, TargetModel.Attachment to, SourceModel.Attachment from)
        {
            to.UrlElement = ConvertFhirUrlToFhirUri(from.UrlElement, converter);
        }

        private static void ConvertRelatedArtifact(FhirConverter converter, TargetModel.RelatedArtifact to, SourceModel.RelatedArtifact from)
        {
            to.UrlElement = ConvertFhirUrlToFhirUri(from.UrlElement, converter);
            to.CitationElement = ConvertMarkdownToFhirString(from.Citation, converter);
            to.Resource = ConvertCanonicalToResourceReference(from.ResourceElement, converter);
        }

        private static void ConvertDataRequirement(FhirConverter converter, TargetModel.DataRequirement to, SourceModel.DataRequirement from)
        {
            to.ProfileElement = from.ProfileElement.Select(e => ConvertCanonicalToFhirUri(e, converter)).ToList();
        }

        private static void ConvertDataRequirementCodeFilterComponent(FhirConverter converter, TargetModel.DataRequirement.CodeFilterComponent to, SourceModel.DataRequirement.CodeFilterComponent from)
        {
            to.ValueSet = ConvertCanonicalToResourceReference(from.ValueSetElement, converter);
        }

        private static void ConvertMeta(FhirConverter converter, TargetModel.Meta to, SourceModel.Meta from)
        {
            to.ProfileElement = from.ProfileElement.Select(e => ConvertCanonicalToFhirUri(e, converter)).ToList();
        }

        private static void ConvertElementDefinitionTypeRefComponent(FhirConverter converter, TargetModel.ElementDefinition.TypeRefComponent to, SourceModel.ElementDefinition.TypeRefComponent from)
        {
            to.ProfileElement = ConvertCanonicalToFhirUri(from.ProfileElement.FirstOrDefault(), converter);
            to.TargetProfileElement = ConvertCanonicalToFhirUri(from.TargetProfileElement.FirstOrDefault(), converter);
        }

        private static void ConvertElementDefinitionConstraintComponent(FhirConverter converter, TargetModel.ElementDefinition.ConstraintComponent to, SourceModel.ElementDefinition.ConstraintComponent from)
        {
            to.SourceElement = ConvertCanonicalToFhirUri(from.SourceElement, converter);
        }

        private static void ConvertElementDefinitionBindingComponent(FhirConverter converter, TargetModel.ElementDefinition.ElementDefinitionBindingComponent to, SourceModel.ElementDefinition.ElementDefinitionBindingComponent from)
        {
            to.ValueSet = ConvertCanonicalToResourceReference(from.ValueSetElement, converter);
        }

        private static void ConvertValueSetConceptSetComponent(FhirConverter converter, TargetModel.ValueSet.ConceptSetComponent to, SourceModel.ValueSet.ConceptSetComponent from)
        {
            to.ValueSetElement = from.ValueSetElement.Select(e => ConvertCanonicalToFhirUri(e, converter)).ToList();
        }

        private static void ConvertValueSetFilterComponent(FhirConverter converter, TargetModel.ValueSet.FilterComponent to, SourceModel.ValueSet.FilterComponent from)
        {
            to.ValueElement = ConvertFhirStringToCode(from.ValueElement, converter);
        }

        private static void ConvertAnnotation(FhirConverter converter, TargetModel.Annotation to, SourceModel.Annotation from)
        {
            to.TextElement = ConvertMarkdownToFhirString(from.Text, converter);
        }

        private static void ConvertTimingRepeatComponent(FhirConverter converter, TargetModel.Timing.RepeatComponent to, SourceModel.Timing.RepeatComponent from)
        {
            to.CountElement = ConvertPositiveIntToInteger(from.CountElement, converter);
            to.CountMaxElement = ConvertPositiveIntToInteger(from.CountMaxElement, converter);
            to.FrequencyElement = ConvertPositiveIntToInteger(from.FrequencyElement, converter);
            to.FrequencyMaxElement = ConvertPositiveIntToInteger(from.FrequencyMaxElement, converter);
        }

        private static void ConvertParameterDefinition(FhirConverter converter, TargetModel.ParameterDefinition to, SourceModel.ParameterDefinition from)
        {
            to.Profile = ConvertCanonicalToResourceReference(from.ProfileElement, converter);
        }

        private static void ConvertDosage(FhirConverter converter, TargetModel.Dosage to, SourceModel.Dosage from)
        {
            var fromDose = from.DoseAndRate.FirstOrDefault(it => it.Dose != null);
            var fromRate = from.DoseAndRate.FirstOrDefault(it => it.Rate != null);

            to.Dose = fromDose == null ? to.Dose : converter.Convert<TargetModel.Element, SourceModel.Element>(fromDose);
            to.Rate = fromRate == null ? to.Rate : converter.Convert<TargetModel.Element, SourceModel.Element>(fromRate);
        }

        // Helpers
        private static TargetModel.Code ConvertFhirStringToCode(SourceModel.FhirString from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.Code
            {
                Value = from.Value,
                Extension = converter.Convert<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.FhirUri ConvertCanonicalToFhirUri(SourceModel.Canonical from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.FhirUri
            {
                Value = from.Value,
                Extension = converter.Convert<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.FhirUri ConvertFhirUrlToFhirUri(SourceModel.FhirUrl from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.FhirUri
            {
                Value = from.Value,
                Extension = converter.Convert<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.FhirString ConvertMarkdownToFhirString(SourceModel.Markdown from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.FhirString
            {
                Value = from.Value,
                Extension = converter.Convert<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.Integer ConvertPositiveIntToInteger(SourceModel.PositiveInt from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.Integer
            {
                Value = from.Value,
                Extension = converter.Convert<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.ResourceReference ConvertCanonicalToResourceReference(SourceModel.Canonical from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.ResourceReference
            {
                Reference = from.Value,
                Extension = converter.Convert<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }
    }
}
