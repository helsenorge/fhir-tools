extern alias R3;
extern alias R4;

using System.Linq;
using Hl7.Fhir.Utility;

using R3Model = R3::Hl7.Fhir.Model;
using R4Model = R4::Hl7.Fhir.Model;

namespace FhirTool.Conversion.Converters
{
    internal partial class FhirR4ToR3ConversionRoutines : BaseConverter
    {
        public FhirR4ToR3ConversionRoutines() : base()
        {
            Map.Add<R3Model.QuestionnaireResponse, R4Model.QuestionnaireResponse>(ConvertQuestionnaireResponse);
            Map.Add<R3Model.Questionnaire.ItemComponent, R4Model.Questionnaire.ItemComponent>(ConvertQuestionnaireItemComponent);
            Map.Add<R3Model.Questionnaire.EnableWhenComponent, R4Model.Questionnaire.EnableWhenComponent>(ConvertQuestionnaireEnableWhenComponent);
            Map.Add<R3Model.Money, R4Model.Money>(ConvertMoney);
            Map.Add<R3Model.Attachment, R4Model.Attachment>(ConvertAttachment);
            Map.Add<R3Model.RelatedArtifact, R4Model.RelatedArtifact>(ConvertRelatedArtifact);
            Map.Add<R3Model.DataRequirement, R4Model.DataRequirement>(ConvertDataRequirement);
            Map.Add<R3Model.DataRequirement.CodeFilterComponent, R4Model.DataRequirement.CodeFilterComponent>(ConvertDataRequirementCodeFilterComponent);
            Map.Add<R3Model.Meta, R4Model.Meta>(ConvertMeta);
            Map.Add<R3Model.ElementDefinition.TypeRefComponent, R4Model.ElementDefinition.TypeRefComponent>(ConvertElementDefinitionTypeRefComponent);
            Map.Add<R3Model.ElementDefinition.ConstraintComponent, R4Model.ElementDefinition.ConstraintComponent>(ConvertElementDefinitionConstraintComponent);
            Map.Add<R3Model.ElementDefinition.ElementDefinitionBindingComponent, R4Model.ElementDefinition.ElementDefinitionBindingComponent>(ConvertElementDefinitionBindingComponent);
            Map.Add<R3Model.ValueSet.ConceptSetComponent, R4Model.ValueSet.ConceptSetComponent>(ConvertValueSetConceptSetComponent);
            Map.Add<R3Model.ValueSet.FilterComponent, R4Model.ValueSet.FilterComponent>(ConvertValueSetFilterComponent);
            Map.Add<R3Model.Annotation, R4Model.Annotation>(ConvertAnnotation);
            Map.Add<R3Model.Timing.RepeatComponent, R4Model.Timing.RepeatComponent>(ConvertTimingRepeatComponent);
            Map.Add<R3Model.ParameterDefinition, R4Model.ParameterDefinition>(ConvertParameterDefinition);
        }

        private static void ConvertQuestionnaireResponse(FhirConverter converter, R3Model.QuestionnaireResponse to, R4Model.QuestionnaireResponse from)
        {
            to.Questionnaire = new R3Model.ResourceReference(from.Questionnaire);
        }

        private static void ConvertQuestionnaireItemComponent(FhirConverter converter, R3Model.Questionnaire.ItemComponent to, R4Model.Questionnaire.ItemComponent from)
        {
            to.Initial = converter.Convert<R3Model.Element, R4Model.Element>(from.Initial.FirstOrDefault()?.Value);
            to.Options = ConvertCanonicalToResourceReference(from.AnswerValueSetElement, converter);
        }

        private static void ConvertQuestionnaireEnableWhenComponent(FhirConverter converter, R3Model.Questionnaire.EnableWhenComponent to, R4Model.Questionnaire.EnableWhenComponent from)
        {
            if (from.Operator == R4Model.Questionnaire.QuestionnaireItemOperator.Exists && from.Answer is R4Model.FhirBoolean answer)
            {
                to.HasAnswer = answer.Value;
            }
        }

        private static void ConvertMoney(FhirConverter converter, R3Model.Money to, R4Model.Money from)
        {
            if (from.Currency.HasValue)
            {
                to.System = "urn:iso:std:iso:4217";
                to.CodeElement = new R3Model.Code
                {
                    Value = from.Currency.Value.GetLiteral(),
                    Extension = converter.Convert<R3Model.Extension, R4Model.Extension>(from.Extension).ToList()
                };
            }
        }

        private static void ConvertAttachment(FhirConverter converter, R3Model.Attachment to, R4Model.Attachment from)
        {
            to.UrlElement = ConvertFhirUrlToFhirUri(from.UrlElement, converter);
        }

        private static void ConvertRelatedArtifact(FhirConverter converter, R3Model.RelatedArtifact to, R4Model.RelatedArtifact from)
        {
            to.UrlElement = ConvertFhirUrlToFhirUri(from.UrlElement, converter);
            to.CitationElement = ConvertMarkdownToFhirString(from.Citation, converter);
            to.Resource = ConvertCanonicalToResourceReference(from.ResourceElement, converter);
        }

        private static void ConvertDataRequirement(FhirConverter converter, R3Model.DataRequirement to, R4Model.DataRequirement from)
        {
            to.ProfileElement = from.ProfileElement.Select(e => ConvertCanonicalToFhirUri(e, converter)).ToList();
        }

