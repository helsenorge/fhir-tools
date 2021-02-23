/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

extern alias R3;
extern alias R4;

using R3Model = R3::Hl7.Fhir.Model;
using R4::Hl7.Fhir.Model;

using Xunit;
using System;
using Hl7.Fhir.Utility;
using FhirTool.Core;
using Hl7.Fhir.Model;

namespace FhirTool.Conversion.Tests
{
    public class BaseR3ToR4ConversionTests
    {
        [Fact]
        public void Can_ConvertElement_R3_Instant_To_R4_Instant()
        {
            var value = DateTimeOffset.UtcNow;
            var r3TypeInstance = new Instant(value);
            var r3ToR4Conversion = new FhirConverter(FhirVersion.R4, FhirVersion.R3);
            var r4TypeInstance = r3ToR4Conversion.Convert<Instant,Instant>(r3TypeInstance);
            Assert.NotNull(r4TypeInstance);
            Assert.Equal(value, r4TypeInstance.Value);
        }

        [Fact]
        public void Can_ConvertElement_R3_Time_To_R4_Time()
        {
            var value = "13:09:45";
            var r3TypeInstance = new Time(value);
            var r4TypeInstance = new FhirConverter(FhirVersion.R4, FhirVersion.R3).Convert<Time,Time>(r3TypeInstance);
            Assert.NotNull(r4TypeInstance);
            Assert.Equal(value, r4TypeInstance.Value);
        }

        [Fact]
        public void Can_ConvertElement_R3_Date_To_R4_Date()
        {
            var value = "2020-06-23";
            var r3TypeInstance = new Date(value);
            var r4TypeInstance = new FhirConverter(FhirVersion.R4, FhirVersion.R3)
                .Convert<Date,Date>(r3TypeInstance);
            Assert.NotNull(r4TypeInstance);
            Assert.Equal(value, r4TypeInstance.Value);
        }

        [Fact]
        public void Can_ConvertElement_R3_FhirDateTime_To_R4_FhirDateTime()
        {
            var value = "2022-06-23T00:00:00.000Z";
            var r3TypeInstance = new FhirDateTime(value);
            var r4TypeInstance = new FhirConverter(FhirVersion.R4, FhirVersion.R3)
                .Convert<FhirDateTime, FhirDateTime>(r3TypeInstance);
            Assert.NotNull(r4TypeInstance);
            Assert.Equal(value, r4TypeInstance.Value);
        }

        [Fact]
        public void Can_ConvertElement_R3_Base64Binary_To_R4_Base64Binary()
        {
            var value = System.Convert.FromBase64String("VGhpcyB0ZXN0IGNhc2UgdmVyaWZpZXMgdGhhdCB3ZSBjYW4gY29udmVydCBmcm9tIGFuIFIzIGluc3RhbmNlIG9mIEJhc2U2NEJpbmFyeSB0byBhbiBSNCBpbnN0YW5jZSBvZiBCYXNlNjRCaW5hcnk=");
            var r3TypeInstance = new Base64Binary(value);
            var r4TypeInstance = new FhirConverter(FhirVersion.R4, FhirVersion.R3)
                .Convert<Base64Binary,Base64Binary>(r3TypeInstance);
            Assert.NotNull(r4TypeInstance);
            Assert.Equal(value, r4TypeInstance.Value);
        }

        [Fact]
        public void Can_ConvertElement_R3_FhirDecimal_To_R4_FhirDecimal()
        {
            var value = 10.7564785m;
            var r3TypeInstance = new FhirDecimal(value);
            var r4TypeInstance = new FhirConverter(FhirVersion.R4, FhirVersion.R3)
                .Convert<FhirDecimal,FhirDecimal>(r3TypeInstance);
            Assert.NotNull(r4TypeInstance);
            Assert.Equal(value, r4TypeInstance.Value);
        }

        [Fact]
        public void Can_ConvertElement_R3_FhirBoolean_To_R4_FhirBoolean()
        {
            var value = true;
            var r3TypeInstance = new FhirBoolean(value);
            var r4TypeInstance = new FhirConverter(FhirVersion.R4, FhirVersion.R3)
                .Convert<FhirBoolean,FhirBoolean>(r3TypeInstance);
            Assert.NotNull(r4TypeInstance);
            Assert.Equal(value, r4TypeInstance.Value);
        }

        [Fact]
        public void Can_ConvertElement_R3_Code_To_R4_Code()
        {
            var value = "J-001";
            var r3TypeInstance = new Code(value);
            var r4TypeInstance = new FhirConverter(FhirVersion.R4, FhirVersion.R3)
                .Convert<Code, Code>(r3TypeInstance);
            Assert.NotNull(r4TypeInstance);
            Assert.Equal(value, r4TypeInstance.Value);
        }

