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

        private static void ConvertQuestionnaireResponse(TargetModel.QuestionnaireResponse to, SourceModel.QuestionnaireResponse from, FhirConverter converter)
        {
            to.Questionnaire = new TargetModel.ResourceReference(from.Questionnaire);
        }

        private static void ConvertQuestionnaireItemComponent(TargetModel.Questionnaire.ItemComponent to, SourceModel.Questionnaire.ItemComponent from, FhirConverter converter)
        {
            to.Initial = converter.ConvertObject<TargetModel.Element, SourceModel.Element>(from.Initial.FirstOrDefault()?.Value);
            to.Options = ConvertCanonicalToResourceReference(from.AnswerValueSetElement, converter);
        }

        private static void ConvertQuestionnaireEnableWhenComponent(TargetModel.Questionnaire.EnableWhenComponent to, SourceModel.Questionnaire.EnableWhenComponent from, FhirConverter converter)
        {
            if (from.Operator == SourceModel.Questionnaire.QuestionnaireItemOperator.Exists && from.Answer is SourceModel.FhirBoolean answer)
            {
                to.HasAnswer = answer.Value;
            }
        }

        private static void ConvertMoney(TargetModel.Money to, SourceModel.Money from, FhirConverter converter)
        {
            if (from.Currency.HasValue)
            {
                to.System = "urn:iso:std:iso:4217";
                to.CodeElement = new TargetModel.Code
                {
                    Value = from.Currency.Value.GetLiteral(),
                    Extension = converter.ConvertList<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
                };
            }
        }

        private static void ConvertAttachment(TargetModel.Attachment to, SourceModel.Attachment from, FhirConverter converter)
        {
            to.UrlElement = ConvertFhirUrlToFhirUri(from.UrlElement, converter);
        }

        private static void ConvertRelatedArtifact(TargetModel.RelatedArtifact to, SourceModel.RelatedArtifact from, FhirConverter converter)
        {
            to.UrlElement = ConvertFhirUrlToFhirUri(from.UrlElement, converter);
            to.CitationElement = ConvertMarkdownToFhirString(from.Citation, converter);
            to.Resource = ConvertCanonicalToResourceReference(from.ResourceElement, converter);
        }

        private static void ConvertDataRequirement(TargetModel.DataRequirement to, SourceModel.DataRequirement from, FhirConverter converter)
        {
            to.ProfileElement = from.ProfileElement.Select(e => ConvertCanonicalToFhirUri(e, converter)).ToList();
        }

        private static void ConvertDataRequirementCodeFilterComponent(TargetModel.DataRequirement.CodeFilterComponent to, SourceModel.DataRequirement.CodeFilterComponent from, FhirConverter converter)
        {
            to.ValueSet = ConvertCanonicalToResourceReference(from.ValueSetElement, converter);
        }

        private static void ConvertMeta(TargetModel.Meta to, SourceModel.Meta from, FhirConverter converter)
        {
            to.ProfileElement = from.ProfileElement.Select(e => ConvertCanonicalToFhirUri(e, converter)).ToList();
        }

        private static void ConvertElementDefinitionTypeRefComponent(TargetModel.ElementDefinition.TypeRefComponent to, SourceModel.ElementDefinition.TypeRefComponent from, FhirConverter converter)
        {
            to.ProfileElement = ConvertCanonicalToFhirUri(from.ProfileElement.FirstOrDefault(), converter);
            to.TargetProfileElement = ConvertCanonicalToFhirUri(from.TargetProfileElement.FirstOrDefault(), converter);
        }

        private static void ConvertElementDefinitionConstraintComponent(TargetModel.ElementDefinition.ConstraintComponent to, SourceModel.ElementDefinition.ConstraintComponent from, FhirConverter converter)
        {
            to.SourceElement = ConvertCanonicalToFhirUri(from.SourceElement, converter);
        }

        private static void ConvertElementDefinitionBindingComponent(TargetModel.ElementDefinition.ElementDefinitionBindingComponent to, SourceModel.ElementDefinition.ElementDefinitionBindingComponent from, FhirConverter converter)
        {
            to.ValueSet = ConvertCanonicalToResourceReference(from.ValueSetElement, converter);
        }

        private static void ConvertValueSetConceptSetComponent(TargetModel.ValueSet.ConceptSetComponent to, SourceModel.ValueSet.ConceptSetComponent from, FhirConverter converter)
        {
            to.ValueSetElement = from.ValueSetElement.Select(e => ConvertCanonicalToFhirUri(e, converter)).ToList();
        }

        private static void ConvertValueSetFilterComponent(TargetModel.ValueSet.FilterComponent to, SourceModel.ValueSet.FilterComponent from, FhirConverter converter)
        {
            to.ValueElement = ConvertFhirStringToCode(from.ValueElement, converter);
        }

        private static void ConvertAnnotation(TargetModel.Annotation to, SourceModel.Annotation from, FhirConverter converter)
        {
            to.TextElement = ConvertMarkdownToFhirString(from.Text, converter);
        }

        private static void ConvertTimingRepeatComponent(TargetModel.Timing.RepeatComponent to, SourceModel.Timing.RepeatComponent from, FhirConverter converter)
        {
            to.CountElement = ConvertPositiveIntToInteger(from.CountElement, converter);
            to.CountMaxElement = ConvertPositiveIntToInteger(from.CountMaxElement, converter);
            to.FrequencyElement = ConvertPositiveIntToInteger(from.FrequencyElement, converter);
            to.FrequencyMaxElement = ConvertPositiveIntToInteger(from.FrequencyMaxElement, converter);
        }

        private static void ConvertParameterDefinition(TargetModel.ParameterDefinition to, SourceModel.ParameterDefinition from, FhirConverter converter)
        {
            to.Profile = ConvertCanonicalToResourceReference(from.ProfileElement, converter);
        }

        private static void ConvertDosage(TargetModel.Dosage to, SourceModel.Dosage from, FhirConverter converter)
        {
            var fromDose = from.DoseAndRate.FirstOrDefault(it => it.Dose != null);
            var fromRate = from.DoseAndRate.FirstOrDefault(it => it.Rate != null);

            to.Dose = fromDose == null ? to.Dose : converter.ConvertObject<TargetModel.Element, SourceModel.Element>(fromDose);
            to.Rate = fromRate == null ? to.Rate : converter.ConvertObject<TargetModel.Element, SourceModel.Element>(fromRate);
        }

        // Helpers
        private static TargetModel.Code ConvertFhirStringToCode(SourceModel.FhirString from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.Code
            {
                Value = from.Value,
                Extension = converter.ConvertList<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.FhirUri ConvertCanonicalToFhirUri(SourceModel.Canonical from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.FhirUri
            {
                Value = from.Value,
                Extension = converter.ConvertList<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.FhirUri ConvertFhirUrlToFhirUri(SourceModel.FhirUrl from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.FhirUri
            {
                Value = from.Value,
                Extension = converter.ConvertList<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.FhirString ConvertMarkdownToFhirString(SourceModel.Markdown from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.FhirString
            {
                Value = from.Value,
                Extension = converter.ConvertList<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.Integer ConvertPositiveIntToInteger(SourceModel.PositiveInt from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.Integer
            {
                Value = from.Value,
                Extension = converter.ConvertList<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.ResourceReference ConvertCanonicalToResourceReference(SourceModel.Canonical from, FhirConverter converter)
        {
            if (from == null) return default;

            return new TargetModel.ResourceReference
            {
                Reference = from.Value,
                Extension = converter.ConvertList<TargetModel.Extension, SourceModel.Extension>(from.Extension).ToList()
            };
        }
    }
}
