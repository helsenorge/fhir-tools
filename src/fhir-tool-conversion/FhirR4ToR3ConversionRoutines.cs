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
    internal class FhirR4ToR3ConversionRoutines
    {
        internal TTo? ConvertEnum<TTo, TFrom>(TFrom? from)
            where TTo : struct
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

        internal Primitive<TTo> ConvertPrimitive<TTo, TFrom>(R4Model.Primitive<TFrom> element)
            where TTo : struct, Enum
            where TFrom : struct, Enum
        {
            if (element == null) return default;

            if (element is R4Model.Code<TFrom> code)
            {
                return new Code<TTo>
                {
                    Extension = Convert<Extension, R4Model.Extension>(code.Extension),
                    Value = ConvertEnum<TTo, TFrom>(code.Value)
                };
            }

            return default;
        }
        internal Primitive ConvertPrimitive(Base from, Type enumType)
        {
            if (from == null) return null;

            var innerFrom = (R4Model.Primitive)from;

            var targetType = (typeof(Code<>)).MakeGenericType(enumType);
            Primitive to = Activator.CreateInstance(targetType) as Primitive;
            to.Extension = Convert<Extension, R4Model.Extension>(innerFrom.Extension);
            to.ObjectValue = innerFrom.ObjectValue;

            return to;
        }

        internal Code ConvertToCode(R4Model.Code<R4Model.Money.Currencies> currency)
        {
            if (currency == null) return null;
            return new Code
            {
                Extension = Convert<Extension, R4Model.Extension>(currency.Extension),
                Value = currency.Value?.GetLiteral()
            };
        }

        internal Element ConvertInitalComponent(List<R4Model.Questionnaire.InitialComponent> initials)
        {
            var initial = initials.FirstOrDefault();
            if (initial == null) return null;

            return ConvertElement(initial);
        }

        internal FhirUri ConvertToFhirUri(R4Model.FhirUrl url)
        {
            if (url == null) return null;
            return new FhirUri
            {
                Extension = Convert<Extension, R4Model.Extension>(url.Extension),
                Value = url.Value
            };
        }

        internal FhirUri ConvertToFhirUri(R4Model.Canonical canonical)
        {
            if (canonical == null) return null;
            return new FhirUri
            {
                Extension = Convert<Extension, R4Model.Extension>(canonical.Extension),
                Value = canonical.Value
            };
        }

        internal FhirString ConvertToFhirString(R4Model.Markdown markdown)
        {
            if (markdown == null) return null;
            return new FhirString
            {
                Extension = Convert<Extension, R4Model.Extension>(markdown.Extension),
                Value = markdown.Value
            };
        }

        internal Integer ConvertToInteger(R4Model.PositiveInt positiveInt)
        {
            if (positiveInt == null) return null;
            return new Integer
            {
                Extension = Convert<Extension, R4Model.Extension>(positiveInt.Extension),
                Value = positiveInt.Value
            };
        }

        internal ResourceReference ConvertToReference(R4Model.Canonical canonical)
        {
            if (canonical == null) return null;
            return new ResourceReference
            {
                Extension = Convert<Extension, R4Model.Extension>(canonical.Extension),
                Reference = canonical.Value
            };
        }

        internal Code ConvertToCode(R4Model.FhirString str)
        {
            if (str == null) return null;
            return new Code
            {
                Extension = Convert<Extension, R4Model.Extension>(str.Extension),
                Value = str.Value
            };
        }

        internal List<Questionnaire.EnableWhenComponent> ConvertEnableWhenComponent(List<R4Model.Questionnaire.EnableWhenComponent> enableWhenComponents)
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

                if (enableWhen.Operator == R4Model.Questionnaire.QuestionnaireItemOperator.Exists
                    && enableWhen.Answer is R4Model.FhirBoolean answer)
                {
                    enableWhenR3.HasAnswer = answer.Value;
                }
                else if (enableWhen.Answer != null)
                {
                    enableWhenR3.Answer = ConvertElement(enableWhen.Answer);
                }

                enableWhenComponentsR3.Add(enableWhenR3);
            }
            return enableWhenComponentsR3;
        }

        internal Element ConvertElement(Base element)
        {
            if (element == null) return null;

            if (element is R4Model.Instant instant)
            {
                return new Instant
                {
                    Extension = Convert<Extension, R4Model.Extension>(instant.Extension),
                    Value = instant.Value
                };
            }
            else if (element is R4Model.Time time)
            {
                return new Time
                {
                    Extension = Convert<Extension, R4Model.Extension>(time.Extension),
                    Value = time.Value
                };
            }
            else if (element is R4Model.Date date)
            {
                return new Date
                {
                    Extension = Convert<Extension, R4Model.Extension>(date.Extension),
                    Value = date.Value
                };
            }
            else if (element is R4Model.FhirDateTime dateTime)
            {
                return new FhirDateTime
                {
                    Extension = Convert<Extension, R4Model.Extension>(dateTime.Extension),
                    Value = dateTime.Value
                };
            }
            else if (element is R4Model.Base64Binary base64Binary)
            {
                return new Base64Binary
                {
                    Extension = Convert<Extension, R4Model.Extension>(base64Binary.Extension),
                    Value = base64Binary.Value
                };
            }
            else if (element is R4Model.FhirDecimal dec)
            {
                return new FhirDecimal
                {
                    Extension = Convert<Extension, R4Model.Extension>(dec.Extension),
                    Value = dec.Value
                };
            }
            else if (element is R4Model.FhirBoolean boolean)
            {
                return new FhirBoolean
                {
                    Extension = Convert<Extension, R4Model.Extension>(boolean.Extension),
                    Value = boolean.Value
                };
            }
            else if (element is R4Model.Code code)
            {
                return new Code
                {
                    Extension = Convert<Extension, R4Model.Extension>(code.Extension),
                    Value = code.Value
                };
            }
            else if (element is R4Model.FhirString str)
            {
                return new FhirString
                {
                    Extension = Convert<Extension, R4Model.Extension>(str.Extension),
                    Value = str.Value
                };
            }
            else if (element is R4Model.Integer integer)
            {
                return new Integer
                {
                    Extension = Convert<Extension, R4Model.Extension>(integer.Extension),
                    Value = integer.Value
                };
            }
            else if (element is R4Model.FhirUri uri)
            {
                return new FhirUri
                {
                    Extension = Convert<Extension, R4Model.Extension>(uri.Extension),
                    Value = uri.Value
                };
            }
            else if (element is R4Model.Id id)
            {
                return new Id
                {
                    Extension = Convert<Extension, R4Model.Extension>(id.Extension),
                    Value = id.Value
                };
            }
            else if (element is R4Model.Oid oid)
            {
                return new Oid
                {
                    Extension = Convert<Extension, R4Model.Extension>(oid.Extension),
                    Value = oid.Value
                };
            }
            else if (element is R4Model.Uuid uuid)
            {
                return new Uuid
                {
                    Extension = Convert<Extension, R4Model.Extension>(uuid.Extension),
                    Value = uuid.Value
                };
            }
            else if (element is R4Model.UnsignedInt unsignedInt)
            {
                return new UnsignedInt
                {
                    Extension = Convert<Extension, R4Model.Extension>(unsignedInt.Extension),
                    Value = unsignedInt.Value
                };
            }
            else if (element is R4Model.PositiveInt positiveInt)
            {
                return new PositiveInt
                {
                    Extension = Convert<Extension, R4Model.Extension>(positiveInt.Extension),
                    Value = positiveInt.Value
                };
            }
            else if (element is R4Model.Ratio ratio)
            {
                return new Ratio
                {
                    Extension = Convert<Extension, R4Model.Extension>(ratio.Extension),
                    Denominator = ConvertElement(ratio.Denominator) as Quantity,
                    Numerator = ConvertElement(ratio.Numerator) as Quantity
                };
            }
            else if (element is R4Model.Period period)
            {
                return new Period
                {
                    Extension = Convert<Extension, R4Model.Extension>(period.Extension),
                    StartElement = ConvertElement(period.StartElement) as FhirDateTime,
                    EndElement = ConvertElement(period.EndElement) as FhirDateTime,
                };
            }
            else if (element is R4Model.Range range)
            {
                return new Range
                {
                    Extension = Convert<Extension, R4Model.Extension>(range.Extension),
                    High = ConvertElement(range.High) as Quantity,
                    Low = ConvertElement(range.Low) as Quantity,
                };
            }
            else if (element is R4Model.Attachment attachment)
            {
                return new Attachment
                {
                    Extension = Convert<Extension, R4Model.Extension>(attachment.Extension),
                    ContentTypeElement = ConvertElement(attachment.ContentTypeElement) as Code,
                    CreationElement = ConvertElement(attachment.CreationElement) as FhirDateTime,
                    DataElement = ConvertElement(attachment.DataElement) as Base64Binary,
                    HashElement = ConvertElement(attachment.HashElement) as Base64Binary,
                    LanguageElement = ConvertElement(attachment.LanguageElement) as Code,
                    SizeElement = ConvertElement(attachment.SizeElement) as UnsignedInt,
                    TitleElement = ConvertElement(attachment.TitleElement) as FhirString,
                    UrlElement = ConvertToFhirUri(attachment.UrlElement)
                };
            }
            else if (element is R4Model.Identifier identifier)
            {
                return new Identifier
                {
                    Extension = Convert<Extension, R4Model.Extension>(identifier.Extension),
                    Assigner = ConvertElement(identifier.Assigner) as ResourceReference,
                    Period = ConvertElement(identifier.Period) as Period,
                    SystemElement = ConvertElement(identifier.SystemElement) as FhirUri,
                    Type = ConvertElement(identifier.Type) as CodeableConcept,
                    UseElement = ConvertPrimitive<Identifier.IdentifierUse, R4Model.Identifier.IdentifierUse>(identifier.UseElement) as Code<Identifier.IdentifierUse>,
                    ValueElement = ConvertElement(identifier.ValueElement) as FhirString
                };
            }
            else if (element is R4Model.Annotation annotation)
            {
                return new Annotation
                {
                    Extension = Convert<Extension, R4Model.Extension>(annotation.Extension),
                    Author = ConvertElement(annotation.Author),
                    TextElement = ConvertToFhirString(annotation.Text),
                    TimeElement = ConvertElement(annotation.TimeElement) as FhirDateTime,
                };
            }
            else if (element is R4Model.HumanName humanName)
            {
                return new HumanName
                {
                    Extension = Convert<Extension, R4Model.Extension>(humanName.Extension),
                    FamilyElement = ConvertElement(humanName.FamilyElement) as FhirString,
                    GivenElement = Convert<FhirString, R4Model.FhirString>(humanName.GivenElement),
                    Period = ConvertElement(humanName.Period) as Period,
                    PrefixElement = Convert<FhirString, R4Model.FhirString>(humanName.PrefixElement),
                    SuffixElement = Convert<FhirString, R4Model.FhirString>(humanName.SuffixElement),
                    TextElement = ConvertElement(humanName.TextElement) as FhirString,
                    UseElement = ConvertPrimitive<HumanName.NameUse, R4Model.HumanName.NameUse>(humanName.UseElement) as Code<HumanName.NameUse>
                };
            }
            else if (element is R4Model.CodeableConcept codeableConcept)
            {
                return new CodeableConcept
                {
                    Extension = Convert<Extension, R4Model.Extension>(codeableConcept.Extension),
                    Coding = Convert<Coding, R4Model.Coding>(codeableConcept.Coding),
                    TextElement = ConvertElement(codeableConcept.TextElement) as FhirString
                };
            }
            else if (element is R4Model.ContactPoint contactPoint)
            {
                return new ContactPoint
                {
                    Extension = Convert<Extension, R4Model.Extension>(contactPoint.Extension),
                    Period = ConvertElement(contactPoint.Period) as Period,
                    RankElement = ConvertElement(contactPoint.RankElement) as PositiveInt,
                    SystemElement = ConvertPrimitive<ContactPoint.ContactPointSystem, R4Model.ContactPoint.ContactPointSystem>(contactPoint.SystemElement) as Code<ContactPoint.ContactPointSystem>,
                    UseElement = ConvertPrimitive<ContactPoint.ContactPointUse, R4Model.ContactPoint.ContactPointUse>(contactPoint.UseElement) as Code<ContactPoint.ContactPointUse>,
                    ValueElement = ConvertElement(contactPoint.ValueElement) as FhirString
                };
            }
            else if (element is R4Model.Coding coding)
            {
                return new Coding
                {
                    Extension = Convert<Extension, R4Model.Extension>(coding.Extension),
                    CodeElement = ConvertElement(coding.CodeElement) as Code,
                    DisplayElement = ConvertElement(coding.DisplayElement) as FhirString,
                    SystemElement = ConvertElement(coding.SystemElement) as FhirUri,
                    UserSelectedElement = ConvertElement(coding.UserSelectedElement) as FhirBoolean,
                    VersionElement = ConvertElement(coding.VersionElement) as FhirString
                };
            }
            else if (element is R4Model.Money money)
            {
                Money mon = new Money
                {
                    Extension = Convert<Extension, R4Model.Extension>(money.Extension),
                    ValueElement = ConvertElement(money.ValueElement) as FhirDecimal
                };
                if (money.Currency.HasValue)
                {
                    mon.System = "urn:iso:std:iso:4217";
                    mon.CodeElement = ConvertToCode(money.CurrencyElement);
                }
                return mon;
            }
            else if (element is R4Model.Address address)
            {
                return new Address
                {
                    Extension = Convert<Extension, R4Model.Extension>(address.Extension),
                    CityElement = ConvertElement(address.CityElement) as FhirString,
                    CountryElement = ConvertElement(address.CountryElement) as FhirString,
                    DistrictElement = ConvertElement(address.DistrictElement) as FhirString,
                    LineElement = Convert<FhirString, R4Model.FhirString>(address.LineElement),
                    Period = ConvertElement(address.Period) as Period,
                    PostalCodeElement = ConvertElement(address.PostalCodeElement) as FhirString,
                    StateElement = ConvertElement(address.StateElement) as FhirString,
                    TextElement = ConvertElement(address.TextElement) as FhirString,
                    TypeElement = ConvertPrimitive<Address.AddressType, R4Model.Address.AddressType>(address.TypeElement) as Code<Address.AddressType>,
                    UseElement = ConvertPrimitive<Address.AddressUse, R4Model.Address.AddressUse>(address.UseElement) as Code<Address.AddressUse>
                };
            }
            else if (element is R4Model.Timing timing)
            {
                return new Timing
                {
                    EventElement = Convert<FhirDateTime, R4Model.FhirDateTime>(timing.EventElement),
                    Repeat = ConvertElement(timing.Repeat) as Timing.RepeatComponent,
                    Code = ConvertElement(timing.Code) as CodeableConcept
                };
            }
            else if (element is R4Model.Timing.RepeatComponent repeatComponent)
            {
                return new Timing.RepeatComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(repeatComponent.Extension),
                    Bounds = ConvertElement(repeatComponent.Bounds),
                    CountElement = ConvertToInteger(repeatComponent.CountElement),
                    CountMaxElement = ConvertToInteger(repeatComponent.CountMaxElement),
                    DurationElement = ConvertElement(repeatComponent.DurationElement) as FhirDecimal,
                    DurationMaxElement = ConvertElement(repeatComponent.DurationMaxElement) as FhirDecimal,
                    DurationUnitElement = ConvertPrimitive<Timing.UnitsOfTime, R4Model.Timing.UnitsOfTime>(repeatComponent.DurationUnitElement) as Code<Timing.UnitsOfTime>,
                    FrequencyElement = ConvertToInteger(repeatComponent.FrequencyElement),
                    FrequencyMaxElement = ConvertToInteger(repeatComponent.FrequencyMaxElement),
                    PeriodElement = ConvertElement(repeatComponent.PeriodElement) as FhirDecimal,
                    PeriodMaxElement = ConvertElement(repeatComponent.PeriodMaxElement) as FhirDecimal,
                    PeriodUnitElement = ConvertPrimitive<Timing.UnitsOfTime, R4Model.Timing.UnitsOfTime>(repeatComponent.PeriodUnitElement) as Code<Timing.UnitsOfTime>,
                    DayOfWeekElement = repeatComponent.DayOfWeekElement.Select(dow => ConvertPrimitive<DaysOfWeek, R4Model.DaysOfWeek>(dow) as Code<DaysOfWeek>).ToList(),
                    TimeOfDayElement = Convert<Time, R4Model.Time>(repeatComponent.TimeOfDayElement),
                    WhenElement = repeatComponent.WhenElement.Select(w => ConvertPrimitive<Timing.EventTiming, R4Model.Timing.EventTiming>(w) as Code<Timing.EventTiming>).ToList(),
                    OffsetElement = ConvertElement(repeatComponent.OffsetElement) as UnsignedInt
                };
            }
            else if (element is R4Model.Quantity quantity)
            {
                return new Quantity
                {
                    Extension = Convert<Extension, R4Model.Extension>(quantity.Extension),
                    CodeElement = ConvertElement(quantity.CodeElement) as Code,
                    ComparatorElement = ConvertPrimitive<Quantity.QuantityComparator, R4Model.Quantity.QuantityComparator>(quantity.ComparatorElement) as Code<Quantity.QuantityComparator>,
                    SystemElement = ConvertElement(quantity.SystemElement) as FhirUri,
                    UnitElement = ConvertElement(quantity.UnitElement) as FhirString,
                    ValueElement = ConvertElement(quantity.ValueElement) as FhirDecimal
                };
            }
            else if (element is R4Model.SampledData sampledData)
            {
                return new SampledData
                {
                    Extension = Convert<Extension, R4Model.Extension>(sampledData.Extension),
                    Origin = ConvertElement(sampledData.Origin) as Quantity,
                    PeriodElement = ConvertElement(sampledData.PeriodElement) as FhirDecimal,
                    FactorElement = ConvertElement(sampledData.FactorElement) as FhirDecimal,
                    LowerLimitElement = ConvertElement(sampledData.LowerLimitElement) as FhirDecimal,
                    UpperLimitElement = ConvertElement(sampledData.UpperLimitElement) as FhirDecimal,
                    DimensionsElement = ConvertElement(sampledData.DimensionsElement) as PositiveInt,
                    DataElement = ConvertElement(sampledData.DataElement) as FhirString
                };
            }
            else if (element is R4Model.Signature signature)
            {
                return new Signature
                {
                    Type = Convert<Coding, R4Model.Coding>(signature.Type),
                    WhenElement = ConvertElement(signature.WhenElement) as Instant,
                    Who = ConvertElement(signature.Who),
                    OnBehalfOf = ConvertElement(signature.OnBehalfOf),
                    ContentTypeElement = ConvertElement(signature.SigFormatElement) as Code,
                    BlobElement = ConvertElement(signature.DataElement) as Base64Binary
                };
            }
            else if (element is R4Model.Age age)
            {
                return new Age
                {
                    Extension = Convert<Extension, R4Model.Extension>(age.Extension),
                    CodeElement = ConvertElement(age.CodeElement) as Code,
                    ComparatorElement = ConvertPrimitive<Quantity.QuantityComparator, R4Model.Quantity.QuantityComparator>(age.ComparatorElement) as Code<Quantity.QuantityComparator>,
                    SystemElement = ConvertElement(age.SystemElement) as FhirUri,
                    UnitElement = ConvertElement(age.UnitElement) as FhirString,
                    ValueElement = ConvertElement(age.ValueElement) as FhirDecimal
                };
            }
            else if (element is R4Model.Distance distance)
            {
                return new Distance
                {
                    Extension = Convert<Extension, R4Model.Extension>(distance.Extension),
                    CodeElement = ConvertElement(distance.CodeElement) as Code,
                    ComparatorElement = ConvertPrimitive<Quantity.QuantityComparator, R4Model.Quantity.QuantityComparator>(distance.ComparatorElement) as Code<Quantity.QuantityComparator>,
                    SystemElement = ConvertElement(distance.SystemElement) as FhirUri,
                    UnitElement = ConvertElement(distance.UnitElement) as FhirString,
                    ValueElement = ConvertElement(distance.ValueElement) as FhirDecimal
                };
            }
            else if (element is R4Model.Duration duration)
            {
                return new Duration
                {
                    Extension = Convert<Extension, R4Model.Extension>(duration.Extension),
                    CodeElement = ConvertElement(duration.CodeElement) as Code,
                    ComparatorElement = ConvertPrimitive<Quantity.QuantityComparator, R4Model.Quantity.QuantityComparator>(duration.ComparatorElement) as Code<Quantity.QuantityComparator>,
                    SystemElement = ConvertElement(duration.SystemElement) as FhirUri,
                    UnitElement = ConvertElement(duration.UnitElement) as FhirString,
                    ValueElement = ConvertElement(duration.ValueElement) as FhirDecimal
                };
            }
            else if (element is R4Model.Count count)
            {
                return new Count
                {
                    Extension = Convert<Extension, R4Model.Extension>(count.Extension),
                    CodeElement = ConvertElement(count.CodeElement) as Code,
                    ComparatorElement = ConvertPrimitive<Quantity.QuantityComparator, R4Model.Quantity.QuantityComparator>(count.ComparatorElement) as Code<Quantity.QuantityComparator>,
                    SystemElement = ConvertElement(count.SystemElement) as FhirUri,
                    UnitElement = ConvertElement(count.UnitElement) as FhirString,
                    ValueElement = ConvertElement(count.ValueElement) as FhirDecimal
                };
            }
            else if (element is R4Model.MoneyQuantity moneyQuantity)
            {
                return new Money
                {
                    Extension = Convert<Extension, R4Model.Extension>(moneyQuantity.Extension),
                    CodeElement = ConvertElement(moneyQuantity.CodeElement) as Code,
                    ComparatorElement = ConvertPrimitive<Quantity.QuantityComparator, R4Model.Quantity.QuantityComparator>(moneyQuantity.ComparatorElement) as Code<Quantity.QuantityComparator>,
                    SystemElement = ConvertElement(moneyQuantity.SystemElement) as FhirUri,
                    UnitElement = ConvertElement(moneyQuantity.UnitElement) as FhirString,
                    ValueElement = ConvertElement(moneyQuantity.ValueElement) as FhirDecimal
                };
            }
            else if (element is R4Model.SimpleQuantity simpleQuantity)
            {
                return new SimpleQuantity
                {
                    Extension = Convert<Extension, R4Model.Extension>(simpleQuantity.Extension),
                    CodeElement = ConvertElement(simpleQuantity.CodeElement) as Code,
                    ComparatorElement = ConvertPrimitive<Quantity.QuantityComparator, R4Model.Quantity.QuantityComparator>(simpleQuantity.ComparatorElement) as Code<Quantity.QuantityComparator>,
                    SystemElement = ConvertElement(simpleQuantity.SystemElement) as FhirUri,
                    UnitElement = ConvertElement(simpleQuantity.UnitElement) as FhirString,
                    ValueElement = ConvertElement(simpleQuantity.ValueElement) as FhirDecimal
                };
            }
            else if (element is R4Model.ContactDetail contactDetail)
            {
                return new ContactDetail
                {
                    Extension = Convert<Extension, R4Model.Extension>(contactDetail.Extension),
                    NameElement = ConvertElement(contactDetail.NameElement) as FhirString,
                    Telecom = Convert<ContactPoint, R4Model.ContactPoint>(contactDetail.Telecom)
                };
            }
            else if (element is R4Model.Contributor contributor)
            {
                return new Contributor
                {
                    Extension = Convert<Extension, R4Model.Extension>(contributor.Extension),
                    TypeElement = ConvertPrimitive<Contributor.ContributorType, R4Model.Contributor.ContributorType>(contributor.TypeElement) as Code<Contributor.ContributorType>,
                    NameElement = ConvertElement(contributor.NameElement) as FhirString,
                    Contact = Convert<ContactDetail, R4Model.ContactDetail>(contributor.Contact)
                };
            }
            else if (element is R4Model.DataRequirement dataRequirement)
            {
                return new DataRequirement
                {
                    Extension = Convert<Extension, R4Model.Extension>(dataRequirement.Extension),
                    TypeElement = ConvertPrimitive<FHIRAllTypes, R4Model.FHIRAllTypes>(dataRequirement.TypeElement) as Code<FHIRAllTypes>,
                    ProfileElement = dataRequirement.ProfileElement.Select(p => ConvertToFhirUri(p)).ToList(),
                    MustSupportElement = Convert<FhirString, R4Model.FhirString>(dataRequirement.MustSupportElement),
                    CodeFilter = Convert<DataRequirement.CodeFilterComponent, R4Model.DataRequirement.CodeFilterComponent>(dataRequirement.CodeFilter),
                    DateFilter = Convert<DataRequirement.DateFilterComponent, R4Model.DataRequirement.DateFilterComponent>(dataRequirement.DateFilter)
                };
            }
            else if (element is R4Model.DataRequirement.CodeFilterComponent codeFilter)
            {
                return new DataRequirement.CodeFilterComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(codeFilter.Extension),
                    PathElement = ConvertElement(codeFilter.PathElement) as FhirString,
                    ValueSet = ConvertToReference(codeFilter.ValueSetElement),
                    ValueCoding = Convert<Coding, R4Model.Coding>(codeFilter.Code),
                };
            }
            else if (element is R4Model.DataRequirement.DateFilterComponent dateFilter)
            {
                return new DataRequirement.DateFilterComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(dateFilter.Extension),
                    PathElement = ConvertElement(dateFilter.PathElement) as FhirString,
                    Value = ConvertElement(dateFilter.Value)
                };
            }
            else if (element is R4Model.RelatedArtifact relatedArtifact)
            {
                return new RelatedArtifact
                {
                    Extension = Convert<Extension, R4Model.Extension>(relatedArtifact.Extension),
                    TypeElement = ConvertPrimitive<RelatedArtifact.RelatedArtifactType, R4Model.RelatedArtifact.RelatedArtifactType>(relatedArtifact.TypeElement) as Code<RelatedArtifact.RelatedArtifactType>,
                    DisplayElement = ConvertElement(relatedArtifact.DisplayElement) as FhirString,
                    CitationElement = ConvertToFhirString(relatedArtifact.Citation),
                    UrlElement = ConvertToFhirUri(relatedArtifact.UrlElement),
                    Document = ConvertElement(relatedArtifact.Document) as Attachment,
                    Resource = ConvertToReference(relatedArtifact.ResourceElement)
                };
            }
            else if (element is R4Model.UsageContext usageContext)
            {
                return new UsageContext
                {
                    Extension = Convert<Extension, R4Model.Extension>(usageContext.Extension),
                    Code = ConvertElement(usageContext.Code) as Coding,
                    Value = ConvertElement(usageContext.Value)
                };
            }
            else if (element is R4Model.ParameterDefinition parameterDefinition)
            {
                return new ParameterDefinition
                {
                    Extension = Convert<Extension, R4Model.Extension>(parameterDefinition.Extension),
                    NameElement = ConvertElement(parameterDefinition.NameElement) as Code,
                    UseElement = ConvertPrimitive<OperationParameterUse, R4Model.OperationParameterUse>(parameterDefinition.UseElement) as Code<OperationParameterUse>,
                    MinElement = ConvertElement(parameterDefinition.MinElement) as Integer,
                    MaxElement = ConvertElement(parameterDefinition.MaxElement) as FhirString,
                    DocumentationElement = ConvertElement(parameterDefinition.DocumentationElement) as FhirString,
                    TypeElement = ConvertPrimitive<FHIRAllTypes, R4Model.FHIRAllTypes>(parameterDefinition.TypeElement) as Code<FHIRAllTypes>,
                    Profile = ConvertToReference(parameterDefinition.ProfileElement)
                };
            }
            else if (element is R4Model.TriggerDefinition triggerDefinition)
            {
                return new TriggerDefinition
                {
                    Extension = Convert<Extension, R4Model.Extension>(triggerDefinition.Extension),
                    TypeElement = ConvertPrimitive<TriggerDefinition.TriggerType, R4Model.TriggerDefinition.TriggerType>(triggerDefinition.TypeElement) as Code<TriggerDefinition.TriggerType>,
                    EventNameElement = ConvertElement(triggerDefinition.NameElement) as FhirString,
                    EventTiming = ConvertElement(triggerDefinition.Timing),
                    EventData = ConvertElement(triggerDefinition.Data.FirstOrDefault()) as DataRequirement
                };
            }
            else if (element is R4Model.ResourceReference reference)
            {
                return new ResourceReference
                {
                    Extension = Convert<Extension, R4Model.Extension>(reference.Extension),
                    DisplayElement = ConvertElement(reference.DisplayElement) as FhirString,
                    Identifier = ConvertElement(reference.Identifier) as Identifier,
                    ReferenceElement = ConvertElement(reference.ReferenceElement) as FhirString
                };
            }
            else if (element is R4Model.Meta meta)
            {
                return new Meta
                {
                    Extension = Convert<Extension, R4Model.Extension>(meta.Extension),
                    LastUpdatedElement = ConvertElement(meta.LastUpdatedElement) as Instant,
                    ProfileElement = meta.ProfileElement.Select(p => ConvertToFhirUri(p)).ToList(),
                    Security = Convert<Coding, R4Model.Coding>(meta.Security),
                    Tag = Convert<Coding, R4Model.Coding>(meta.Tag),
                    VersionIdElement = ConvertElement(meta.VersionIdElement) as Id
                };
            }
            else if (element is R4Model.Dosage dosage)
            {
                var dos = new Dosage
                {
                    Extension = Convert<Extension, R4Model.Extension>(dosage.Extension),
                    SequenceElement = ConvertElement(dosage.SequenceElement) as Integer,
                    TextElement = ConvertElement(dosage.TextElement) as FhirString,
                    AdditionalInstruction = Convert<CodeableConcept, R4Model.CodeableConcept>(dosage.AdditionalInstruction),
                    PatientInstructionElement = ConvertElement(dosage.PatientInstructionElement) as FhirString,
                    Timing = ConvertElement(dosage.Timing) as Timing,
                    AsNeeded = ConvertElement(dosage.AsNeeded),
                    Site = ConvertElement(dosage.Site) as CodeableConcept,
                    Route = ConvertElement(dosage.Route) as CodeableConcept,
                    Method = ConvertElement(dosage.Method) as CodeableConcept,
                    Dose = ConvertElement(dosage.DoseAndRate?.FirstOrDefault()?.Dose),
                    Rate = ConvertElement(dosage.DoseAndRate?.FirstOrDefault()?.Rate),
                    MaxDosePerPeriod = ConvertElement(dosage.MaxDosePerPeriod) as Ratio,
                    MaxDosePerAdministration = ConvertElement(dosage.MaxDosePerAdministration) as Quantity,
                    MaxDosePerLifetime = ConvertElement(dosage.MaxDosePerLifetime) as Quantity
                };
                bool foundDose = false;
                bool foundRate = false;
                foreach (var doseAndRate in dosage.DoseAndRate)
                {
                    if (foundDose && foundRate) break;

                    if (!foundDose && doseAndRate.Dose != null)
                    {
                        dos.Dose = ConvertElement(doseAndRate.Dose);
                    }
                    if (!foundRate && doseAndRate.Rate != null)
                    {
                        dos.Rate = ConvertElement(doseAndRate.Rate);
                    }
                }
            }
            else if (element is R4Model.XHtml xhtml)
            {
                return new XHtml
                {
                    Extension = Convert<Extension, R4Model.Extension>(xhtml.Extension),
                    Value = xhtml.Value
                };
            }
            else if (element is R4Model.Narrative narrative)
            {
                return new Narrative
                {
                    Extension = Convert<Extension, R4Model.Extension>(narrative.Extension),
                    Div = narrative.Div,
                    StatusElement = ConvertPrimitive<Narrative.NarrativeStatus, R4Model.Narrative.NarrativeStatus>(narrative.StatusElement) as Code<Narrative.NarrativeStatus>
                };
            }
            else if (element is R4Model.ElementDefinition elementDefinition)
            {
                return new ElementDefinition
                {
                    Extension = Convert<Extension, R4Model.Extension>(elementDefinition.Extension),
                    PathElement = ConvertElement(elementDefinition.PathElement) as FhirString,
                    RepresentationElement = elementDefinition.RepresentationElement.Select(r => ConvertPrimitive<ElementDefinition.PropertyRepresentation, R4Model.ElementDefinition.PropertyRepresentation>(r) as Code<ElementDefinition.PropertyRepresentation>).ToList(),
                    SliceNameElement = ConvertElement(elementDefinition.SliceNameElement) as FhirString,
                    LabelElement = ConvertElement(elementDefinition.LabelElement) as FhirString,
                    Code = Convert<Coding, R4Model.Coding>(elementDefinition.Code),
                    Slicing = ConvertElement(elementDefinition.Slicing) as ElementDefinition.SlicingComponent,
                    ShortElement = ConvertElement(elementDefinition.ShortElement) as FhirString,
                    DefinitionElement = ConvertElement(elementDefinition.Definition) as Markdown,
                    CommentElement = ConvertElement(elementDefinition.Comment) as Markdown,
                    RequirementsElement = ConvertElement(elementDefinition.Requirements) as Markdown,
                    AliasElement = Convert<FhirString, R4Model.FhirString>(elementDefinition.AliasElement),
                    MinElement = ConvertElement(elementDefinition.MinElement) as UnsignedInt,
                    MaxElement = ConvertElement(elementDefinition.MaxElement) as FhirString,
                    Base = ConvertElement(elementDefinition.Base) as ElementDefinition.BaseComponent,
                    ContentReferenceElement = ConvertElement(elementDefinition.ContentReferenceElement) as FhirUri,
                    Type = Convert<ElementDefinition.TypeRefComponent, R4Model.ElementDefinition.TypeRefComponent>(elementDefinition.Type),
                    DefaultValue = ConvertElement(elementDefinition.DefaultValue),
                    MeaningWhenMissingElement = ConvertElement(elementDefinition.MeaningWhenMissing) as Markdown,
                    OrderMeaningElement = ConvertElement(elementDefinition.OrderMeaningElement) as FhirString,
                    Fixed = ConvertElement(elementDefinition.Fixed),
                    Pattern = ConvertElement(elementDefinition.Pattern),
                    Example = Convert<ElementDefinition.ExampleComponent, R4Model.ElementDefinition.ExampleComponent>(elementDefinition.Example),
                    MinValue = ConvertElement(elementDefinition.MinValue),
                    MaxValue = ConvertElement(elementDefinition.MaxValue),
                    MaxLengthElement = ConvertElement(elementDefinition.MaxLengthElement) as Integer,
                    ConditionElement = Convert<Id, R4Model.Id>(elementDefinition.ConditionElement),
                    Constraint = Convert<ElementDefinition.ConstraintComponent, R4Model.ElementDefinition.ConstraintComponent>(elementDefinition.Constraint),
                    MustSupportElement = ConvertElement(elementDefinition.MustSupportElement) as FhirBoolean,
                    IsModifierElement = ConvertElement(elementDefinition.IsModifierElement) as FhirBoolean,
                    IsSummaryElement = ConvertElement(elementDefinition.IsSummaryElement) as FhirBoolean,
                    Binding = ConvertElement(elementDefinition.Binding) as ElementDefinition.ElementDefinitionBindingComponent,
                    Mapping = Convert<ElementDefinition.MappingComponent, R4Model.ElementDefinition.MappingComponent>(elementDefinition.Mapping)
                };
            }
            else if (element is R4Model.ElementDefinition.SlicingComponent slicing)
            {
                return new ElementDefinition.SlicingComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(slicing.Extension),
                    Discriminator = Convert<ElementDefinition.DiscriminatorComponent, R4Model.ElementDefinition.DiscriminatorComponent>(slicing.Discriminator),
                    DescriptionElement = ConvertElement(slicing.DescriptionElement) as FhirString,
                    OrderedElement = ConvertElement(slicing.OrderedElement) as FhirBoolean,
                    RulesElement = ConvertPrimitive<ElementDefinition.SlicingRules, R4Model.ElementDefinition.SlicingRules>(slicing.RulesElement) as Code<ElementDefinition.SlicingRules>
                };
            }
            else if (element is R4Model.ElementDefinition.DiscriminatorComponent discriminator)
            {
                return new ElementDefinition.DiscriminatorComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(discriminator.Extension),
                    TypeElement = ConvertPrimitive<ElementDefinition.DiscriminatorType, R4Model.ElementDefinition.DiscriminatorType>(discriminator.TypeElement) as Code<ElementDefinition.DiscriminatorType>,
                    PathElement = ConvertElement(discriminator.PathElement) as FhirString
                };
            }
            else if (element is R4Model.ElementDefinition.BaseComponent baseComponent)
            {
                return new ElementDefinition.BaseComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(baseComponent.Extension),
                    PathElement = ConvertElement(baseComponent.PathElement) as FhirString,
                    MinElement = ConvertElement(baseComponent.MinElement) as UnsignedInt,
                    MaxElement = ConvertElement(baseComponent.MaxElement) as FhirString
                };
            }
            else if (element is R4Model.ElementDefinition.TypeRefComponent typeRef)
            {
                return new ElementDefinition.TypeRefComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(typeRef.Extension),
                    CodeElement = ConvertElement(typeRef.CodeElement) as FhirUri,
                    ProfileElement = ConvertToFhirUri(typeRef.ProfileElement.FirstOrDefault()),
                    TargetProfileElement = ConvertToFhirUri(typeRef.TargetProfileElement.FirstOrDefault()),
                    AggregationElement = typeRef.AggregationElement.Select(a => ConvertPrimitive<ElementDefinition.AggregationMode, R4Model.ElementDefinition.AggregationMode>(a) as Code<ElementDefinition.AggregationMode>).ToList(),
                    VersioningElement = ConvertPrimitive<ElementDefinition.ReferenceVersionRules, R4Model.ElementDefinition.ReferenceVersionRules>(typeRef.VersioningElement) as Code<ElementDefinition.ReferenceVersionRules>
                };
            }
            else if (element is R4Model.ElementDefinition.ExampleComponent example)
            {
                return new ElementDefinition.ExampleComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(example.Extension),
                    LabelElement = ConvertElement(example.LabelElement) as FhirString,
                    Value = ConvertElement(example.Value)
                };
            }
            else if (element is R4Model.ElementDefinition.ConstraintComponent constraint)
            {
                return new ElementDefinition.ConstraintComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(constraint.Extension),
                    KeyElement = ConvertElement(constraint.KeyElement) as Id,
                    RequirementsElement = ConvertElement(constraint.RequirementsElement) as FhirString,
                    SeverityElement = ConvertPrimitive<ElementDefinition.ConstraintSeverity, R4Model.ElementDefinition.ConstraintSeverity>(constraint.SeverityElement) as Code<ElementDefinition.ConstraintSeverity>,
                    HumanElement = ConvertElement(constraint.HumanElement) as FhirString,
                    ExpressionElement = ConvertElement(constraint.ExpressionElement) as FhirString,
                    XpathElement = ConvertElement(constraint.XpathElement) as FhirString,
                    SourceElement = ConvertToFhirUri(constraint.SourceElement)
                };
            }
            else if (element is R4Model.ElementDefinition.ElementDefinitionBindingComponent binding)
            {
                return new ElementDefinition.ElementDefinitionBindingComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(binding.Extension),
                    StrengthElement = ConvertPrimitive<BindingStrength, R4Model.BindingStrength>(binding.StrengthElement) as Code<BindingStrength>,
                    DescriptionElement = ConvertElement(binding.DescriptionElement) as FhirString,
                    ValueSet = ConvertToReference(binding.ValueSetElement)
                };
            }
            else if (element is R4Model.ElementDefinition.MappingComponent mapping)
            {
                return new ElementDefinition.MappingComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(mapping.Extension),
                    IdentityElement = ConvertElement(mapping.IdentityElement) as Id,
                    LanguageElement = ConvertElement(mapping.LanguageElement) as Code,
                    MapElement = ConvertElement(mapping.MapElement) as FhirString,
                    CommentElement = ConvertElement(mapping.CommentElement) as FhirString
                };
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
            else if (element is R4Model.Markdown markdown)
            {
                return new Markdown
                {
                    Extension = Convert<Extension, R4Model.Extension>(markdown.Extension),
                    Value = markdown.Value
                };
            }
            else if (element is R4Model.Questionnaire.ItemComponent item)
            {
                return new Questionnaire.ItemComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(item.Extension),
                    ModifierExtension = Convert<Extension, R4Model.Extension>(item.ModifierExtension),
                    Code = Convert<Coding, R4Model.Coding>(item.Code),
                    DefinitionElement = ConvertElement(item.DefinitionElement) as FhirUri,
                    EnableWhen = ConvertEnableWhenComponent(item.EnableWhen),
                    Initial = ConvertInitalComponent(item.Initial),
                    Item = Convert<Questionnaire.ItemComponent, R4Model.Questionnaire.ItemComponent>(item.Item),
                    LinkIdElement = ConvertElement(item.LinkIdElement) as FhirString,
                    MaxLengthElement = ConvertElement(item.MaxLengthElement) as Integer,
                    Option = Convert<Questionnaire.OptionComponent, R4Model.Questionnaire.AnswerOptionComponent>(item.AnswerOption),
                    Options = ConvertToReference(item.AnswerValueSetElement),
                    PrefixElement = ConvertElement(item.PrefixElement) as FhirString,
                    ReadOnlyElement = ConvertElement(item.ReadOnlyElement) as FhirBoolean,
                    RepeatsElement = ConvertElement(item.RepeatsElement) as FhirBoolean,
                    RequiredElement = ConvertElement(item.RequiredElement) as FhirBoolean,
                    TextElement = ConvertElement(item.TextElement) as FhirString,
                    TypeElement = ConvertPrimitive<Questionnaire.QuestionnaireItemType, R4Model.Questionnaire.QuestionnaireItemType>(item.TypeElement) as Code<Questionnaire.QuestionnaireItemType>
                };
            }
            else if (element is R4Model.ValueSet.ComposeComponent compose)
            {
                return new ValueSet.ComposeComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(compose.Extension),
                    ModifierExtension = Convert<Extension, R4Model.Extension>(compose.ModifierExtension),
                    Exclude = Convert<ValueSet.ConceptSetComponent, R4Model.ValueSet.ConceptSetComponent>(compose.Exclude),
                    InactiveElement = ConvertElement(compose.InactiveElement) as FhirBoolean,
                    Include = Convert<ValueSet.ConceptSetComponent, R4Model.ValueSet.ConceptSetComponent>(compose.Include),
                    LockedDateElement = ConvertElement(compose.LockedDateElement) as Date
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
                    SystemElement = ConvertElement(conceptSet.SystemElement) as FhirUri,
                    ValueSetElement = conceptSet.ValueSetElement.Select(vs => ConvertToFhirUri(vs)).ToList(),
                    VersionElement = ConvertElement(conceptSet.VersionElement) as FhirString
                };
            }
            else if (element is R4Model.ValueSet.ConceptReferenceComponent concept)
            {
                return new ValueSet.ConceptReferenceComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(concept.Extension),
                    ModifierExtension = Convert<Extension, R4Model.Extension>(concept.ModifierExtension),
                    CodeElement = ConvertElement(concept.CodeElement) as Code,
                    Designation = Convert<ValueSet.DesignationComponent, R4Model.ValueSet.DesignationComponent>(concept.Designation),
                    DisplayElement = ConvertElement(concept.DisplayElement) as FhirString
                };
            }
            else if (element is R4Model.ValueSet.FilterComponent filter)
            {
                return new ValueSet.FilterComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(filter.Extension),
                    ModifierExtension = Convert<Extension, R4Model.Extension>(filter.ModifierExtension),
                    OpElement = ConvertPrimitive<FilterOperator, R4Model.FilterOperator>(filter.OpElement) as Code<FilterOperator>,
                    PropertyElement = ConvertElement(filter.PropertyElement) as Code,
                    ValueElement = ConvertToCode(filter.ValueElement)
                };
            }
            else if (element is R4Model.ValueSet.DesignationComponent designation)
            {
                return new ValueSet.DesignationComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(designation.Extension),
                    ModifierExtension = Convert<Extension, R4Model.Extension>(designation.ModifierExtension),
                    LanguageElement = ConvertElement(designation.LanguageElement) as Code,
                    Use = ConvertElement(designation.Use) as Coding,
                    ValueElement = ConvertElement(designation.ValueElement) as FhirString
                };
            }
            else if (element is R4Model.ValueSet.ExpansionComponent expansion)
            {
                return new ValueSet.ExpansionComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(expansion.Extension),
                    ModifierExtension = Convert<Extension, R4Model.Extension>(expansion.ModifierExtension),
                    Contains = Convert<ValueSet.ContainsComponent, R4Model.ValueSet.ContainsComponent>(expansion.Contains),
                    IdentifierElement = ConvertElement(expansion.IdentifierElement) as FhirUri,
                    OffsetElement = ConvertElement(expansion.OffsetElement) as Integer,
                    Parameter = Convert<ValueSet.ParameterComponent, R4Model.ValueSet.ParameterComponent>(expansion.Parameter),
                    TimestampElement = ConvertElement(expansion.TimestampElement) as FhirDateTime,
                    TotalElement = ConvertElement(expansion.TotalElement) as Integer
                };
            }
            else if (element is R4Model.ValueSet.ContainsComponent contains)
            {
                return new ValueSet.ContainsComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(contains.Extension),
                    ModifierExtension = Convert<Extension, R4Model.Extension>(contains.ModifierExtension),
                    AbstractElement = ConvertElement(contains.AbstractElement) as FhirBoolean,
                    CodeElement = ConvertElement(contains.CodeElement) as Code,
                    Contains = Convert<ValueSet.ContainsComponent, R4Model.ValueSet.ContainsComponent>(contains.Contains),
                    Designation = Convert<ValueSet.DesignationComponent, R4Model.ValueSet.DesignationComponent>(contains.Designation),
                    DisplayElement = ConvertElement(contains.DisplayElement) as FhirString,
                    InactiveElement = ConvertElement(contains.InactiveElement) as FhirBoolean,
                    SystemElement = ConvertElement(contains.SystemElement) as FhirUri,
                    VersionElement = ConvertElement(contains.VersionElement) as FhirString
                };
            }
            else if (element is R4Model.ValueSet.ParameterComponent parameter)
            {
                return new ValueSet.ParameterComponent
                {
                    Extension = Convert<Extension, R4Model.Extension>(parameter.Extension),
                    ModifierExtension = Convert<Extension, R4Model.Extension>(parameter.ModifierExtension),
                    NameElement = ConvertElement(parameter.NameElement) as FhirString,
                    Value = ConvertElement(parameter.Value)
                };
            }

            throw new UnknownFhirTypeException(element.GetType());
        }
    }
}