        [Fact]
        public void Can_ConvertElement_R3_FhirString_To_R4_FhirString()
        {
            var value = "This is a FhirString converted from R3 to R4";
            var r3TypeInstance = new FhirString(value);
            var r4TypeInstance = new FhirConverter(FhirVersion.R4, FhirVersion.R3)
                .Convert<FhirString,FhirString>(r3TypeInstance);
            Assert.NotNull(r4TypeInstance);
            Assert.Equal(value, r4TypeInstance.Value);
        }

        [Fact]
        public void Can_ConvertElement_R3_Integer_To_R4_Integer()
        {
            var value = 42;
            var r3TypeInstance = new Integer(value);
            var r4TypeInstance = new FhirConverter(FhirVersion.R4, FhirVersion.R3)
                .Convert<Integer,Integer>(r3TypeInstance);
            Assert.NotNull(r4TypeInstance);
            Assert.Equal(value, r4TypeInstance.Value);
        }

        [Fact]
        public void Can_ConvertElement_R3_FhirUri_To_R4_FhirUri()
        {
            var value = "https://helsenorge.no";
            var r3TypeInstance = new FhirUri(value);
            var r4TypeInstance = new FhirConverter(FhirVersion.R4, FhirVersion.R3)
                .Convert<FhirUri, FhirUri>(r3TypeInstance);
            Assert.NotNull(r4TypeInstance);
            Assert.Equal(value, r4TypeInstance.Value);
        }

        [Fact]
        public void Can_ConvertElement_R3_Id_To_R4_Id()
        {
            var value = "9ed040e6-e75c-4c76-bb65-40c147e4fce0";
            var r3TypeInstance = new Id(value);
            var r4TypeInstance = new FhirConverter(FhirVersion.R4, FhirVersion.R3)
                .Convert<Id, Id>(r3TypeInstance);
            Assert.NotNull(r4TypeInstance);
            Assert.Equal(value, r4TypeInstance.Value);
        }

        [Fact]
        public void Can_ConvertElement_R3_Oid_To_R4_Oid()
        {
            var value = "urn:oid:2.16.578.1.12.4.1.1";
            var r3TypeInstance = new Oid(value);
            var r4TypeInstance = new FhirConverter(FhirVersion.R4, FhirVersion.R3)
                .Convert<Oid, Oid>(r3TypeInstance);
            Assert.NotNull(r4TypeInstance);
            Assert.Equal(value, r4TypeInstance.Value);
        }

        [Fact]
        public void Can_ConvertElement_R3_Uuid_To_R4_Uuid()
        {
            var value = "9ed040e6-e75c-4c76-bb65-40c147e4fce0";
            var r3TypeInstance = new Uuid(value);
            var r4TypeInstance = new FhirConverter(FhirVersion.R4, FhirVersion.R3)
                .Convert<Uuid, Uuid>(r3TypeInstance);
            Assert.NotNull(r4TypeInstance);
            Assert.Equal(value, r4TypeInstance.Value);
        }

        [Fact]
        public void Can_ConvertElement_R3_UnsignedInt_To_R4_UnsignedInt()
        {
            var value = 0;
            var r3TypeInstance = new UnsignedInt(value);
            var r4TypeInstance = new FhirConverter(FhirVersion.R4, FhirVersion.R3)
                .Convert<UnsignedInt, UnsignedInt>(r3TypeInstance);
            Assert.NotNull(r4TypeInstance);
            Assert.Equal(value, r4TypeInstance.Value);
        }

        [Fact]
        public void Can_ConvertElement_R3_PositiveInt_To_R4_PositiveInt()
        {
            var value = 1;
            var r3TypeInstance = new PositiveInt(value);
            var r4TypeInstance = new FhirConverter(FhirVersion.R4, FhirVersion.R3)
                .Convert<PositiveInt, PositiveInt>(r3TypeInstance);
            Assert.NotNull(r4TypeInstance);
            Assert.Equal(value, r4TypeInstance.Value);
        }

