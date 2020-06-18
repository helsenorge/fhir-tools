extern alias R3;
extern alias R4;

using R3Model = R3::Hl7.Fhir.Model;
using Hl7.Fhir.Model;
using R4::Hl7.Fhir.Model;

using System.Collections.Generic;
using Hl7.Fhir.Utility;
using System;
using System.Linq;

namespace FhirTool.Conversion
{
    internal class FhirR3ToR4ConversionRoutines
    {
        internal TTo? ConvertEnum<TTo, TFrom>(TFrom? from)
            where TTo : struct, Enum
            where TFrom : struct, Enum
        {
            if (from == null) return default;

            return EnumUtility.ParseLiteral<TTo>(from.GetLiteral());
        }

        internal List<TTo> Convert<TTo, TFrom>(List<TFrom> items)
            where TTo : Base
            where TFrom : Base
        {
            var itemsR3 = new List<TTo>();
            foreach (var item in items)
                itemsR3.Add(ConvertElement(item) as TTo);
            return itemsR3;
        }

        internal Primitive<TTo> ConvertPrimitive<TTo, TFrom>(R3Model.Primitive<TFrom> element)
            where TTo : struct, Enum
            where TFrom : struct, Enum
        {
            if (element == null) return default;

            if (element is R3Model.Code<TFrom> code)
            {
                return new Code<TTo>
                {
                    Extension = Convert<Extension, R3Model.Extension>(code.Extension),
                    Value = ConvertEnum<TTo, TFrom>(code.Value)
                };
            }

            return default;
        }
        internal Primitive ConvertPrimitive(Base from, Type enumType)
        {
            if (from == null) return null;

            var innerFrom = (R3Model.Primitive)from;

            var targetType = (typeof(Code<>)).MakeGenericType(enumType);
            Primitive to = Activator.CreateInstance(targetType) as Primitive;
            to.Extension = Convert<Extension, R3Model.Extension>(innerFrom.Extension);
            to.ObjectValue = innerFrom.ObjectValue;
            
            return to;
        }

        internal Code<Money.Currencies> ConvertToCurrency(R3Model.Code code)
        {
            if (code == null) return null;

            return new Code<Money.Currencies>
            {
                Extension = Convert<Extension, R3Model.Extension>(code.Extension),
                Value = string.IsNullOrEmpty(code.Value) 
                ? null 
                : EnumUtility.ParseLiteral<Money.Currencies>(code.Value)
            };
        }

        internal FhirUrl ConvertToFhirUrl(R3Model.FhirUri uri)
        {
            if (uri == null) return null;

            return new FhirUrl
            {
                Extension = Convert<Extension, R3Model.Extension>(uri.Extension),
                Value = uri.Value
            };
        }

        internal SimpleQuantity ConvertToSimpleQuantity(R3Model.Quantity quantity)
        {
            if (quantity == null) return null;

            return new SimpleQuantity
            {
                Extension = Convert<Extension, R3Model.Extension>(quantity.Extension),
                Code = quantity.Code,
                System = quantity.System,
                Unit = quantity.Unit,
                Value = quantity.Value
            };
        }

        internal Questionnaire.InitialComponent ConvertToInitalComponent(R3Model.Element initial)
        {
            if (initial == null) return null;

            return new Questionnaire.InitialComponent
            {
                Extension = Convert<Extension, R3Model.Extension>(initial.Extension),
                Value = ConvertElement(initial)
            };
        }

        internal Canonical ConvertToCanonical(R3Model.FhirUri uri)
        {
            if (uri == null) return null;
            return new Canonical
            {
                Extension = Convert<Extension, R3Model.Extension>(uri.Extension),
                Value = uri.Value
            };
        }

        internal Canonical ConvertToCanonical(R3Model.FhirString str)
        {
            if (str == null) return null;
            return new Canonical
            {
                Extension = Convert<Extension, R3Model.Extension>(str.Extension),
                Value = str.Value
            };
        }

        internal Canonical ConvertToCanonical(R3Model.ResourceReference reference)
        {
            if (reference == null) return null;
            return new Canonical
            {
                Extension = Convert<Extension, R3Model.Extension>(reference.Extension),
                Value = reference.Reference
            };
        }

        internal Markdown ConvertToMarkdown(R3Model.FhirString str)
        {
            if (str == null) return null;
            return new Markdown
            {
                Extension = Convert<Extension, R3Model.Extension>(str.Extension),
                Value = str.Value
            };
        }

        internal FhirString ConvertToString(R3Model.Code code)
        {
            if (code == null) return null;
            return new FhirString
            {
                Extension = Convert<Extension, R3Model.Extension>(code.Extension),
                Value = code.Value
            };
        }

        internal ResourceReference ConvertToReference(R3Model.FhirUri uri)
        {
            if (uri == null) return null;
            return new ResourceReference
            {
                Extension = Convert<Extension, R3Model.Extension>(uri.Extension),
                Reference = uri.Value
            };
        }

        internal PositiveInt ConvertToPositiveInt(R3Model.Integer integer)
        {
            if (integer == null) return null;
            return new PositiveInt
            {
                Extension = Convert<Extension, R3Model.Extension>(integer.Extension),
                Value = integer.Value
            };
        }

        internal List<Questionnaire.EnableWhenComponent> ConvertEnableWhenComponent(List<R3Model.Questionnaire.EnableWhenComponent> enableWhenComponents)
        {
            var enableWhenComponentsR4 = new List<Questionnaire.EnableWhenComponent>();
            foreach (var enableWhen in enableWhenComponents)
            {
                var enableWhenR4 = new Questionnaire.EnableWhenComponent
                {
                    Extension = Convert<Extension, R3Model.Extension>(enableWhen.Extension),
                    ModifierExtension = Convert<Extension, R3Model.Extension>(enableWhen.ModifierExtension),
                    Question = enableWhen.Question
                };

                if (enableWhen.HasAnswer.HasValue)
                {
                    enableWhenR4.Operator = Questionnaire.QuestionnaireItemOperator.Exists;
                    enableWhenR4.Answer = new FhirBoolean(enableWhen.HasAnswer);
                }
                else if (enableWhen.Answer != null)
                {
                    enableWhenR4.Operator = Questionnaire.QuestionnaireItemOperator.Equal;
                    enableWhenR4.Answer = ConvertElement(enableWhen.Answer);
                }

                enableWhenComponentsR4.Add(enableWhenR4);
            }
            return enableWhenComponentsR4;
        }

