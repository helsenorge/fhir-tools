extern alias R3;
extern alias R4;
using R3::Hl7.Fhir.Model;
using Hl7.Fhir.ElementModel;
using R4Model = R4::Hl7.Fhir.Model;
using R4::Hl7.Fhir.ElementModel;

namespace FhirTool.Conversion
{
    public class ValueSetR4ToR3Conversion : BaseR4ToR3Conversion
    {
        public ValueSet Convert(ISourceNode source)
        {
            var valueSetR4 = source.ToPoco<R4Model.ValueSet>();

            return new ValueSet
            {
                Extension = Convert<Extension, R4Model.Extension>(valueSetR4.Extension),
                ModifierExtension = Convert<Extension, R4Model.Extension>(valueSetR4.ModifierExtension),
                Id = valueSetR4.Id,
                Identifier = Convert<Identifier, R4Model.Identifier>(valueSetR4.Identifier),
                Meta = ConvertElement(valueSetR4.Meta) as Meta,
                Compose = ConvertElement(valueSetR4.Compose) as ValueSet.ComposeComponent,
                Contact = Convert<ContactDetail, R4Model.ContactDetail>(valueSetR4.Contact),
                Contained = Convert<Resource, R4Model.Resource>(valueSetR4.Contained),
                Copyright = ConvertElement(valueSetR4.Copyright) as Markdown,
                Date = valueSetR4.Date,
                Description = ConvertElement(valueSetR4.Description) as Markdown,
                Expansion = ConvertElement(valueSetR4.Expansion) as ValueSet.ExpansionComponent,
                Experimental = valueSetR4.Experimental,
                Immutable = valueSetR4.Immutable,
                ImplicitRules = valueSetR4.ImplicitRules,
                Jurisdiction = Convert<CodeableConcept, R4Model.CodeableConcept>(valueSetR4.Jurisdiction),
                Language = valueSetR4.Language,
                Name = valueSetR4.Name,
                Publisher = valueSetR4.Publisher,
                Purpose = ConvertElement(valueSetR4.Purpose) as Markdown,
                ResourceBase = valueSetR4.ResourceBase,
                Status = ConvertEnum<PublicationStatus, R4Model.PublicationStatus>(valueSetR4.Status),
                Text = ConvertElement(valueSetR4.Text) as Narrative,
                Title = valueSetR4.Title,
                Url = valueSetR4.Url,
                UseContext = Convert<UsageContext, R4Model.UsageContext>(valueSetR4.UseContext),
                Version = valueSetR4.Version
            };
        }
    }
}