        [Fact]
        public void Can_ConvertElement_R3_Ratio_To_R4_Ratio()
        {
            var r3TypeInstance = new R3Model.Ratio
            {
                Numerator = new R3Model.Quantity
                {
                    Value = 103.50m,
                    Unit = "US$",
                    Code = "USD",
                    System = "urn:iso:std:iso:4217"
                },
                Denominator = new R3Model.Quantity
                {
                    Value = 1,
                    Unit = "day",
                    Code = "day",
                    System = "http://unitsofmeasure.org"
                }
            };
            var r4TypeInstance = new FhirConverter(FhirVersion.R4, FhirVersion.R3)
                .Convert<Ratio, R3Model.Ratio>(r3TypeInstance);
            Assert.NotNull(r4TypeInstance);
            Assert.Equal(r3TypeInstance.Numerator.Value, r4TypeInstance.Numerator.Value);
            Assert.Equal(r3TypeInstance.Numerator.Unit, r4TypeInstance.Numerator.Unit);
            Assert.Equal(r3TypeInstance.Numerator.Code, r4TypeInstance.Numerator.Code);
            Assert.Equal(r3TypeInstance.Numerator.System, r4TypeInstance.Numerator.System);
            Assert.Equal(r3TypeInstance.Denominator.Value, r4TypeInstance.Denominator.Value);
            Assert.Equal(r3TypeInstance.Denominator.Unit, r4TypeInstance.Denominator.Unit);
            Assert.Equal(r3TypeInstance.Denominator.Code, r4TypeInstance.Denominator.Code);
            Assert.Equal(r3TypeInstance.Denominator.System, r4TypeInstance.Denominator.System);
        }

        [Fact]
        public void Can_ConvertElement_R3_Period_To_R4_Period()
        {
            var r3TypeInstance = new Period
            {
                Start = "2020-06-23",
                End = "2020-06-27"
            };
            var r4TypeInstance = new FhirConverter(FhirVersion.R4, FhirVersion.R3)
                .Convert<Period, Period>(r3TypeInstance);
            Assert.NotNull(r4TypeInstance);
            Assert.Equal(r3TypeInstance.Start, r4TypeInstance.Start);
            Assert.Equal(r3TypeInstance.End, r4TypeInstance.End);
        }

        [Fact]
        public void Can_ConvertElement_R3_Range_To_R4_Range()
        {
            var r3TypeInstance = new R3Model.Range
            {
                Low = new R3Model.Quantity
                {
                    Value = 1.6m,
                    Unit = "m"
                },
                High = new R3Model.Quantity
                {
                    Value = 1.9m,
                    Unit = "m"
                }
            };
            var r4TypeInstance = new FhirConverter(FhirVersion.R4, FhirVersion.R3)
                .Convert<R4::Hl7.Fhir.Model.Range, R3Model.Range>(r3TypeInstance);
            Assert.NotNull(r4TypeInstance);
            Assert.Equal(r3TypeInstance.Low.Value, r4TypeInstance.Low.Value);
            Assert.Equal(r3TypeInstance.Low.Unit, r4TypeInstance.Low.Unit);
            Assert.Equal(r3TypeInstance.High.Value, r4TypeInstance.High.Value);
            Assert.Equal(r3TypeInstance.High.Unit, r4TypeInstance.High.Unit);
        }

        [Fact]
        public void Can_ConvertElement_R3_Attachment_To_R4_Attachment()
        {
            var r3TypeInstance = new R3Model.Attachment
            {
                ContentType = "application/pdf",
                Data = System.Convert.FromBase64String("JVBERi0xLjcKCjEgMCBvYmogICUgZW50cnkgcG9pbnQKPDwKICAvVHlwZSAvQ2F0YWxvZwogIC9QYWdlcyAyIDAgUgo+PgplbmRvYmoKCjIgMCBvYmoKPDwKICAvVHlwZSAvUGFnZXMKICAvTWVkaWFCb3ggWyAwIDAgMjAwIDIwMCBdCiAgL0NvdW50IDEKICAvS2lkcyBbIDMgMCBSIF0KPj4KZW5kb2JqCgozIDAgb2JqCjw8CiAgL1R5cGUgL1BhZ2UKICAvUGFyZW50IDIgMCBSCiAgL1Jlc291cmNlcyA8PAogICAgL0ZvbnQgPDwKICAgICAgL0YxIDQgMCBSIAogICAgPj4KICA+PgogIC9Db250ZW50cyA1IDAgUgo+PgplbmRvYmoKCjQgMCBvYmoKPDwKICAvVHlwZSAvRm9udAogIC9TdWJ0eXBlIC9UeXBlMQogIC9CYXNlRm9udCAvVGltZXMtUm9tYW4KPj4KZW5kb2JqCgo1IDAgb2JqICAlIHBhZ2UgY29udGVudAo8PAogIC9MZW5ndGggNDQKPj4Kc3RyZWFtCkJUCjcwIDUwIFRECi9GMSAxMiBUZgooSGVsbG8sIHdvcmxkISkgVGoKRVQKZW5kc3RyZWFtCmVuZG9iagoKeHJlZgowIDYKMDAwMDAwMDAwMCA2NTUzNSBmIAowMDAwMDAwMDEwIDAwMDAwIG4gCjAwMDAwMDAwNzkgMDAwMDAgbiAKMDAwMDAwMDE3MyAwMDAwMCBuIAowMDAwMDAwMzAxIDAwMDAwIG4gCjAwMDAwMDAzODAgMDAwMDAgbiAKdHJhaWxlcgo8PAogIC9TaXplIDYKICAvUm9vdCAxIDAgUgo+PgpzdGFydHhyZWYKNDkyCiUlRU9G"),
                Title = "Example PDF"
            };
            var r4TypeInstance = new FhirConverter(FhirVersion.R4, FhirVersion.R3)
                .Convert<Attachment, R3Model.Attachment>(r3TypeInstance);
            Assert.NotNull(r4TypeInstance);
            Assert.Equal(r3TypeInstance.ContentType, r4TypeInstance.ContentType);
            Assert.Equal(r3TypeInstance.Data, r4TypeInstance.Data);
            Assert.Equal(r3TypeInstance.Title, r4TypeInstance.Title);
        }

