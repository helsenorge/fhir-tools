extern alias R3;
extern alias R4;

using System;
using System.IO;
using FhirTool.Core;
using R3Model = R3::Hl7.Fhir.Model;
using R4Model = R4::Hl7.Fhir.Model;
using Xunit;

namespace FhirTool.Conversion.Tests
{
    public class FhirConverterTests : FhirTestsBase
    {
        [Fact]
        public void CanConvertQuestionnaireExampleFromR3ToR4()
        {
            FhirConverter convertor = new FhirConverter();
            R3Model.Questionnaire r3Questionnaire = ReadR3Resource(Path.Combine("TestData", "questionnaire-example-R3.json")) as R3Model.Questionnaire;
            R4Model.Questionnaire r4Questionnaire = convertor.ConvertResource<R4Model.Questionnaire, R3Model.Questionnaire>(r3Questionnaire);
            SerializeR4ResourceToDiskAsJson(r4Questionnaire, "questionnaire-example-R4.json");
        }

        [Fact]
        public void CanConvertQuestionnaire305FromR3ToR4()
        {
            FhirConverter convertor = new FhirConverter();
            R3Model.Questionnaire r3Questionnaire = ReadR3Resource(Path.Combine("TestData", "questionnaire-305-R3.json")) as R3Model.Questionnaire;
            var resource = convertor.ConvertResource<R4Model.Questionnaire, R3Model.Questionnaire>(r3Questionnaire);
            SerializeR4ResourceToDiskAsJson(resource, "questionnaire-305-R4.json");
        }

        [Fact]
        public void CanConvertQuestionnaireExampleFromR4ToR3()
        {
            FhirConverter convertor = new FhirConverter();
            R4Model.Questionnaire r4Questionnaire = ReadR4Resource(Path.Combine("TestData", "questionnaire-example-R4.json")) as R4Model.Questionnaire;
            R3Model.Questionnaire r3Questionnaire = convertor.ConvertResource<R3Model.Questionnaire, R4Model.Questionnaire>(r4Questionnaire);
            SerializeR3ResourceToDiskAsJson(r3Questionnaire, "questionnaire-example-R3.json");
        }

        [Fact]
        public void CanConvertQuestionnaire305FromR4ToR3()
        {
            FhirConverter convertor = new FhirConverter();
            R4Model.Questionnaire r4Questionnaire = ReadR4Resource(Path.Combine("TestData", "questionnaire-305-R4.json")) as R4Model.Questionnaire;
            R3Model.Questionnaire r3Questionnaire = convertor.ConvertResource<R3Model.Questionnaire, R4Model.Questionnaire>(r4Questionnaire);
            SerializeR3ResourceToDiskAsJson(r3Questionnaire, "questionnaire-305-R3.json");
        }

        [Theory]
        [InlineData(FhirVersion.R2, FhirVersion.R3)]
        [InlineData(FhirVersion.R3, FhirVersion.R2)]
        [InlineData(FhirVersion.R3, FhirVersion.R4)]
        [InlineData(FhirVersion.R4, FhirVersion.R3)]
        [InlineData(FhirVersion.R4, FhirVersion.R5)]
        [InlineData(FhirVersion.R5, FhirVersion.R4)]
        public void CanConvertBetweenFhirVersionWithOneHop(FhirVersion from, FhirVersion to)
        {
            FhirConverter converter = new FhirConverter();
            Assert.True(converter.CanConvertBetweenVersions(from, to));
        }

        [Theory]
        [InlineData(FhirVersion.R2, FhirVersion.R4)]
        [InlineData(FhirVersion.R4, FhirVersion.R2)]
        [InlineData(FhirVersion.R3, FhirVersion.R5)]
        [InlineData(FhirVersion.R5, FhirVersion.R3)]
        public void CannotConvertBetweenFhirVersionWithMoreThanOneHop(FhirVersion from, FhirVersion to)
        {
            FhirConverter converter = new FhirConverter();
            Assert.False(converter.CanConvertBetweenVersions(from, to));
        }

        [Theory]
        [InlineData(FhirVersion.R4, typeof(R4Model.Questionnaire))]
        [InlineData(FhirVersion.R4, typeof(R4Model.Patient))]
        [InlineData(FhirVersion.R4, typeof(R4Model.ValueSet))]
        [InlineData(FhirVersion.R3, typeof(R3Model.Questionnaire))]
        [InlineData(FhirVersion.R3, typeof(R3Model.Patient))]
        [InlineData(FhirVersion.R3, typeof(R3Model.ValueSet))]
        public void FhirVersionMatchesVersionOfFhirType(FhirVersion expected, Type fhirType)
        {
            FhirConverter converter = new FhirConverter();
            Assert.Equal(expected, converter.GetFhirVersion(fhirType));
        }

        [Theory]
        [InlineData(FhirVersion.R3)]
        [InlineData(FhirVersion.R4)]
        public void FhirVersionSupported(FhirVersion version)
        {
            FhirConverter converter = new FhirConverter();
            Assert.True(converter.IsVersionSupported(version));
        }

        [Theory]
        [InlineData(FhirVersion.R2)]
        [InlineData(FhirVersion.R5)]
        public void FhirVersionNotSupported(FhirVersion version)
        {
            FhirConverter converter = new FhirConverter();
            Assert.False(converter.IsVersionSupported(version));
        }
    }
}
