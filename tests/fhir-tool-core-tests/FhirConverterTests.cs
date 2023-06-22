/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

extern alias R3;
extern alias R4;

using System;
using FhirTool.Core;
using R3Model = R3::Hl7.Fhir.Model;
using R4Model = R4::Hl7.Fhir.Model;
using Xunit;
using FhirTool.Core.FhirWrappers;
using System.Threading.Tasks;
using System.IO;
using Hl7.Fhir.Model;

namespace FhirTool.Conversion.Tests
{
    public class FhirConverterTests : FhirTestsBase
    {
        [Theory]
        [InlineData(@"TestData/questionnaire-example-r3.json")]
        [InlineData(@"TestData/questionnaire-305-r3.json")]
        [InlineData(@"TestData/account.profile-r3.json")]
        [InlineData(@"TestData/activitydefinition.profile-r3.json")]
        [InlineData(@"TestData/adverseevent.profile-r3.json")]
        [InlineData(@"TestData/allergyintolerance.profile-r3.json")]
        [InlineData(@"TestData/appointment.profile-r3.json")]
        [InlineData(@"TestData/appointmentresponse.profile-r3.json")]
        [InlineData(@"TestData/consent.profile-r3.json")]
        public async Task CanConvert_Resource_FromR3ToR4_RoundTrip(string path)
        {
            var converterFromR3ToR4 = new FhirConverter(to: FhirVersion.R4, from: FhirVersion.R3);
            var converterFromR4ToR3 = new FhirConverter(to: FhirVersion.R3, from: FhirVersion.R4);
            
            var r3Serializer = new SerializationWrapper(FhirVersion.R3);
            var r3Resource = r3Serializer.Parse(await File.ReadAllTextAsync(path));

            var r4Resource = converterFromR3ToR4.Convert<Resource, Resource>(r3Resource.Resource);
            var r3ResourceRoundTrip = converterFromR4ToR3.Convert<Resource, Resource>(r4Resource);

            var r3ResourceContent = r3Serializer.Serialize(r3Resource, FhirMimeType.Json);
            var r3ResourceRoundTripContent = r3Serializer.Serialize(r3ResourceRoundTrip, FhirMimeType.Json);

            Assert.Equal(r3ResourceContent, r3ResourceRoundTripContent);
        }

        [Theory]
        [InlineData(@"TestData/questionnaire-example-r4.json")]
        [InlineData(@"TestData/questionnaire-305-r4.json")]
        //[InlineData(@"TestData\account.profile-r4.json")]
        //[InlineData(@"TestData\activitydefinition.profile-r4.json")]
        //[InlineData(@"TestData\adverseevent.profile-r4.json")]
        //[InlineData(@"TestData\allergyintolerance.profile-r4.json")]
        //[InlineData(@"TestData\appointment.profile-r4.json")]
        //[InlineData(@"TestData\appointmentresponse.profile-r4.json")]
        //[InlineData(@"TestData\consent.profile-r4.json")]
        public async Task CanConvert_Questionnaire_FromR4ToR3_RoundTrip(string path)
        {
            var converterFromR4ToR3 = new FhirConverter(to: FhirVersion.R3, from: FhirVersion.R4);
            var converterFromR3ToR4 = new FhirConverter(to: FhirVersion.R4, from: FhirVersion.R3);
            
            var r4Serializer = new SerializationWrapper(FhirVersion.R4);
            var r4Resource = r4Serializer.Parse(await File.ReadAllTextAsync(path));

            var r3Resource = converterFromR4ToR3.Convert<Resource, Resource>(r4Resource.Resource);
            var r4ResourceRoundTrip = converterFromR3ToR4.Convert<Resource, Resource>(r3Resource);

            var r4ResourceContent = r4Serializer.Serialize(r4Resource);
            var r4ResourceRoundTripContent = r4Serializer.Serialize(r4ResourceRoundTrip);

            Assert.Equal(r4ResourceContent, r4ResourceRoundTripContent);
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
