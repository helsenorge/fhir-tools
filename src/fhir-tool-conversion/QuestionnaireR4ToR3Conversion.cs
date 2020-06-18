extern alias R3;
extern alias R4;
using Hl7.Fhir.ElementModel;
using R3::Hl7.Fhir.Model;

using R4::Hl7.Fhir.ElementModel;
using R4Model = R4::Hl7.Fhir.Model;

namespace FhirTool.Conversion
{
    public class QuestionnaireR4ToR3Conversion : BaseR4ToR3Conversion
    {
        public Questionnaire Convert(ISourceNode source)
        {
            R4Model.Questionnaire questR4 = source.ToPoco<R4Model.Questionnaire>();
            return new Questionnaire
            {
                Extension = Convert<Extension, R4Model.Extension>(questR4.Extension),
                ModifierExtension = Convert<Extension, R4Model.Extension>(questR4.ModifierExtension),
                ApprovalDate = questR4.ApprovalDate,
                Code = Convert<Coding, R4Model.Coding>(questR4.Code),
                Contact = Convert<ContactDetail, R4Model.ContactDetail>(questR4.Contact),
                Contained = Convert<Resource, R4Model.Resource>(questR4.Contained),
                Copyright = ConvertElement(questR4.Copyright) as Markdown,
                Date = questR4.Date,
                Description = ConvertElement(questR4.Description) as Markdown,
                EffectivePeriod = ConvertElement(questR4.EffectivePeriod) as Period,
                Experimental = questR4.Experimental,
                Id = questR4.Id,
                Identifier = Convert<Identifier, R4Model.Identifier>(questR4.Identifier),
                ImplicitRules = questR4.ImplicitRules,
                Item = Convert<Questionnaire.ItemComponent, R4Model.Questionnaire.ItemComponent>(questR4.Item),
                Jurisdiction = Convert<CodeableConcept, R4Model.CodeableConcept>(questR4.Jurisdiction),
                Language = questR4.Language,
                LastReviewDate = questR4.LastReviewDate,
                Meta = ConvertElement(questR4.Meta) as Meta,
                Name = questR4.Name,
                Publisher = questR4.Publisher,
                Purpose = ConvertElement(questR4.Purpose) as Markdown,
                ResourceBase = questR4.ResourceBase,
                Status = ConvertEnum<PublicationStatus, R4Model.PublicationStatus>(questR4.Status),
                SubjectType = Convert<ResourceType, R4Model.ResourceType>(questR4.SubjectType),
                Text = ConvertElement(questR4.Text) as Narrative,
                Title = questR4.Title,
                Url = questR4.Url,
                UseContext = Convert<UsageContext, R4Model.UsageContext>(questR4.UseContext),
                Version = questR4.Version
            };
        }
    }
}
