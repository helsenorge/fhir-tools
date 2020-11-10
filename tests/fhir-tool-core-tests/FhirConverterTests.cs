extern alias R3;
extern alias R4;

using System;
using FhirTool.Core;
using R3Model = R3::Hl7.Fhir.Model;
using R4Model = R4::Hl7.Fhir.Model;
using Xunit;
using R3Serialization = R3::Hl7.Fhir.Serialization;
using R4Serialization = R4::Hl7.Fhir.Serialization;

namespace FhirTool.Conversion.Tests
{
    public class FhirConverterTests : FhirTestsBase
    {
        [Theory]
        [InlineData(@"TestData\questionnaire-example-R3.json")]
        [InlineData(@"TestData\questionnaire-305-R3.json")]
        public void CanConvert_Questionnaire_FromR3ToR4_RoundTrip(string path)
        {
            var converterFromR3ToR4 = new FhirConverter(to: FhirVersion.R4, from: FhirVersion.R3);
            var converterFromR4ToR3 = new FhirConverter(to: FhirVersion.R3, from: FhirVersion.R4);
            var r3Serializer = new R3Serialization.FhirJsonSerializer();

            var r3Questionnaire = ReadR3Resource(path) as R3Model.Questionnaire;
            var r4Questionnaire = converterFromR3ToR4.Convert<R4Model.Questionnaire, R3Model.Questionnaire>(r3Questionnaire);
            var r3QuestionnaireRoundTrip = converterFromR4ToR3.Convert<R3Model.Questionnaire, R4Model.Questionnaire>(r4Questionnaire);

            var r3QuestionnaireContent = r3Serializer.SerializeToString(r3Questionnaire);
            var r3QuestionnaireRoundTripContent = r3Serializer.SerializeToString(r3QuestionnaireRoundTrip);

            Assert.Equal(r3QuestionnaireContent, r3QuestionnaireRoundTripContent);
        }

        [Theory]
        [InlineData(@"TestData\questionnaire-example-R4.json")]
        [InlineData(@"TestData\questionnaire-305-R4.json")]
        public void CanConvert_Questionnaire_FromR4ToR3_RoundTrip(string path)
        {
            var converterFromR4ToR3 = new FhirConverter(to: FhirVersion.R3, from: FhirVersion.R4);
            var converterFromR3ToR4 = new FhirConverter(to: FhirVersion.R4, from: FhirVersion.R3);
            var r4Serializer = new R4Serialization.FhirJsonSerializer();

            var r4Questionnaire = ReadR4Resource(path) as R4Model.Questionnaire;
            var r3Questionnaire = converterFromR4ToR3.Convert<R3Model.Questionnaire, R4Model.Questionnaire>(r4Questionnaire);
            var r4QuestionnaireRoundTrip = converterFromR3ToR4.Convert<R4Model.Questionnaire, R3Model.Questionnaire>(r3Questionnaire);

            var r4QuestionnaireContent = r4Serializer.SerializeToString(r4Questionnaire);
            var r4QuestionnaireRoundTripContent = r4Serializer.SerializeToString(r4QuestionnaireRoundTrip);

            Assert.Equal(r4QuestionnaireContent, r4QuestionnaireRoundTripContent);
        }

        [Theory]
        [InlineData(FhirVersion.R2, FhirVersion.R3)]
        [InlineData(FhirVersion.R3, FhirVersion.R2)]
        [InlineData(FhirVersion.R3, FhirVersion.R4)]
        [InlineData(FhirVersion.R4, FhirVersion.R3)]
        [InlineData(FhirVersion.R4, FhirVersion.R5)]
        [InlineData(FhirVersion.R5, FhirVersion.R4)]
        public void CanConvert_Between_FhirVersion_WithOneHop(FhirVersion from, FhirVersion to)
        {
            Assert.True(FhirConverter.CanConvertBetweenVersions(from, to));
        }

        [Theory]
        [InlineData(FhirVersion.R2, FhirVersion.R4)]
        [InlineData(FhirVersion.R4, FhirVersion.R2)]
        [InlineData(FhirVersion.R3, FhirVersion.R5)]
        [InlineData(FhirVersion.R5, FhirVersion.R3)]
        public void CannotConvert_Between_FhirVersion_WithMoreThanOneHop(FhirVersion from, FhirVersion to)
        {
            Assert.False(FhirConverter.CanConvertBetweenVersions(from, to));
        }

        [Theory]
        [InlineData(FhirVersion.R4, typeof(R4Model.Questionnaire))]
        [InlineData(FhirVersion.R4, typeof(R4Model.Patient))]
        [InlineData(FhirVersion.R4, typeof(R4Model.ValueSet))]
        [InlineData(FhirVersion.R3, typeof(R3Model.Questionnaire))]
        [InlineData(FhirVersion.R3, typeof(R3Model.Patient))]
        [InlineData(FhirVersion.R3, typeof(R3Model.ValueSet))]
        public void FhirVersion_Matches_Version_Of_FhirType(FhirVersion expected, Type fhirType)
        {
            Assert.Equal(expected, FhirConverter.GetFhirVersion(fhirType));
        }

        [Theory]
        [InlineData(FhirVersion.R3)]
        [InlineData(FhirVersion.R4)]
        public void Is_FhirVersion_Supported(FhirVersion version)
        {
            Assert.True(FhirConverter.IsVersionSupported(version));
        }

        [Theory]
        [InlineData(FhirVersion.R2)]
        [InlineData(FhirVersion.R5)]
        public void FhirVersion_Is_Not_Supported(FhirVersion version)
        {
            Assert.False(FhirConverter.IsVersionSupported(version));
        }
    }
}
