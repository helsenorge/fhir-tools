extern alias R3;
extern alias R4;
using Hl7.Fhir.Utility;
using Hl7.Fhir.Model;
using R3::Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using R4Model = R4::Hl7.Fhir.Model;

namespace FhirTool.Conversion
{
    public abstract class BaseR4ToR3Conversion
    {
        protected IEnumerable<TTo?> Convert<TTo, TFrom>(IEnumerable<TFrom?> enums)
            where TTo : struct
            where TFrom : struct, Enum
        {
            var enumsR3 = new List<TTo?>();
            foreach (var @enum in enums)
                enumsR3.Add(ConvertEnum<TTo, TFrom>(@enum));
            return enumsR3;
        }

        protected TTo? ConvertEnum<TTo, TFrom>(TFrom? from)
            where TTo : struct
            where TFrom : struct, Enum
        {
            if (from == null) return default;

            return EnumUtility.ParseLiteral<TTo>(from.GetLiteral());
        }

        protected List<TTo> Convert<TTo, TFrom>(List<TFrom> items)
            where TTo : Base
            where TFrom : Base
        {
            var itemsR3 = new List<TTo>();
            foreach (var item in items)
                itemsR3.Add(ConvertElement(item) as TTo);
            return itemsR3;
        }

        protected Element ConvertInitalComponent(List<R4Model.Questionnaire.InitialComponent> initials)
        {
            var initial = initials.FirstOrDefault();
            if (initial == null) return null;

            return ConvertElement(initial);
        }

        protected List<Questionnaire.EnableWhenComponent> ConvertEnableWhenComponent(List<R4Model.Questionnaire.EnableWhenComponent> enableWhenComponents)
        {
            var enableWhenComponentsR3 = new List<Questionnaire.EnableWhenComponent>();
            foreach (var enableWhen in enableWhenComponents)
            {
                var enableWhenR3 = new Questionnaire.EnableWhenComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(enableWhen.Extension),
                    ModifierExtension = Convert<Extension, R4Model.Extension>(enableWhen.ModifierExtension),
                    Question = enableWhen.Question,
                };

                if (enableWhen.Answer != null)
                    enableWhenR3.Answer = ConvertElement(enableWhen.Answer);
                if (enableWhen.Operator == R4Model.Questionnaire.QuestionnaireItemOperator.Exists)
                    enableWhenR3.HasAnswer = true;

                enableWhenComponentsR3.Add(enableWhenR3);
            }
            return enableWhenComponentsR3;
        }

