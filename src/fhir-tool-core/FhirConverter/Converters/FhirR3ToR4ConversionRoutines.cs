/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

extern alias R3;
extern alias R4;

using System.Linq;
using System.Collections.Generic;
using Hl7.Fhir.Utility;

using TargetModel = R4::Hl7.Fhir.Model;
using SourceModel = R3::Hl7.Fhir.Model;
using Hl7.Fhir.Model;

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
            Map.Add<TargetModel.ElementDefinition.TypeRefComponent, SourceModel.ElementDefinition.TypeRefComponent>(ConvertElementDefinitionTypeRefComponent);
            Map.Add<TargetModel.ElementDefinition.ConstraintComponent, SourceModel.ElementDefinition.ConstraintComponent>(ConvertElementDefinitionConstraintComponent);
            Map.Add<TargetModel.ElementDefinition.ElementDefinitionBindingComponent, SourceModel.ElementDefinition.ElementDefinitionBindingComponent>(ConvertElementDefinitionBindingComponent);
            Map.Add<TargetModel.ValueSet.ConceptSetComponent, SourceModel.ValueSet.ConceptSetComponent>(ConvertValueSetConceptSetComponent);
            Map.Add<TargetModel.ValueSet.FilterComponent, SourceModel.ValueSet.FilterComponent>(ConvertValueSetFilterComponent);
            Map.Add<TargetModel.Annotation, SourceModel.Annotation>(ConvertAnnotation);
            Map.Add<TargetModel.Timing.RepeatComponent, SourceModel.Timing.RepeatComponent>(ConvertTimingRepeatComponent);
            Map.Add<TargetModel.ParameterDefinition, SourceModel.ParameterDefinition>(ConvertParameterDefinition);
            Map.Add<TargetModel.Signature, SourceModel.Signature>(ConvertSignature);
            Map.Add<TargetModel.Dosage, SourceModel.Dosage>(ConvertDosage);
            Map.Add<TargetModel.Endpoint, SourceModel.Endpoint>(ConvertEndpoint);
            Map.Add<TargetModel.StructureDefinition, SourceModel.StructureDefinition>(ConvertStructureDefinition);
            Map.Add<TargetModel.ElementDefinition, SourceModel.ElementDefinition>(ConvertElementDefinition);
        }

        private static void ConvertElementDefinition(TargetModel.ElementDefinition to, SourceModel.ElementDefinition from, FhirConverter converter)
        {
            to.Definition = converter.ConvertElement<Markdown, Markdown>(from.DefinitionElement);
            to.Comment = converter.ConvertElement<Markdown, Markdown>(from.CommentElement);
            to.Requirements = converter.ConvertElement<Markdown, Markdown>(from.RequirementsElement);
            to.MeaningWhenMissing = converter.ConvertElement<Markdown, Markdown>(from.MeaningWhenMissingElement);
        }

        private static void ConvertStructureDefinition(TargetModel.StructureDefinition to, SourceModel.StructureDefinition from, FhirConverter converter)
        {
            // StructureDefinition.fhirVersion
            to.FhirVersionElement = new Code<TargetModel.FHIRVersion>(TargetModel.FHIRVersion.N4_0_0)
            {
                Extension = converter.ConvertList<Extension, Extension>(from.FhirVersionElement?.Extension).ToList()
            };

            // StructureDefinition.context
            to.Context = from.ContextElement.Select(contextElement =>
                new TargetModel.StructureDefinition.ContextComponent
                {
                    TypeElement = converter.ConvertElement<Code<TargetModel.StructureDefinition.ExtensionContextType>, Code<SourceModel.StructureDefinition.ExtensionContext>>(from.ContextTypeElement),
                    ExpressionElement = converter.ConvertElement<FhirString, FhirString>(contextElement)
                }
            ).ToList();

            // StructureDefinition.type
            to.TypeElement = ConvertCodeToFhirUri(from.TypeElement, converter);

            // StructureDefinition.baseDefinition
            to.BaseDefinitionElement = ConvertFhirUriToCanonical(from.BaseDefinitionElement, converter);
        }

        private static void ConvertQuestionnaireResponse(TargetModel.QuestionnaireResponse to, SourceModel.QuestionnaireResponse from, FhirConverter converter)
        {
            to.QuestionnaireElement = ConvertResourceReferenceToCanonical(from.Questionnaire, converter);
        }

        private static void ConvertQuestionnaireItemComponent(TargetModel.Questionnaire.ItemComponent to, SourceModel.Questionnaire.ItemComponent from, FhirConverter converter)
        {
            to.Initial = from.Initial == null ? to.Initial : new List<TargetModel.Questionnaire.InitialComponent> { ConvertElementToInitialComponent(from.Initial, converter) };
            to.AnswerValueSetElement = ConvertResourceReferenceToCanonical(from.Options, converter);
            to.AnswerOption = from.Option == null ? to.AnswerOption : converter.ConvertList<TargetModel.Questionnaire.AnswerOptionComponent, SourceModel.Questionnaire.OptionComponent>(from.Option).ToList();
        }

        private static void ConvertQuestionnaireEnableWhenComponent(TargetModel.Questionnaire.EnableWhenComponent to, SourceModel.Questionnaire.EnableWhenComponent from, FhirConverter converter)
        {
            if (from.HasAnswer.HasValue)
            {
                to.Operator = TargetModel.Questionnaire.QuestionnaireItemOperator.Exists;
                to.Answer = new FhirBoolean(from.HasAnswer);
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
                to.ValueElement = converter.ConvertElement<FhirDecimal, FhirDecimal>(from.ValueElement);
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
            if (from.ValueSet is ResourceReference fromResourceReference)
            {
                to.ValueSetElement = ConvertResourceReferenceToCanonical(fromResourceReference, converter);
            }
            else if (from.ValueSet is FhirString fromFhirString)
            {
                to.ValueSetElement = ConvertFhirStringToCanonical(fromFhirString, converter);
            }
        }

        private static void ConvertElementDefinitionTypeRefComponent(TargetModel.ElementDefinition.TypeRefComponent to, SourceModel.ElementDefinition.TypeRefComponent from, FhirConverter converter)
        {
            to.ProfileElement = from.ProfileElement == null ? to.ProfileElement : new List<Canonical> { ConvertFhirUriToCanonical(from.ProfileElement, converter) };
            to.TargetProfileElement = from.TargetProfileElement == null ? to.TargetProfileElement : new List<Canonical> { ConvertFhirUriToCanonical(from.TargetProfileElement, converter) };
        }

        private static void ConvertElementDefinitionConstraintComponent(TargetModel.ElementDefinition.ConstraintComponent to, SourceModel.ElementDefinition.ConstraintComponent from, FhirConverter converter)
        {
            to.SourceElement = ConvertFhirUriToCanonical(from.SourceElement, converter);
        }

        private static void ConvertElementDefinitionBindingComponent(TargetModel.ElementDefinition.ElementDefinitionBindingComponent to, SourceModel.ElementDefinition.ElementDefinitionBindingComponent from, FhirConverter converter)
        {
            if (from.ValueSet is FhirUri fromFhirUri)
            {
                to.ValueSetElement = ConvertFhirUriToCanonical(fromFhirUri, converter);
            }
            else if (from.ValueSet is ResourceReference fromResourceReference)
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
                if (from.Who is ResourceReference fromResourceReference)
                {
                    to.Who = converter.ConvertElement<ResourceReference, ResourceReference>(fromResourceReference);
                }
                else if (from.Who is FhirUri fromFhirUri)
                {
                    to.Who = ConvertFhirUriToResourceReference(fromFhirUri, converter);
                }
            }

            if (from.OnBehalfOf != null)
            {
                if (from.OnBehalfOf is ResourceReference fromResourceReference)
                {
                    to.OnBehalfOf = converter.ConvertElement<ResourceReference, ResourceReference>(fromResourceReference);
                }
                else if (from.OnBehalfOf is FhirUri fromFhirUri)
                {
                    to.OnBehalfOf = ConvertFhirUriToResourceReference(fromFhirUri, converter);
                }
            }
        }

        private static void ConvertDosage(TargetModel.Dosage to, SourceModel.Dosage from, FhirConverter converter)
        {
            if (from.Dose != null)
            {
                to.DoseAndRate.Add(new TargetModel.Dosage.DoseAndRateComponent { Dose = converter.ConvertElement<DataType, DataType>(from.Dose) });
            }
            if (from.Rate != null)
            {
                to.DoseAndRate.Add(new TargetModel.Dosage.DoseAndRateComponent { Rate = converter.ConvertElement<DataType, DataType>(from.Rate) });
            }
        }

        private static void ConvertEndpoint(TargetModel.Endpoint to, SourceModel.Endpoint from, FhirConverter converter)
        {
            to.AddressElement = ConvertFhirUriToFhirUrl(from.AddressElement, converter);
        }


        // Helpers
        private static Code<TargetModel.Money.Currencies> ConvertCodeToCodeMoney(Code from, FhirConverter converter)
        {
            if (from == null) return default;

            return new Code<TargetModel.Money.Currencies>
            {
                Value = string.IsNullOrWhiteSpace(from.Value) ? null : EnumUtility.ParseLiteral<TargetModel.Money.Currencies>(from.Value),
                Extension = converter.ConvertList<Extension, Extension>(from.Extension).ToList()
            };
        }

        private static FhirUrl ConvertFhirUriToFhirUrl(FhirUri from, FhirConverter converter)
        {
            if (from == null) return default;

            return new FhirUrl
            {
                Value = from.Value,
                Extension = converter.ConvertList<Extension, Extension>(from.Extension).ToList()
            };
        }

        private static TargetModel.Questionnaire.InitialComponent ConvertElementToInitialComponent(DataType from, FhirConverter converter)
        {
            return new TargetModel.Questionnaire.InitialComponent
            {
                Value = converter.ConvertElement<DataType, DataType>(from),
            };
        }

        private static Canonical ConvertFhirUriToCanonical(FhirUri from, FhirConverter converter)
        {
            if (from == null) return default;

            return new Canonical
            {
                Value = from.Value,
                Extension = converter.ConvertList<Extension, Extension>(from.Extension).ToList()
            };
        }

        private static Canonical ConvertResourceReferenceToCanonical(ResourceReference from, FhirConverter converter)
        {
            if (from == null) return default;

            return new Canonical
            {
                Value = from.Reference,
                Extension = converter.ConvertList<Extension, Extension>(from.Extension).ToList()
            };
        }

        private static Canonical ConvertFhirStringToCanonical(FhirString from, FhirConverter converter)
        {
            if (from == null) return default;

            return new Canonical
            {
                Value = from.Value,
                Extension = converter.ConvertList<Extension, Extension>(from.Extension).ToList()
            };
        }

        private static Markdown ConvertFhirStringToMarkdown(FhirString from, FhirConverter converter)
        {
            if (from == null) return default;

            return new Markdown
            {
                Value = from.Value,
                Extension = converter.ConvertList<Extension, Extension>(from.Extension).ToList()
            };
        }

        private static FhirString ConvertCodeToFhirString(Code from, FhirConverter converter)
        {
            if (from == null) return default;

            return new FhirString
            {
                Value = from.Value,
                Extension = converter.ConvertList<Extension, Extension>(from.Extension).ToList()
            };
        }

        private static FhirUri ConvertCodeToFhirUri(Code from, FhirConverter converter)
        {
            if (from == null) return default;

            return new FhirUri
            {
                Value = from.Value,
                Extension = converter.ConvertList<Extension, Extension>(from.Extension).ToList()
            };
        }

        private static ResourceReference ConvertFhirUriToResourceReference(FhirUri from, FhirConverter converter)
        {
            if (from == null) return default;

            return new ResourceReference
            {
                Reference = from.Value,
                Extension = converter.ConvertList<Extension, Extension>(from.Extension).ToList()
            };
        }

        private static PositiveInt ConvertIntegerToPositiveInt(Integer from, FhirConverter converter)
        {
            if (from == null) return default;

            return new PositiveInt
            {
                Value = from.Value,
                Extension = converter.ConvertList<Extension, Extension>(from.Extension).ToList()
            };
        }
    }
}