        private static void ConvertDataRequirementCodeFilterComponent(FhirConverter converter, R3Model.DataRequirement.CodeFilterComponent to, R4Model.DataRequirement.CodeFilterComponent from)
        {
            to.ValueSet = ConvertCanonicalToResourceReference(from.ValueSetElement, converter);
        }

        private static void ConvertMeta(FhirConverter converter, R3Model.Meta to, R4Model.Meta from)
        {
            to.ProfileElement = from.ProfileElement.Select(e => ConvertCanonicalToFhirUri(e, converter)).ToList();
        }

        private static void ConvertElementDefinitionTypeRefComponent(FhirConverter converter, R3Model.ElementDefinition.TypeRefComponent to, R4Model.ElementDefinition.TypeRefComponent from)
        {
            to.ProfileElement = ConvertCanonicalToFhirUri(from.ProfileElement.FirstOrDefault(), converter);
            to.TargetProfileElement = ConvertCanonicalToFhirUri(from.TargetProfileElement.FirstOrDefault(), converter);
        }

        private static void ConvertElementDefinitionConstraintComponent(FhirConverter converter, R3Model.ElementDefinition.ConstraintComponent to, R4Model.ElementDefinition.ConstraintComponent from)
        {
            to.SourceElement = ConvertCanonicalToFhirUri(from.SourceElement, converter);
        }

        private static void ConvertElementDefinitionBindingComponent(FhirConverter converter, R3Model.ElementDefinition.ElementDefinitionBindingComponent to, R4Model.ElementDefinition.ElementDefinitionBindingComponent from)
        {
            to.ValueSet = ConvertCanonicalToResourceReference(from.ValueSetElement, converter);
        }

        private static void ConvertValueSetConceptSetComponent(FhirConverter converter, R3Model.ValueSet.ConceptSetComponent to, R4Model.ValueSet.ConceptSetComponent from)
        {
            to.ValueSetElement = from.ValueSetElement.Select(e => ConvertCanonicalToFhirUri(e, converter)).ToList();
        }

        private static void ConvertValueSetFilterComponent(FhirConverter converter, R3Model.ValueSet.FilterComponent to, R4Model.ValueSet.FilterComponent from)
        {
            to.ValueElement = ConvertFhirStringToCode(from.ValueElement, converter);
        }

        private static void ConvertAnnotation(FhirConverter converter, R3Model.Annotation to, R4Model.Annotation from)
        {
            to.TextElement = ConvertMarkdownToFhirString(from.Text, converter);
        }

        private static void ConvertTimingRepeatComponent(FhirConverter converter, R3Model.Timing.RepeatComponent to, R4Model.Timing.RepeatComponent from)
        {
            to.CountElement = ConvertPositiveIntToInteger(from.CountElement, converter);
            to.CountMaxElement = ConvertPositiveIntToInteger(from.CountMaxElement, converter);
            to.FrequencyElement = ConvertPositiveIntToInteger(from.FrequencyElement, converter);
            to.FrequencyMaxElement = ConvertPositiveIntToInteger(from.FrequencyMaxElement, converter);
        }

        private static void ConvertParameterDefinition(FhirConverter converter, R3Model.ParameterDefinition to, R4Model.ParameterDefinition from)
        {
            to.Profile = ConvertCanonicalToResourceReference(from.ProfileElement, converter);
        }

        // Helpers
        private static R3Model.Code ConvertFhirStringToCode(R4Model.FhirString from, FhirConverter converter)
        {
            if (from == null) return default;

            return new R3Model.Code
            {
                Value = from.Value,
                Extension = converter.Convert<R3Model.Extension, R4Model.Extension>(from.Extension).ToList()
            };
        }

        private static R3Model.FhirUri ConvertCanonicalToFhirUri(R4Model.Canonical from, FhirConverter converter)
        {
            if (from == null) return default;

            return new R3Model.FhirUri
            {
                Value = from.Value,
                Extension = converter.Convert<R3Model.Extension, R4Model.Extension>(from.Extension).ToList()
            };
        }

        private static R3Model.FhirUri ConvertFhirUrlToFhirUri(R4Model.FhirUrl from, FhirConverter converter)
        {
            if (from == null) return default;

            return new R3Model.FhirUri
            {
                Value = from.Value,
                Extension = converter.Convert<R3Model.Extension, R4Model.Extension>(from.Extension).ToList()
            };
        }

        private static R3Model.FhirString ConvertMarkdownToFhirString(R4Model.Markdown from, FhirConverter converter)
        {
            if (from == null) return default;

            return new R3Model.FhirString
            {
                Value = from.Value,
                Extension = converter.Convert<R3Model.Extension, R4Model.Extension>(from.Extension).ToList()
            };
        }

        private static R3Model.Integer ConvertPositiveIntToInteger(R4Model.PositiveInt from, FhirConverter converter)
        {
            if (from == null) return default;

            return new R3Model.Integer
            {
                Value = from.Value,
                Extension = converter.Convert<R3Model.Extension, R4Model.Extension>(from.Extension).ToList()
            };
        }

        private static R3Model.ResourceReference ConvertCanonicalToResourceReference(R4Model.Canonical from, FhirConverter converter)
        {
            if (from == null) return default;

            return new R3Model.ResourceReference
            {
                Reference = from.Value,
                Extension = converter.Convert<R3Model.Extension, R4Model.Extension>(from.Extension).ToList()
            };
        }
    }
}
