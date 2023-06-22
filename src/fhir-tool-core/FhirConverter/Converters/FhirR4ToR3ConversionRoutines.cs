/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

extern alias R3;
extern alias R4;
using System.Linq;
using Hl7.Fhir.Utility;

using TargetModel = R3::Hl7.Fhir.Model;
using SourceModel = R4::Hl7.Fhir.Model;
using Hl7.Fhir.Model;

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
            Map.Add<TargetModel.ElementDefinition.TypeRefComponent, SourceModel.ElementDefinition.TypeRefComponent>(ConvertElementDefinitionTypeRefComponent);
            Map.Add<TargetModel.ElementDefinition.ConstraintComponent, SourceModel.ElementDefinition.ConstraintComponent>(ConvertElementDefinitionConstraintComponent);
            Map.Add<TargetModel.ElementDefinition.ElementDefinitionBindingComponent, SourceModel.ElementDefinition.ElementDefinitionBindingComponent>(ConvertElementDefinitionBindingComponent);
            Map.Add<TargetModel.ValueSet.ConceptSetComponent, SourceModel.ValueSet.ConceptSetComponent>(ConvertValueSetConceptSetComponent);
            Map.Add<TargetModel.ValueSet.FilterComponent, SourceModel.ValueSet.FilterComponent>(ConvertValueSetFilterComponent);
            Map.Add<TargetModel.Annotation, SourceModel.Annotation>(ConvertAnnotation);
            Map.Add<TargetModel.Timing.RepeatComponent, SourceModel.Timing.RepeatComponent>(ConvertTimingRepeatComponent);
            Map.Add<TargetModel.ParameterDefinition, SourceModel.ParameterDefinition>(ConvertParameterDefinition);
            Map.Add<TargetModel.Dosage, SourceModel.Dosage>(ConvertDosage);
            Map.Add<TargetModel.Endpoint, SourceModel.Endpoint>(ConvertEndpoint);
            Map.Add<TargetModel.StructureDefinition, SourceModel.StructureDefinition>(ConvertStructureDefinition);
            Map.Add<TargetModel.ElementDefinition, SourceModel.ElementDefinition>(ConvertElementDefinition);
        }

        private static void ConvertElementDefinition(TargetModel.ElementDefinition to, SourceModel.ElementDefinition from, FhirConverter converter)
        {
            to.DefinitionElement = converter.ConvertElement<Markdown, Markdown>(from.Definition);
            to.CommentElement = converter.ConvertElement<Markdown, Markdown>(from.Comment);
            to.RequirementsElement = converter.ConvertElement<Markdown, Markdown>(from.Requirements);
            to.MeaningWhenMissingElement = converter.ConvertElement<Markdown, Markdown>(from.MeaningWhenMissing);
        }

        private void ConvertStructureDefinition(TargetModel.StructureDefinition to, SourceModel.StructureDefinition from, FhirConverter converter)
        {
            to.FhirVersionElement = new Id("3.0.2")
            {
                Extension = converter.ConvertList<Extension, Extension>(from.FhirVersionElement?.Extension).ToList()
            };

            var contextTypeElement = from.Context?.FirstOrDefault()?.TypeElement;
            to.ContextTypeElement = converter.ConvertElement<Code<TargetModel.StructureDefinition.ExtensionContext>, Code<SourceModel.StructureDefinition.ExtensionContextType>>(contextTypeElement);
            to.ContextElement = from.Context.Select(context => 
                converter.ConvertElement<FhirString, FhirString>(context.ExpressionElement)
            ).ToList();

            to.TypeElement = ConvertFhirUriToCode(from.TypeElement, converter);

            to.BaseDefinitionElement = ConvertCanonicalToFhirUri(from.BaseDefinitionElement, converter);
        }

        private static void ConvertQuestionnaireResponse(TargetModel.QuestionnaireResponse to, SourceModel.QuestionnaireResponse from, FhirConverter converter)
        {
            to.Questionnaire = new ResourceReference(from.Questionnaire);
        }

        private static void ConvertQuestionnaireItemComponent(TargetModel.Questionnaire.ItemComponent to, SourceModel.Questionnaire.ItemComponent from, FhirConverter converter)
        {
            to.Initial = converter.ConvertElement<DataType, DataType>(from.Initial.FirstOrDefault()?.Value);
            to.Options = ConvertCanonicalToResourceReference(from.AnswerValueSetElement, converter);
            to.Option = from.AnswerOption == null ? to.Option : converter.ConvertList<TargetModel.Questionnaire.OptionComponent, SourceModel.Questionnaire.AnswerOptionComponent>(from.AnswerOption).ToList();
        }

        private static void ConvertQuestionnaireEnableWhenComponent(TargetModel.Questionnaire.EnableWhenComponent to, SourceModel.Questionnaire.EnableWhenComponent from, FhirConverter converter)
        {
            if (from.Operator == SourceModel.Questionnaire.QuestionnaireItemOperator.Exists && from.Answer is FhirBoolean answer)
            {
                to.HasAnswer = answer.Value;
            }
        }

        private static void ConvertMoney(TargetModel.Money to, SourceModel.Money from, FhirConverter converter)
        {
            if (from.Currency.HasValue)
            {
                to.System = "urn:iso:std:iso:4217";
                to.CodeElement = new Code
                {
                    Value = from.Currency.Value.GetLiteral(),
                    Extension = converter.ConvertList<Extension, Extension>(from.Extension).ToList()
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

            to.Dose = fromDose == null ? to.Dose : converter.ConvertElement<DataType, DataType>(fromDose.Dose);
            to.Rate = fromRate == null ? to.Rate : converter.ConvertElement<DataType, DataType>(fromRate.Rate);
        }

        private static void ConvertEndpoint(TargetModel.Endpoint to, SourceModel.Endpoint from, FhirConverter converter)
        {
            to.AddressElement = ConvertFhirUrlToFhirUri(from.AddressElement, converter);
        }

        // Helpers
        private static Code ConvertFhirStringToCode(FhirString from, FhirConverter converter)
        {
            if (from == null) return default;

            return new Code
            {
                Value = from.Value,
                Extension = converter.ConvertList<Extension, Extension>(from.Extension).ToList()
            };
        }

        private static FhirUri ConvertCanonicalToFhirUri(Canonical from, FhirConverter converter)
        {
            if (from == null) return default;

            return new FhirUri
            {
                Value = from.Value,
                Extension = converter.ConvertList<Extension, Extension>(from.Extension).ToList()
            };
        }

        private static FhirUri ConvertFhirUrlToFhirUri(FhirUrl from, FhirConverter converter)
        {
            if (from == null) return default;

            return new FhirUri
            {
                Value = from.Value,
                Extension = converter.ConvertList<Extension, Extension>(from.Extension).ToList()
            };
        }

        private static Code ConvertFhirUriToCode(FhirUri from, FhirConverter converter)
        {
            if (from == null) return default;

            return new Code
            {
                Value = from.Value,
                Extension = converter.ConvertList<Extension, Extension>(from.Extension).ToList()
            };
        }

        private static FhirString ConvertMarkdownToFhirString(Markdown from, FhirConverter converter)
        {
            if (from == null) return default;

            return new FhirString
            {
                Value = from.Value,
                Extension = converter.ConvertList<Extension, Extension>(from.Extension).ToList()
            };
        }

        private static Integer ConvertPositiveIntToInteger(PositiveInt from, FhirConverter converter)
        {
            if (from == null) return default;

            return new Integer
            {
                Value = from.Value,
                Extension = converter.ConvertList<Extension, Extension>(from.Extension).ToList()
            };
        }

        private static ResourceReference ConvertCanonicalToResourceReference(Canonical from, FhirConverter converter)
        {
            if (from == null) return default;

            return new ResourceReference
            {
                Reference = from.Value,
                Extension = converter.ConvertList<Extension, Extension>(from.Extension).ToList()
            };
        }
    }
}