        public Element ConvertElement(Base element)
        {
            if (element is R4Model.FhirBoolean boolean)
            {
                return new FhirBoolean(boolean.Value);
            }
            else if (element is R4Model.FhirDecimal dec)
            {
                return new FhirDecimal(dec.Value);
            }
            else if (element is R4Model.Integer integer)
            {
                return new Integer(integer.Value);
            }
            else if (element is R4Model.Date date)
            {
                return new Date(date.Value);
            }
            else if (element is R4Model.FhirDateTime dateTime)
            {
                return new FhirDateTime(dateTime.Value);
            }
            else if (element is R4Model.Time time)
            {
                return new Time(time.Value);
            }
            else if (element is R4Model.FhirString str)
            {
                return new FhirString(str.Value);
            }
            else if (element is R4Model.FhirUri uri)
            {
                return new FhirUri(uri.Value);
            }
            else if (element is R4Model.Attachment attachment)
            {
                return new Attachment
                {
                    Extension = Convert<Extension, R4Model.Extension>(attachment.Extension),
                    ContentType = attachment.ContentType,
                    Creation = attachment.Creation,
                    Data = attachment.Data,
                    Hash = attachment.Hash,
                    Language = attachment.Language,
                    Size = attachment.Size,
                    Title = attachment.Title,
                    Url = attachment.Url
                };
            }
            else if (element is R4Model.Coding coding)
            {
                return new Coding
                {
                    Extension = Convert<Extension, R4Model.Extension>(coding.Extension),
                    Code = coding.Code,
                    Display = coding.Display,
                    System = coding.System,
                    UserSelected = coding.UserSelected,
                    Version = coding.Version,
                };
            }
            else if (element is R4Model.Quantity quantity)
            {
                return new Quantity
                {
                    Extension = Convert<Extension, R4Model.Extension>(quantity.Extension),
                    Code = quantity.Code,
                    Comparator = ConvertEnum<Quantity.QuantityComparator, R4Model.Quantity.QuantityComparator>(quantity.Comparator),
                    System = quantity.System,
                    Unit = quantity.Unit,
                    Value = quantity.Value
                };
            }
            else if (element is R4Model.ResourceReference)
            {
                // TODO: Use our extension for STU3
                return null;
            }
            else if (element is R4Model.Questionnaire.AnswerOptionComponent answerOption)
            {
                return new Questionnaire.OptionComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(answerOption.Extension),
                    ModifierExtension = Convert<Extension, R4Model.Extension>(answerOption.ModifierExtension),
                    Value = ConvertElement(answerOption.Value)
                };
            }
            else if (element is R4Model.Extension extension)
            {
                return new Extension
                {
                    Extension = Convert<Extension, R4Model.Extension>(extension.Extension),
                    Url = extension.Url,
                    Value = ConvertElement(extension.Value)
                };
            }
            else if (element is R4Model.UsageContext usageContext)
            {
                return new UsageContext
                {
                    Extension = Convert<Extension, R4Model.Extension>(usageContext.Extension),
                    Code = ConvertElement(usageContext.Code as R4Model.Element) as Coding,
                    Value = ConvertElement(usageContext.Value)
                };
            }
            else if (element is R4Model.Period period)
            {
                return new Period
                {
                    Extension = Convert<Extension, R4Model.Extension>(period.Extension),
                    Start = period.Start,
                    End = period.End,
                };
            }
            else if (element is R4Model.Markdown markdown)
            {
                return new Markdown
                {
                    Extension = Convert<Extension, R4Model.Extension>(markdown.Extension),
                    Value = markdown.Value
                };
            }
            else if (element is R4Model.Meta meta)
            {
                return new Meta
                {
                    Extension = Convert<Extension, R4Model.Extension>(meta.Extension),
                    LastUpdated = meta.LastUpdated,
                    Profile = meta.Profile,
                    Security = Convert<Coding, R4Model.Coding>(meta.Security),
                    Tag = Convert<Coding, R4Model.Coding>(meta.Tag),
                    VersionId = meta.VersionId
                };
            }
            else if (element is R4Model.Questionnaire.ItemComponent item)
            {
                return new Questionnaire.ItemComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(item.Extension),
                    ModifierExtension = Convert<Extension, R4Model.Extension>(item.ModifierExtension),
                    Code = Convert<Coding, R4Model.Coding>(item.Code),
                    Definition = item.Definition,
                    EnableWhen = ConvertEnableWhenComponent(item.EnableWhen),
                    Initial = ConvertInitalComponent(item.Initial),
                    Item = Convert<Questionnaire.ItemComponent, R4Model.Questionnaire.ItemComponent>(item.Item),
                    LinkId = item.LinkId,
                    MaxLength = item.MaxLength,
                    Option = Convert<Questionnaire.OptionComponent, R4Model.Questionnaire.AnswerOptionComponent>(item.AnswerOption),
                    Options = new ResourceReference(item.AnswerValueSet),
                    Prefix = item.Prefix,
                    ReadOnly = item.ReadOnly,
                    Repeats = item.Repeats,
                    Required = item.Required,
                    Text = item.Text,
                    Type = ConvertEnum<Questionnaire.QuestionnaireItemType, R4Model.Questionnaire.QuestionnaireItemType>(item.Type)
                };
            }
            else if (element is R4Model.Narrative narrative)
            {
                return new Narrative
                {
                    Extension = Convert<Extension, R4Model.Extension>(narrative.Extension),
                    Div = narrative.Div,
                    Status = ConvertEnum<Narrative.NarrativeStatus, R4Model.Narrative.NarrativeStatus>(narrative?.Status),
                };
            }
            else if (element is R4Model.Identifier identifier)
            {
                return new Identifier
                {
                    Extension = Convert<Extension, R4Model.Extension>(identifier.Extension),
                    Assigner = ConvertElement(identifier.Assigner) as ResourceReference,
                    Period = ConvertElement(identifier.Period) as Period,
                    System = identifier.System,
                    Type = ConvertElement(identifier.Type) as CodeableConcept,
                    Use = ConvertEnum<Identifier.IdentifierUse, R4Model.Identifier.IdentifierUse>(identifier.Use),
                    Value = identifier.Value
                };
            }
            else if (element is R4Model.CodeableConcept codeableConcept)
            {
                return new CodeableConcept
                {
                    Extension = Convert<Extension, R4Model.Extension>(codeableConcept.Extension),
                    Coding = Convert<Coding, R4Model.Coding>(codeableConcept.Coding),
                    Text = codeableConcept.Text
                };
            }
            else if (element is R4Model.ResourceReference reference)
            {
                return new ResourceReference
                {
                    Extension = Convert<Extension, R4Model.Extension>(reference.Extension),
                    Reference = reference.Reference,
                    Identifier = ConvertElement(reference.Identifier) as Identifier,
                    Display = reference.Display,
                    Url = reference.Url
                };
            }
            else if (element is R4Model.ContactDetail contactDetail)
            {
                return new ContactDetail
                {
                    Extension = Convert<Extension, R4Model.Extension>(contactDetail.Extension),
                    Name = contactDetail.Name,
                    Telecom = Convert<ContactPoint, R4Model.ContactPoint>(contactDetail.Telecom)
                };
            }
            else if (element is R4Model.ContactPoint contactPoint)
            {
                return new ContactPoint
                {
                    Extension = Convert<Extension, R4Model.Extension>(contactPoint.Extension),
                    Period = ConvertElement(contactPoint.Period) as Period,
                    Rank = contactPoint.Rank,
                    System = ConvertEnum<ContactPoint.ContactPointSystem, R4Model.ContactPoint.ContactPointSystem>(contactPoint.System),
                    Use = ConvertEnum<ContactPoint.ContactPointUse, R4Model.ContactPoint.ContactPointUse>(contactPoint.Use),
                    Value = contactPoint.Value
                };
            }
            else if (element is R4Model.ValueSet.ComposeComponent compose)
            {
                return new ValueSet.ComposeComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(compose.Extension),
                    ModifierExtension = Convert<Extension, R4Model.Extension>(compose.ModifierExtension),
                    Exclude = Convert<ValueSet.ConceptSetComponent, R4Model.ValueSet.ConceptSetComponent>(compose.Exclude),
                    Inactive = compose.Inactive,
                    Include = Convert<ValueSet.ConceptSetComponent, R4Model.ValueSet.ConceptSetComponent>(compose.Include),
                    LockedDate = compose.LockedDate
                };
            }
            else if (element is R4Model.ValueSet.ConceptSetComponent conceptSet)
            {
                return new ValueSet.ConceptSetComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(conceptSet.Extension),
                    ModifierExtension = Convert<Extension, R4Model.Extension>(conceptSet.ModifierExtension),
                    Concept = Convert<ValueSet.ConceptReferenceComponent, R4Model.ValueSet.ConceptReferenceComponent>(conceptSet.Concept),
                    Filter = Convert<ValueSet.FilterComponent, R4Model.ValueSet.FilterComponent>(conceptSet.Filter),
                    System = conceptSet.System,
                    ValueSet = conceptSet.ValueSet,
                    Version = conceptSet.Version
                };
            }
            else if (element is R4Model.ValueSet.ConceptReferenceComponent concept)
            {
                return new ValueSet.ConceptReferenceComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(concept.Extension),
                    ModifierExtension = Convert<Extension, R4Model.Extension>(concept.ModifierExtension),
                    Code = concept.Code,
                    Designation = Convert<ValueSet.DesignationComponent, R4Model.ValueSet.DesignationComponent>(concept.Designation),
                    Display = concept.Display
                };
            }
            else if (element is R4Model.ValueSet.FilterComponent filter)
            {
                return new ValueSet.FilterComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(filter.Extension),
                    ModifierExtension = Convert<Extension, R4Model.Extension>(filter.ModifierExtension),
                    Op = ConvertEnum<FilterOperator, R4Model.FilterOperator>(filter.Op),
                    Property = filter.Property,
                    Value = filter.Value
                };
            }
            else if (element is R4Model.ValueSet.DesignationComponent designation)
            {
                return new ValueSet.DesignationComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(designation.Extension),
                    ModifierExtension = Convert<Extension, R4Model.Extension>(designation.ModifierExtension),
                    Language = designation.Language,
                    Use = ConvertElement(designation.Use) as Coding,
                    Value = designation.Value
                };
            }
            else if (element is R4Model.ValueSet.ExpansionComponent expansion)
            {
                return new ValueSet.ExpansionComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(expansion.Extension),
                    ModifierExtension = Convert<Extension, R4Model.Extension>(expansion.ModifierExtension),
                    Contains = Convert<ValueSet.ContainsComponent, R4Model.ValueSet.ContainsComponent>(expansion.Contains),
                    Identifier = expansion.Identifier,
                    Offset = expansion.Offset,
                    Parameter = Convert<ValueSet.ParameterComponent, R4Model.ValueSet.ParameterComponent>(expansion.Parameter),
                    Timestamp = expansion.Timestamp,
                    Total = expansion.Total
                };
            }
            else if (element is R4Model.ValueSet.ContainsComponent contains)
            {
                return new ValueSet.ContainsComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(contains.Extension),
                    ModifierExtension = Convert<Extension, R4Model.Extension>(contains.ModifierExtension),
                    Abstract = contains.Abstract,
                    Code = contains.Code,
                    Contains = Convert<ValueSet.ContainsComponent, R4Model.ValueSet.ContainsComponent>(contains.Contains),
                    Designation = Convert<ValueSet.DesignationComponent, R4Model.ValueSet.DesignationComponent>(contains.Designation),
                    Display = contains.Display,
                    Inactive = contains.Inactive,
                    System = contains.System,
                    Version = contains.Version
                };
            }
            else if (element is R4Model.ValueSet.ParameterComponent parameter)
            {
                return new ValueSet.ParameterComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(parameter.Extension),
                    ModifierExtension = Convert<Extension, R4Model.Extension>(parameter.ModifierExtension),
                    Name = parameter.Name,
                    Value = ConvertElement(parameter.Value)
                };
            }

            return null;
        }
    }
}