        internal Element ConvertElement(Base element)
        {
            if (element == null) return null;

            if (element is R3Model.Instant instant)
            {
                return new Instant
                {
                    Extension = Convert<Extension, R3Model.Extension>(instant.Extension),
                    Value = instant.Value
                };
            }
            else if (element is R3Model.Time time)
            {
                return new Time
                {
                    Extension = Convert<Extension, R3Model.Extension>(time.Extension),
                    Value = time.Value
                };
            }
            else if (element is R3Model.Date date)
            {
                return new Date
                {
                    Extension = Convert<Extension, R3Model.Extension>(date.Extension),
                    Value = date.Value
                };
            }
            else if (element is R3Model.FhirDateTime dateTime)
            {
                return new FhirDateTime
                {
                    Extension = Convert<Extension, R3Model.Extension>(dateTime.Extension),
                    Value = dateTime.Value
                };
            }
            else if (element is R3Model.Base64Binary base64Binary)
            {
                return new Base64Binary
                {
                    Extension = Convert<Extension, R3Model.Extension>(base64Binary.Extension),
                    Value = base64Binary.Value
                };
            }
            else if (element is R3Model.FhirDecimal dec)
            {
                return new FhirDecimal
                {
                    Extension = Convert<Extension, R3Model.Extension>(dec.Extension),
                    Value = dec.Value
                };
            }
            else if (element is R3Model.FhirBoolean boolean)
            {
                return new FhirBoolean
                {
                    Extension = Convert<Extension, R3Model.Extension>(boolean.Extension),
                    Value = boolean.Value
                };
            }
            else if (element is R3Model.Code code)
            {
                return new Code
                {
                    Extension = Convert<Extension, R3Model.Extension>(code.Extension),
                    Value = code.Value
                };
            }
            else if (element is R3Model.FhirString str)
            {
                return new FhirString
                {
                    Extension = Convert<Extension, R3Model.Extension>(str.Extension),
                    Value = str.Value
                };
            }
            else if (element is R3Model.Integer integer)
            {
                return new Integer
                {
                    Extension = Convert<Extension, R3Model.Extension>(integer.Extension),
                    Value = integer.Value
                };
            }
            else if (element is R3Model.FhirUri uri)
            {
                return new FhirUri
                {
                    Extension = Convert<Extension, R3Model.Extension>(uri.Extension),
                    Value = uri.Value
                };
            }
            else if (element is R3Model.Id id)
            {
                return new Id
                {
                    Extension = Convert<Extension, R3Model.Extension>(id.Extension),
                    Value = id.Value
                };
            }
            else if (element is R3Model.Oid oid)
            {
                return new Oid
                {
                    Extension = Convert<Extension, R3Model.Extension>(oid.Extension),
                    Value = oid.Value
                };
            }
            else if (element is R3Model.Uuid uuid)
            {
                return new Uuid
                {
                    Extension = Convert<Extension, R3Model.Extension>(uuid.Extension),
                    Value = uuid.Value
                };
            }
            else if (element is R3Model.UnsignedInt unsignedInt)
            {
                return new UnsignedInt
                {
                    Extension = Convert<Extension, R3Model.Extension>(unsignedInt.Extension),
                    Value = unsignedInt.Value
                };
            }
            else if (element is R3Model.PositiveInt positiveInt)
            {
                return new PositiveInt
                {
                    Extension = Convert<Extension, R3Model.Extension>(positiveInt.Extension),
                    Value = positiveInt.Value
                };
            }
            else if (element is R3Model.Ratio ratio)
            {
                return new Ratio
                {
                    Extension = Convert<Extension, R3Model.Extension>(ratio.Extension),
                    Denominator = ConvertElement(ratio.Denominator) as Quantity,
                    Numerator = ConvertElement(ratio.Numerator) as Quantity
                };
            }
            else if (element is R3Model.Period period)
            {
                return new Period
                {
                    Extension = Convert<Extension, R3Model.Extension>(period.Extension),
                    StartElement = ConvertElement(period.StartElement) as FhirDateTime,
                    EndElement = ConvertElement(period.EndElement) as FhirDateTime
                };
            }
            else if (element is R3Model.Range range)
            {
                return new Range
                {
                    Extension = Convert<Extension, R3Model.Extension>(range.Extension),
                    High = ConvertToSimpleQuantity(range.High),
                    Low = ConvertToSimpleQuantity(range.Low),
                };
            }
            else if (element is R3Model.Attachment attachment)
            {
                return new Attachment
                {
                    Extension = Convert<Extension, R3Model.Extension>(attachment.Extension),
                    ContentTypeElement = ConvertElement(attachment.ContentTypeElement) as Code,
                    CreationElement = ConvertElement(attachment.CreationElement) as FhirDateTime,
                    DataElement = ConvertElement(attachment.DataElement) as Base64Binary,
                    HashElement = ConvertElement(attachment.HashElement) as Base64Binary,
                    LanguageElement = ConvertElement(attachment.LanguageElement) as Code,
                    SizeElement = ConvertElement(attachment.SizeElement) as UnsignedInt,
                    TitleElement = ConvertElement(attachment.TitleElement) as FhirString,
                    UrlElement = ConvertToFhirUrl(attachment.UrlElement)
                };
            }
            else if (element is R3Model.Identifier identifier)
            {
                return new Identifier
                {
                    Extension = Convert<Extension, R3Model.Extension>(identifier.Extension),
                    Assigner = ConvertElement(identifier.Assigner) as ResourceReference,
                    Period = ConvertElement(identifier.Period) as Period,
                    SystemElement = ConvertElement(identifier.SystemElement) as FhirUri,
                    Type = ConvertElement(identifier.Type) as CodeableConcept,
                    UseElement = ConvertPrimitive<Identifier.IdentifierUse, R3Model.Identifier.IdentifierUse>(identifier.UseElement) as Code<Identifier.IdentifierUse>,
                    ValueElement = ConvertElement(identifier.ValueElement) as FhirString
                };
            }
            else if (element is R3Model.Annotation annotation)
            {
                return new Annotation
                {
                    Extension = Convert<Extension, R3Model.Extension>(annotation.Extension),
                    Author = ConvertElement(annotation.Author),
                    Text = ConvertToMarkdown(annotation.TextElement),
                    TimeElement = ConvertElement(annotation.TimeElement) as FhirDateTime
                };
            }
            else if (element is R3Model.HumanName humanName)
            {
                return new HumanName
                {
                    Extension = Convert<Extension, R3Model.Extension>(humanName.Extension),
                    FamilyElement = ConvertElement(humanName.FamilyElement) as FhirString,
                    GivenElement = Convert<FhirString, R3Model.FhirString>(humanName.GivenElement),
                    Period = ConvertElement(humanName.Period) as Period,
                    PrefixElement = Convert<FhirString, R3Model.FhirString>(humanName.PrefixElement),
                    SuffixElement = Convert<FhirString, R3Model.FhirString>(humanName.SuffixElement),
                    TextElement = ConvertElement(humanName.TextElement) as FhirString,
                    UseElement = ConvertPrimitive<HumanName.NameUse, R3Model.HumanName.NameUse>(humanName.UseElement) as Code<HumanName.NameUse>
                };
            }
            else if (element is R3Model.CodeableConcept codeableConcept)
            {
                return new CodeableConcept
                {
                    Extension = Convert<Extension, R3Model.Extension>(codeableConcept.Extension),
                    Coding = Convert<Coding, R3Model.Coding>(codeableConcept.Coding),
                    TextElement = ConvertElement(codeableConcept.TextElement) as FhirString
                };
            }
            else if (element is R3Model.ContactPoint contactPoint)
            {
                return new ContactPoint
                {
                    Extension = Convert<Extension, R3Model.Extension>(contactPoint.Extension),
                    Period = ConvertElement(contactPoint.Period) as Period,
                    RankElement = ConvertElement(contactPoint.RankElement) as PositiveInt,
                    SystemElement = ConvertPrimitive<ContactPoint.ContactPointSystem, R3Model.ContactPoint.ContactPointSystem>(contactPoint.SystemElement) as Code<ContactPoint.ContactPointSystem>,
                    UseElement = ConvertPrimitive<ContactPoint.ContactPointUse, R3Model.ContactPoint.ContactPointUse>(contactPoint.UseElement) as Code<ContactPoint.ContactPointUse>,
                    ValueElement = ConvertElement(contactPoint.ValueElement) as FhirString
                };
            }
            else if (element is R3Model.Coding coding)
            {
                return new Coding
                {
                    Extension = Convert<Extension, R3Model.Extension>(coding.Extension),
                    CodeElement = ConvertElement(coding.CodeElement) as Code,
                    DisplayElement = ConvertElement(coding.DisplayElement) as FhirString,
                    SystemElement = ConvertElement(coding.SystemElement) as FhirUri,
                    UserSelectedElement = ConvertElement(coding.UserSelectedElement) as FhirBoolean,
                    VersionElement = ConvertElement(coding.VersionElement) as FhirString
                };
            }
            else if (element is R3Model.Money money)
            {
                return new Money
                {
                    CurrencyElement = ConvertToCurrency(money.CodeElement),
                    ValueElement = ConvertElement(money.ValueElement) as FhirDecimal
                };
            }
            else if (element is R3Model.Address address)
            {
                return new Address
                {
                    Extension = Convert<Extension, R3Model.Extension>(address.Extension),
                    CityElement = ConvertElement(address.CityElement) as FhirString,
                    CountryElement = ConvertElement(address.CountryElement) as FhirString,
                    DistrictElement = ConvertElement(address.DistrictElement) as FhirString,
                    LineElement = Convert<FhirString, R3Model.FhirString>(address.LineElement),
                    Period = ConvertElement(address.Period) as Period,
                    PostalCodeElement = ConvertElement(address.PostalCodeElement) as FhirString,
                    StateElement = ConvertElement(address.StateElement) as FhirString,
                    TextElement = ConvertElement(address.TextElement) as FhirString,
                    TypeElement = ConvertPrimitive<Address.AddressType, R3Model.Address.AddressType>(address.TypeElement) as Code<Address.AddressType>,
                    UseElement = ConvertPrimitive<Address.AddressUse, R3Model.Address.AddressUse>(address.UseElement) as Code<Address.AddressUse>
                };
            }
            else if (element is R3Model.Timing timing)
            {
                return new Timing
                {
                    EventElement = Convert<FhirDateTime, R3Model.FhirDateTime>(timing.EventElement),
                    Repeat = ConvertElement(timing.Repeat) as Timing.RepeatComponent,
                    Code = ConvertElement(timing.Code) as CodeableConcept
                };
            }
            else if (element is R3Model.Timing.RepeatComponent repeatComponent)
            {
                return new Timing.RepeatComponent
                {
                    Extension = Convert<Extension, R3Model.Extension>(repeatComponent.Extension),
                    Bounds = ConvertElement(repeatComponent.Bounds),
                    CountElement = ConvertToPositiveInt(repeatComponent.CountElement),
                    CountMaxElement = ConvertToPositiveInt(repeatComponent.CountMaxElement),
                    DurationElement = ConvertElement(repeatComponent.DurationElement) as FhirDecimal,
                    DurationMaxElement = ConvertElement(repeatComponent.DurationMaxElement) as FhirDecimal,
                    DurationUnitElement = ConvertPrimitive<Timing.UnitsOfTime, R3Model.Timing.UnitsOfTime>(repeatComponent.DurationUnitElement) as Code<Timing.UnitsOfTime>,
                    FrequencyElement = ConvertToPositiveInt(repeatComponent.FrequencyElement),
                    FrequencyMaxElement = ConvertToPositiveInt(repeatComponent.FrequencyMaxElement),
                    PeriodElement = ConvertElement(repeatComponent.PeriodElement) as FhirDecimal,
                    PeriodMaxElement = ConvertElement(repeatComponent.PeriodMaxElement) as FhirDecimal,
                    PeriodUnitElement = ConvertPrimitive<Timing.UnitsOfTime, R3Model.Timing.UnitsOfTime>(repeatComponent.PeriodUnitElement) as Code<Timing.UnitsOfTime>,
                    DayOfWeekElement = repeatComponent.DayOfWeekElement.Select(d => ConvertPrimitive<DaysOfWeek, R3Model.DaysOfWeek>(d) as Code<DaysOfWeek>).ToList(),
                    TimeOfDayElement = Convert<Time, R3Model.Time>(repeatComponent.TimeOfDayElement),
                    WhenElement = repeatComponent.WhenElement.Select(w => ConvertPrimitive<Timing.EventTiming, R3Model.Timing.EventTiming>(w) as Code<Timing.EventTiming>).ToList(),
                    OffsetElement = ConvertElement(repeatComponent.OffsetElement) as UnsignedInt
                };
            }
            else if (element is R3Model.Quantity quantity)
            {
                return new Quantity
                {
                    Extension = Convert<Extension, R3Model.Extension>(quantity.Extension),
                    CodeElement = ConvertElement(quantity.CodeElement) as Code,
                    ComparatorElement = ConvertPrimitive<Quantity.QuantityComparator, R3Model.Quantity.QuantityComparator>(quantity.ComparatorElement) as Code<Quantity.QuantityComparator>,
                    SystemElement = ConvertElement(quantity.SystemElement) as FhirUri,
                    UnitElement = ConvertElement(quantity.UnitElement) as FhirString,
                    ValueElement = ConvertElement(quantity.ValueElement) as FhirDecimal
                };
            }
            else if (element is R3Model.SampledData sampledData)
            {
                return new SampledData
                {
                    Extension = Convert<Extension, R3Model.Extension>(sampledData.Extension),
                    Origin = ConvertElement(sampledData.Origin) as SimpleQuantity,
                    PeriodElement = ConvertElement(sampledData.PeriodElement) as FhirDecimal,
                    FactorElement = ConvertElement(sampledData.FactorElement) as FhirDecimal,
                    LowerLimitElement = ConvertElement(sampledData.LowerLimitElement) as FhirDecimal,
                    UpperLimitElement = ConvertElement(sampledData.UpperLimitElement) as FhirDecimal,
                    DimensionsElement = ConvertElement(sampledData.DimensionsElement) as PositiveInt,
                    DataElement = ConvertElement(sampledData.DataElement) as FhirString
                };
            }
            else if (element is R3Model.Signature signature)
            {
                var sig = new Signature
                {
                    Type = Convert<Coding, R3Model.Coding>(signature.Type),
                    WhenElement = ConvertElement(signature.WhenElement) as Instant,
                    SigFormatElement = ConvertElement(signature.ContentTypeElement) as Code,
                    DataElement = ConvertElement(signature.BlobElement) as Base64Binary
                };
                if (signature.Who != null)
                {
                    if (signature.Who is R3Model.ResourceReference whoReference)
                    {
                        sig.Who = ConvertElement(whoReference) as ResourceReference;
                    }
                    else if (signature.Who is R3Model.FhirUri whoUri)
                    {
                        sig.Who = ConvertToReference(whoUri);
                    }
                }
                if (signature.OnBehalfOf != null)
                {
                    if (signature.OnBehalfOf is R3Model.ResourceReference onBehalfOfReference)
                    {
                        sig.Who = ConvertElement(onBehalfOfReference) as ResourceReference;
                    }
                    else if (signature.OnBehalfOf is R3Model.FhirUri onBehalfOfUri)
                    {
                        sig.Who = ConvertToReference(onBehalfOfUri);
                    }
                }
            }
            else if (element is R3Model.Age age)
            {
                return new Age
                {
                    Extension = Convert<Extension, R3Model.Extension>(age.Extension),
                    CodeElement = ConvertElement(age.CodeElement) as Code,
                    ComparatorElement = ConvertPrimitive<Quantity.QuantityComparator, R3Model.Quantity.QuantityComparator>(age.ComparatorElement) as Code<Quantity.QuantityComparator>,
                    SystemElement = ConvertElement(age.SystemElement) as FhirUri,
                    UnitElement = ConvertElement(age.UnitElement) as FhirString,
                    ValueElement = ConvertElement(age.ValueElement) as FhirDecimal
                };
            }
            else if (element is R3Model.Distance distance)
            {
                return new Distance
                {
                    Extension = Convert<Extension, R3Model.Extension>(distance.Extension),
                    CodeElement = ConvertElement(distance.CodeElement) as Code,
                    ComparatorElement = ConvertPrimitive<Quantity.QuantityComparator, R3Model.Quantity.QuantityComparator>(distance.ComparatorElement) as Code<Quantity.QuantityComparator>,
                    SystemElement = ConvertElement(distance.SystemElement) as FhirUri,
                    UnitElement = ConvertElement(distance.UnitElement) as FhirString,
                    ValueElement = ConvertElement(distance.ValueElement) as FhirDecimal
                };
            }
            else if (element is R3Model.Duration duration)
            {
                return new Duration
                {
                    Extension = Convert<Extension, R3Model.Extension>(duration.Extension),
                    CodeElement = ConvertElement(duration.CodeElement) as Code,
                    ComparatorElement = ConvertPrimitive<Quantity.QuantityComparator, R3Model.Quantity.QuantityComparator>(duration.ComparatorElement) as Code<Quantity.QuantityComparator>,
                    SystemElement = ConvertElement(duration.SystemElement) as FhirUri,
                    UnitElement = ConvertElement(duration.UnitElement) as FhirString,
                    ValueElement = ConvertElement(duration.ValueElement) as FhirDecimal
                };
            }
            else if (element is R3Model.Count count)
            {
                return new Count
                {
                    Extension = Convert<Extension, R3Model.Extension>(count.Extension),
                    CodeElement = ConvertElement(count.CodeElement) as Code,
                    ComparatorElement = ConvertPrimitive<Quantity.QuantityComparator, R3Model.Quantity.QuantityComparator>(count.ComparatorElement) as Code<Quantity.QuantityComparator>,
                    SystemElement = ConvertElement(count.SystemElement) as FhirUri,
                    UnitElement = ConvertElement(count.UnitElement) as FhirString,
                    ValueElement = ConvertElement(count.ValueElement) as FhirDecimal
                };
            }
            else if (element is R3Model.SimpleQuantity simpleQuantity)
            {
                return new SimpleQuantity
                {
                    Extension = Convert<Extension, R3Model.Extension>(simpleQuantity.Extension),
                    CodeElement = ConvertElement(simpleQuantity.CodeElement) as Code,
                    ComparatorElement = ConvertPrimitive<Quantity.QuantityComparator, R3Model.Quantity.QuantityComparator>(simpleQuantity.ComparatorElement) as Code<Quantity.QuantityComparator>,
                    SystemElement = ConvertElement(simpleQuantity.SystemElement) as FhirUri,
                    UnitElement = ConvertElement(simpleQuantity.UnitElement) as FhirString,
                    ValueElement = ConvertElement(simpleQuantity.ValueElement) as FhirDecimal
                };
            }
            else if (element is R3Model.ContactDetail contactDetail)
            {
                return new ContactDetail
                {
                    Extension = Convert<Extension, R3Model.Extension>(contactDetail.Extension),
                    NameElement = ConvertElement(contactDetail.NameElement) as FhirString,
                    Telecom = Convert<ContactPoint, R3Model.ContactPoint>(contactDetail.Telecom)
                };
            }
            else if (element is R3Model.Contributor contributor)
            {
                return new Contributor
                {
                    Extension = Convert<Extension, R3Model.Extension>(contributor.Extension),
                    TypeElement = ConvertPrimitive<Contributor.ContributorType, R3Model.Contributor.ContributorType>(contributor.TypeElement) as Code<Contributor.ContributorType>,
                    NameElement = ConvertElement(contributor.NameElement) as FhirString,
                    Contact = Convert<ContactDetail, R3Model.ContactDetail>(contributor.Contact)
                };
            }
            else if (element is R3Model.DataRequirement dataRequirement)
            {
                return new DataRequirement
                {
                    Extension = Convert<Extension, R3Model.Extension>(dataRequirement.Extension),
                    TypeElement = ConvertPrimitive<FHIRAllTypes, R3Model.FHIRAllTypes>(dataRequirement.TypeElement) as Code<FHIRAllTypes>,
                    ProfileElement = dataRequirement.ProfileElement.Select(p => ConvertToCanonical(p)) as List<Canonical>,
                    MustSupportElement = Convert<FhirString, R3Model.FhirString>(dataRequirement.MustSupportElement),
                    CodeFilter = Convert<DataRequirement.CodeFilterComponent, R3Model.DataRequirement.CodeFilterComponent>(dataRequirement.CodeFilter),
                    DateFilter = Convert<DataRequirement.DateFilterComponent, R3Model.DataRequirement.DateFilterComponent>(dataRequirement.DateFilter)
                };
            }
            else if (element is R3Model.DataRequirement.CodeFilterComponent codeFilter)
            {
                var cf = new DataRequirement.CodeFilterComponent
                {
                    Extension = Convert<Extension, R3Model.Extension>(codeFilter.Extension),
                    PathElement = ConvertElement(codeFilter.PathElement) as FhirString,
                    Code = Convert<Coding, R3Model.Coding>(codeFilter.ValueCoding),
                };
                if (codeFilter.ValueSet is R3Model.ResourceReference valueSetRef)
                {
                    cf.ValueSetElement = ConvertToCanonical(valueSetRef);
                }
                else if (codeFilter.ValueSet is R3Model.FhirString valueSetString)
                {
                    cf.ValueSetElement = ConvertToCanonical(valueSetString);
                }
                return cf;
            }
            else if (element is R3Model.DataRequirement.DateFilterComponent dateFilter)
            {
                return new DataRequirement.DateFilterComponent
                {
                    Extension = Convert<Extension, R3Model.Extension>(dateFilter.Extension),
                    PathElement = ConvertElement(dateFilter.PathElement) as FhirString,
                    Value = ConvertElement(dateFilter.Value)
                };
            }
            else if (element is R3Model.RelatedArtifact relatedArtifact)
            {
                return new RelatedArtifact
                {
                    Extension = Convert<Extension, R3Model.Extension>(relatedArtifact.Extension),
                    TypeElement = ConvertPrimitive<RelatedArtifact.RelatedArtifactType, R3Model.RelatedArtifact.RelatedArtifactType>(relatedArtifact.TypeElement) as Code<RelatedArtifact.RelatedArtifactType>,
                    DisplayElement = ConvertElement(relatedArtifact.DisplayElement) as FhirString,
                    Citation = ConvertToMarkdown(relatedArtifact.CitationElement),
                    UrlElement = ConvertToFhirUrl(relatedArtifact.UrlElement) as FhirUrl,
                    Document = ConvertElement(relatedArtifact.Document) as Attachment,
                    ResourceElement = ConvertToCanonical(relatedArtifact.Resource)
                };
            }
            else if (element is R3Model.UsageContext usageContext)
            {
                return new UsageContext
                {
                    Extension = Convert<Extension, R3Model.Extension>(usageContext.Extension),
                    Code = ConvertElement(usageContext.Code) as Coding,
                    Value = ConvertElement(usageContext.Value)
                };
            }
            else if (element is R3Model.ParameterDefinition parameterDefinition)
            {
                return new ParameterDefinition
                {
                    Extension = Convert<Extension, R3Model.Extension>(parameterDefinition.Extension),
                    NameElement = ConvertElement(parameterDefinition.NameElement) as Code,
                    UseElement = ConvertPrimitive<OperationParameterUse, R3Model.OperationParameterUse>(parameterDefinition.UseElement) as Code<OperationParameterUse>,
                    MinElement = ConvertElement(parameterDefinition.MinElement) as Integer,
                    MaxElement = ConvertElement(parameterDefinition.MaxElement) as FhirString,
                    DocumentationElement = ConvertElement(parameterDefinition.DocumentationElement) as FhirString,
                    TypeElement = ConvertPrimitive<FHIRAllTypes, R3Model.FHIRAllTypes>(parameterDefinition.TypeElement) as Code<FHIRAllTypes>,
                    ProfileElement = ConvertToCanonical(parameterDefinition.Profile)
                };
            }
            else if (element is R3Model.TriggerDefinition triggerDefinition)
            {
                return new TriggerDefinition
                {
                    Extension = Convert<Extension, R3Model.Extension>(triggerDefinition.Extension),
                    TypeElement = ConvertPrimitive<TriggerDefinition.TriggerType, R3Model.TriggerDefinition.TriggerType>(triggerDefinition.TypeElement) as Code<TriggerDefinition.TriggerType>,
                    NameElement = ConvertElement(triggerDefinition.EventNameElement) as FhirString,
                    Timing = ConvertElement(triggerDefinition.EventTiming),
                    Data = new List<DataRequirement> { ConvertElement(triggerDefinition.EventData) as DataRequirement }
                };
            }
            else if (element is R3Model.ResourceReference reference)
            {
                return new ResourceReference
                {
                    Extension = Convert<Extension, R3Model.Extension>(reference.Extension),
                    DisplayElement = ConvertElement(reference.DisplayElement) as FhirString,
                    Identifier = ConvertElement(reference.Identifier) as Identifier,
                    ReferenceElement = ConvertElement(reference.ReferenceElement) as FhirString
                };
            }
            else if (element is R3Model.Meta meta)
            {
                return new Meta
                {
                    Extension = Convert<Extension, R3Model.Extension>(meta.Extension),
                    LastUpdatedElement = ConvertElement(meta.LastUpdatedElement) as Instant,
                    ProfileElement = meta.ProfileElement.Select(p => ConvertToCanonical(p)) as List<Canonical>,
                    Security = Convert<Coding, R3Model.Coding>(meta.Security),
                    Tag = Convert<Coding, R3Model.Coding>(meta.Tag),
                    VersionIdElement = ConvertElement(meta.VersionIdElement) as Id
                };
            }
            else if (element is R3Model.Dosage dosage)
            {
                var dos = new Dosage
                {
                    Extension = Convert<Extension, R3Model.Extension>(dosage.Extension),
                    SequenceElement = ConvertElement(dosage.SequenceElement) as Integer,
                    TextElement = ConvertElement(dosage.TextElement) as FhirString,
                    AdditionalInstruction = Convert<CodeableConcept, R3Model.CodeableConcept>(dosage.AdditionalInstruction),
                    PatientInstructionElement = ConvertElement(dosage.PatientInstructionElement) as FhirString,
                    Timing = ConvertElement(dosage.Timing) as Timing,
                    AsNeeded = ConvertElement(dosage.AsNeeded),
                    Site = ConvertElement(dosage.Site) as CodeableConcept,
                    Route = ConvertElement(dosage.Route) as CodeableConcept,
                    Method = ConvertElement(dosage.Method) as CodeableConcept,
                    MaxDosePerPeriod = ConvertElement(dosage.MaxDosePerPeriod) as Ratio,
                    MaxDosePerAdministration = ConvertElement(dosage.MaxDosePerAdministration) as SimpleQuantity,
                    MaxDosePerLifetime = ConvertElement(dosage.MaxDosePerLifetime) as SimpleQuantity
                };
                if(dosage.Dose != null)
                {
                    dos.DoseAndRate.Add(new Dosage.DoseAndRateComponent { Dose = ConvertElement(dosage.Dose) });
                }
                if(dosage.Rate != null)
                {
                    dos.DoseAndRate.Add(new Dosage.DoseAndRateComponent { Rate = ConvertElement(dosage.Rate) });
                }
            }
            else if (element is R3Model.XHtml xhtml)
            {
                return new XHtml
                {
                    Extension = Convert<Extension, R3Model.Extension>(xhtml.Extension),
                    Value = xhtml.Value
                };
            }
            else if (element is R3Model.Narrative narrative)
            {
                return new Narrative
                {
                    Extension = Convert<Extension, R3Model.Extension>(narrative.Extension),
                    Div = narrative.Div,
                    StatusElement = ConvertPrimitive<Narrative.NarrativeStatus, R3Model.Narrative.NarrativeStatus>(narrative?.StatusElement) as Code<Narrative.NarrativeStatus>
                };
            }
            else if (element is R3Model.ElementDefinition elementDefinition)
            {
                return new ElementDefinition
                {
                    Extension = Convert<Extension, R3Model.Extension>(elementDefinition.Extension),
                    PathElement = ConvertElement(elementDefinition.PathElement) as FhirString,
                    RepresentationElement = elementDefinition.RepresentationElement.Select(r => ConvertPrimitive<ElementDefinition.PropertyRepresentation, R3Model.ElementDefinition.PropertyRepresentation>(r) as Code<ElementDefinition.PropertyRepresentation>).ToList(),
                    SliceNameElement = ConvertElement(elementDefinition.SliceNameElement) as FhirString,
                    LabelElement = ConvertElement(elementDefinition.LabelElement) as FhirString,
                    Code = Convert<Coding, R3Model.Coding>(elementDefinition.Code),
                    Slicing = ConvertElement(elementDefinition.Slicing) as ElementDefinition.SlicingComponent,
                    ShortElement = ConvertElement(elementDefinition.ShortElement) as FhirString,
                    Definition = ConvertElement(elementDefinition.DefinitionElement) as Markdown,
                    Comment = ConvertElement(elementDefinition.CommentElement) as Markdown,
                    Requirements = ConvertElement(elementDefinition.RequirementsElement) as Markdown,
                    AliasElement = Convert<FhirString, R3Model.FhirString>(elementDefinition.AliasElement),
                    MinElement = ConvertElement(elementDefinition.MinElement) as UnsignedInt,
                    MaxElement = ConvertElement(elementDefinition.MaxElement) as FhirString,
                    Base = ConvertElement(elementDefinition.Base) as ElementDefinition.BaseComponent,
                    ContentReferenceElement = ConvertElement(elementDefinition.ContentReferenceElement) as FhirUri,
                    Type = Convert<ElementDefinition.TypeRefComponent, R3Model.ElementDefinition.TypeRefComponent>(elementDefinition.Type),
                    DefaultValue = ConvertElement(elementDefinition.DefaultValue),
                    MeaningWhenMissing = ConvertElement(elementDefinition.MeaningWhenMissingElement) as Markdown,
                    OrderMeaningElement = ConvertElement(elementDefinition.OrderMeaningElement) as FhirString,
                    Fixed = ConvertElement(elementDefinition.Fixed),
                    Pattern = ConvertElement(elementDefinition.Pattern),
                    Example = Convert<ElementDefinition.ExampleComponent, R3Model.ElementDefinition.ExampleComponent>(elementDefinition.Example),
                    MinValue = ConvertElement(elementDefinition.MinValue),
                    MaxValue = ConvertElement(elementDefinition.MaxValue),
                    MaxLengthElement = ConvertElement(elementDefinition.MaxLengthElement) as Integer,
                    ConditionElement = Convert<Id, R3Model.Id>(elementDefinition.ConditionElement),
                    Constraint = Convert<ElementDefinition.ConstraintComponent, R3Model.ElementDefinition.ConstraintComponent>(elementDefinition.Constraint),
                    MustSupportElement = ConvertElement(elementDefinition.MustSupportElement) as FhirBoolean,
                    IsModifierElement = ConvertElement(elementDefinition.IsModifierElement) as FhirBoolean,
                    IsSummaryElement = ConvertElement(elementDefinition.IsSummaryElement) as FhirBoolean,
                    Binding = ConvertElement(elementDefinition.Binding) as ElementDefinition.ElementDefinitionBindingComponent,
                    Mapping = Convert<ElementDefinition.MappingComponent, R3Model.ElementDefinition.MappingComponent>(elementDefinition.Mapping)
                };
            }
            else if (element is R3Model.ElementDefinition.SlicingComponent slicing)
            {
                return new ElementDefinition.SlicingComponent
                {
                    Extension = Convert<Extension, R3Model.Extension>(slicing.Extension),
                    Discriminator = Convert<ElementDefinition.DiscriminatorComponent, R3Model.ElementDefinition.DiscriminatorComponent>(slicing.Discriminator),
                    DescriptionElement = ConvertElement(slicing.DescriptionElement) as FhirString,
                    OrderedElement = ConvertElement(slicing.OrderedElement) as FhirBoolean,
                    RulesElement = ConvertPrimitive<ElementDefinition.SlicingRules, R3Model.ElementDefinition.SlicingRules>(slicing.RulesElement) as Code<ElementDefinition.SlicingRules>
                };
            }
            else if (element is R3Model.ElementDefinition.DiscriminatorComponent discriminator)
            {
                return new ElementDefinition.DiscriminatorComponent
                {
                    Extension = Convert<Extension, R3Model.Extension>(discriminator.Extension),
                    TypeElement = ConvertPrimitive<ElementDefinition.DiscriminatorType, R3Model.ElementDefinition.DiscriminatorType>(discriminator.TypeElement) as Code<ElementDefinition.DiscriminatorType>,
                    PathElement = ConvertElement(discriminator.PathElement) as FhirString
                };
            }
            else if (element is R3Model.ElementDefinition.BaseComponent baseComponent)
            {
                return new ElementDefinition.BaseComponent
                {
                    Extension = Convert<Extension, R3Model.Extension>(baseComponent.Extension),
                    PathElement = ConvertElement(baseComponent.PathElement) as FhirString,
                    MinElement = ConvertElement(baseComponent.MinElement) as UnsignedInt,
                    MaxElement = ConvertElement(baseComponent.MaxElement) as FhirString
                };
            }
            else if (element is R3Model.ElementDefinition.TypeRefComponent typeRef)
            {
                return new ElementDefinition.TypeRefComponent
                {
                    Extension = Convert<Extension, R3Model.Extension>(typeRef.Extension),
                    CodeElement = ConvertElement(typeRef.CodeElement) as FhirUri,
                    ProfileElement = typeRef.ProfileElement == null ? null : new List<Canonical> { ConvertToCanonical(typeRef.ProfileElement) },
                    TargetProfileElement = typeRef.TargetProfileElement == null ? null : new List<Canonical> { ConvertToCanonical(typeRef.TargetProfileElement) },
                    AggregationElement = typeRef.AggregationElement.Select(a => ConvertPrimitive<ElementDefinition.AggregationMode, R3Model.ElementDefinition.AggregationMode>(a) as Code<ElementDefinition.AggregationMode>).ToList(),
                    VersioningElement = ConvertPrimitive<ElementDefinition.ReferenceVersionRules, R3Model.ElementDefinition.ReferenceVersionRules>(typeRef.VersioningElement) as Code<ElementDefinition.ReferenceVersionRules>
                };
            }
            else if (element is R3Model.ElementDefinition.ExampleComponent example)
            {
                return new ElementDefinition.ExampleComponent
                {
                    Extension = Convert<Extension, R3Model.Extension>(example.Extension),
                    LabelElement = ConvertElement(example.LabelElement) as FhirString,
                    Value = ConvertElement(example.Value)
                };
            }
            else if (element is R3Model.ElementDefinition.ConstraintComponent constraint)
            {
                return new ElementDefinition.ConstraintComponent
                {
                    Extension = Convert<Extension, R3Model.Extension>(constraint.Extension),
                    KeyElement = ConvertElement(constraint.KeyElement) as Id,
                    RequirementsElement = ConvertElement(constraint.RequirementsElement) as FhirString,
                    SeverityElement = ConvertPrimitive<ElementDefinition.ConstraintSeverity, R3Model.ElementDefinition.ConstraintSeverity>(constraint.SeverityElement) as Code<ElementDefinition.ConstraintSeverity>,
                    HumanElement = ConvertElement(constraint.HumanElement) as FhirString,
                    ExpressionElement = ConvertElement(constraint.ExpressionElement) as FhirString,
                    XpathElement = ConvertElement(constraint.XpathElement) as FhirString,
                    SourceElement = ConvertToCanonical(constraint.SourceElement)
                };
            }
            else if (element is R3Model.ElementDefinition.ElementDefinitionBindingComponent binding)
            {
                var bindingComponent = new ElementDefinition.ElementDefinitionBindingComponent
                {
                    Extension = Convert<Extension, R3Model.Extension>(binding.Extension),
                    StrengthElement = ConvertPrimitive<BindingStrength, R3Model.BindingStrength>(binding.StrengthElement) as Code<BindingStrength>,
                    DescriptionElement = ConvertElement(binding.DescriptionElement) as FhirString
                };
                if(binding.ValueSet is R3Model.FhirUri bindingValueSetUri)
                {
                    bindingComponent.ValueSetElement = ConvertToCanonical(bindingValueSetUri);
                }
                else if (binding.ValueSet is R3Model.ResourceReference bindingValueSetReference)
                {
                    bindingComponent.ValueSetElement = ConvertToCanonical(bindingValueSetReference);
                }
                return bindingComponent;
            }
            else if (element is R3Model.ElementDefinition.MappingComponent mapping)
            {
                return new ElementDefinition.MappingComponent
                {
                    Extension = Convert<Extension, R3Model.Extension>(mapping.Extension),
                    IdentityElement = ConvertElement(mapping.IdentityElement) as Id,
                    LanguageElement = ConvertElement(mapping.LanguageElement) as Code,
                    MapElement = ConvertElement(mapping.MapElement) as FhirString,
                    CommentElement = ConvertElement(mapping.CommentElement) as FhirString
                };
            }
            else if (element is R3Model.Questionnaire.OptionComponent option)
            {
                return new Questionnaire.AnswerOptionComponent
                {
                    Extension = Convert<Extension, R3Model.Extension>(option.Extension),
                    ModifierExtension = Convert<Extension, R3Model.Extension>(option.ModifierExtension),
                    Value = ConvertElement(option.Value)
                };
            }
            else if (element is R3Model.Extension extension)
            {
                return new Extension
                {
                    Extension = Convert<Extension, R3Model.Extension>(extension.Extension),
                    Url = extension.Url,
                    Value = ConvertElement(extension.Value)
                };
            }
            else if (element is R3Model.Markdown markdown)
            {
                return new Markdown
                {
                    Extension = Convert<Extension, R3Model.Extension>(markdown.Extension),
                    Value = markdown.Value
                };
            }
            else if (element is R3Model.Questionnaire.ItemComponent item)
            {
                return new Questionnaire.ItemComponent
                {
                    Extension = Convert<Extension, R3Model.Extension>(item.Extension),
                    ModifierExtension = Convert<Extension, R3Model.Extension>(item.ModifierExtension),
                    Code = Convert<Coding, R3Model.Coding>(item.Code),
                    DefinitionElement = ConvertElement(item.DefinitionElement) as FhirUri,
                    EnableWhen = ConvertEnableWhenComponent(item.EnableWhen),
                    Initial = new List<Questionnaire.InitialComponent> { ConvertToInitalComponent(item.Initial) },
                    Item = Convert<Questionnaire.ItemComponent, R3Model.Questionnaire.ItemComponent>(item.Item),
                    LinkIdElement = ConvertElement(item.LinkIdElement) as FhirString,
                    MaxLengthElement = ConvertElement(item.MaxLengthElement) as Integer,
                    AnswerOption = Convert<Questionnaire.AnswerOptionComponent, R3Model.Questionnaire.OptionComponent>(item.Option),
                    AnswerValueSetElement = ConvertToCanonical(item.Options?.ReferenceElement),
                    PrefixElement = ConvertElement(item.PrefixElement) as FhirString,
                    ReadOnlyElement = ConvertElement(item.ReadOnlyElement) as FhirBoolean,
                    RepeatsElement = ConvertElement(item.RepeatsElement) as FhirBoolean,
                    RequiredElement = ConvertElement(item.RequiredElement) as FhirBoolean,
                    TextElement = ConvertElement(item.TextElement) as FhirString,
                    TypeElement = ConvertPrimitive<Questionnaire.QuestionnaireItemType, R3Model.Questionnaire.QuestionnaireItemType>(item.TypeElement) as Code<Questionnaire.QuestionnaireItemType>
                };
            }
            else if (element is R3Model.ValueSet.ComposeComponent compose)
            {
                return new ValueSet.ComposeComponent
                {
                    Extension = Convert<Extension, R3Model.Extension>(compose.Extension),
                    ModifierExtension = Convert<Extension, R3Model.Extension>(compose.ModifierExtension),
                    Exclude = Convert<ValueSet.ConceptSetComponent, R3Model.ValueSet.ConceptSetComponent>(compose.Exclude),
                    InactiveElement = ConvertElement(compose.InactiveElement) as FhirBoolean,
                    Include = Convert<ValueSet.ConceptSetComponent, R3Model.ValueSet.ConceptSetComponent>(compose.Include),
                    LockedDateElement = ConvertElement(compose.LockedDateElement) as Date
                };
            }
            else if (element is R3Model.ValueSet.ConceptSetComponent conceptSet)
            {
                return new ValueSet.ConceptSetComponent
                {
                    Extension = Convert<Extension, R3Model.Extension>(conceptSet.Extension),
                    ModifierExtension = Convert<Extension, R3Model.Extension>(conceptSet.ModifierExtension),
                    Concept = Convert<ValueSet.ConceptReferenceComponent, R3Model.ValueSet.ConceptReferenceComponent>(conceptSet.Concept),
                    Filter = Convert<ValueSet.FilterComponent, R3Model.ValueSet.FilterComponent>(conceptSet.Filter),
                    SystemElement = ConvertElement(conceptSet.SystemElement) as FhirUri,
                    ValueSetElement = conceptSet.ValueSetElement.Select(vs => ConvertToCanonical(vs)).ToList(),
                    VersionElement = ConvertElement(conceptSet.VersionElement) as FhirString
                };
            }
            else if (element is R3Model.ValueSet.ConceptReferenceComponent concept)
            {
                return new ValueSet.ConceptReferenceComponent
                {
                    Extension = Convert<Extension, R3Model.Extension>(concept.Extension),
                    ModifierExtension = Convert<Extension, R3Model.Extension>(concept.ModifierExtension),
                    CodeElement = ConvertElement(concept.CodeElement) as Code,
                    Designation = Convert<ValueSet.DesignationComponent, R3Model.ValueSet.DesignationComponent>(concept.Designation),
                    DisplayElement = ConvertElement(concept.DisplayElement) as FhirString
                };
            }
            else if (element is R3Model.ValueSet.FilterComponent filter)
            {
                return new ValueSet.FilterComponent
                {
                    Extension = Convert<Extension, R3Model.Extension>(filter.Extension),
                    ModifierExtension = Convert<Extension, R3Model.Extension>(filter.ModifierExtension),
                    OpElement = ConvertPrimitive<FilterOperator, R3Model.FilterOperator>(filter.OpElement) as Code<FilterOperator>,
                    PropertyElement = ConvertElement(filter.PropertyElement) as Code,
                    ValueElement = ConvertToString(filter.ValueElement)
                };
            }
            else if (element is R3Model.ValueSet.DesignationComponent designation)
            {
                return new ValueSet.DesignationComponent
                {
                    Extension = Convert<Extension, R3Model.Extension>(designation.Extension),
                    ModifierExtension = Convert<Extension, R3Model.Extension>(designation.ModifierExtension),
                    LanguageElement = ConvertElement(designation.LanguageElement) as Code,
                    Use = ConvertElement(designation.Use) as Coding,
                    ValueElement = ConvertElement(designation.ValueElement) as FhirString
                };
            }
            else if (element is R3Model.ValueSet.ExpansionComponent expansion)
            {
                return new ValueSet.ExpansionComponent
                {
                    Extension = Convert<Extension, R3Model.Extension>(expansion.Extension),
                    ModifierExtension = Convert<Extension, R3Model.Extension>(expansion.ModifierExtension),
                    Contains = Convert<ValueSet.ContainsComponent, R3Model.ValueSet.ContainsComponent>(expansion.Contains),
                    IdentifierElement = ConvertElement(expansion.IdentifierElement) as FhirUri,
                    OffsetElement = ConvertElement(expansion.OffsetElement) as Integer,
                    Parameter = Convert<ValueSet.ParameterComponent, R3Model.ValueSet.ParameterComponent>(expansion.Parameter),
                    TimestampElement = ConvertElement(expansion.TimestampElement) as FhirDateTime,
                    TotalElement = ConvertElement(expansion.TotalElement) as Integer
                };
            }
            else if (element is R3Model.ValueSet.ContainsComponent contains)
            {
                return new ValueSet.ContainsComponent
                {
                    Extension = Convert<Extension, R3Model.Extension>(contains.Extension),
                    ModifierExtension = Convert<Extension, R3Model.Extension>(contains.ModifierExtension),
                    AbstractElement = ConvertElement(contains.AbstractElement) as FhirBoolean,
                    CodeElement = ConvertElement(contains.CodeElement) as Code,
                    Contains = Convert<ValueSet.ContainsComponent, R3Model.ValueSet.ContainsComponent>(contains.Contains),
                    Designation = Convert<ValueSet.DesignationComponent, R3Model.ValueSet.DesignationComponent>(contains.Designation),
                    DisplayElement = ConvertElement(contains.DisplayElement) as FhirString,
                    InactiveElement = ConvertElement(contains.InactiveElement) as FhirBoolean,
                    SystemElement = ConvertElement(contains.SystemElement) as FhirUri,
                    VersionElement = ConvertElement(contains.VersionElement) as FhirString
                };
            }
            else if (element is R3Model.ValueSet.ParameterComponent parameter)
            {
                return new ValueSet.ParameterComponent
                {
                    Extension = Convert<Extension, R3Model.Extension>(parameter.Extension),
                    ModifierExtension = Convert<Extension, R3Model.Extension>(parameter.ModifierExtension),
                    NameElement = ConvertElement(parameter.NameElement) as FhirString,
                    Value = ConvertElement(parameter.Value)
                };
            }

            throw new UnknownFhirTypeException(element.GetType());
        }
    }
}