        [Fact]
        public void Can_ConvertElement_R3_Identifier_To_R4_Identifier()
        {
            var r3TypeInstance = new Identifier
            {
                Use = Identifier.IdentifierUse.Official,
                System = "http://www.acmehosp.com/patients",
                Value = "44552",
                Period = new Period
                {
                    Start = "2003-05-03"
                }
            };
            var r4TypeInstance = new FhirConverter(FhirVersion.R4, FhirVersion.R3)
                .Convert<Identifier, Identifier>(r3TypeInstance);
            Assert.NotNull(r4TypeInstance);
            Assert.Equal(r3TypeInstance.Use.GetLiteral(), r4TypeInstance.Use.GetLiteral());
            Assert.Equal(r3TypeInstance.System, r4TypeInstance.System);
            Assert.Equal(r3TypeInstance.Value, r4TypeInstance.Value);
        }

        [Fact]
        public void Can_ConvertElement_R3_Annotation_To_R4_Annotation()
        {
            var r3TypeInstance = new R3Model.Annotation
            {
                Author = new FhirString("Kenneth Myhra"),
                Time = "20:36",
                Text = "This is the annotation"
            };
            var r4TypeInstance = new FhirConverter(FhirVersion.R4, FhirVersion.R3)
                .Convert<Annotation, R3Model.Annotation>(r3TypeInstance);
            Assert.NotNull(r4TypeInstance);
            Assert.Equal(((FhirString)r3TypeInstance.Author).Value, ((FhirString)r4TypeInstance.Author).Value);
            Assert.Equal(r3TypeInstance.Time, r4TypeInstance.Time);
            Assert.Equal(r3TypeInstance.Text, r4TypeInstance.Text?.Value);
        }

        [Fact]
        public void Can_ConvertElement_R3_Money_To_R4_Money()
        {
            var r3TypeInstance = new R3Model.Money
            {
                Value = 24.45m,
                Unit = "US$",
                System = "urn:iso:std:iso:4217",
                Code = "USD"
            };
            var r4TypeInstance = new FhirConverter(FhirVersion.R4, FhirVersion.R3)
                .Convert<Money, R3Model.Money>(r3TypeInstance);
            Assert.NotNull(r4TypeInstance);
            Assert.Equal(r3TypeInstance.Value, r4TypeInstance.Value);
            Assert.Equal(Money.Currencies.USD, r4TypeInstance.Currency);
        }

        //[Fact]
        //public void Can_ConvertElement_R3_HumanName_To_R4_HumanName()
        //{
        //    var r3TypeInstance = new R3Model.HumanName
        //    {
        //        Use = R3Model.HumanName.NameUse.Official,
        //        Family = "von Hochheim-Weilenfel",
        //        Given = new[] { "Peter", "James" }
        //    };
        //    var r4TypeInstance = new FhirR3ToR4ConversionRoutines()
        //        .ConvertElement(r3TypeInstance) as Annotation;
        //    Assert.NotNull(r4TypeInstance);
        //    Assert.Equal(((R3Model.FhirString)r3TypeInstance.Author).Value, ((FhirString)r4TypeInstance.Author).Value);
        //    Assert.Equal(r3TypeInstance.Time, r4TypeInstance.Time);
        //    Assert.Equal(r3TypeInstance.Text, r4TypeInstance.Text?.Value);
        //}
    }
}