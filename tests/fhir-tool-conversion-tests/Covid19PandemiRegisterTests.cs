extern alias R4;
extern alias R3;

using R4Model = R4::Hl7.Fhir.Model;
using R4Serialization = R4::Hl7.Fhir.Serialization;
using R3::Hl7.Fhir.Model;
using FhirTool.Core;
using System.IO;
using Xunit;
using System.Collections.Generic;
using Hl7.Fhir.Utility;

namespace FhirTool.Conversion.Tests
{
    public class Covid19PandemiRegisterTests
    {
        private readonly R4Serialization.FhirXmlParser _r4XmlParser = new R4Serialization.FhirXmlParser(new R4Serialization.ParserSettings { PermissiveParsing = true });
        private readonly R4Serialization.FhirJsonParser _r4JsonParser = new R4Serialization.FhirJsonParser(new R4Serialization.ParserSettings { PermissiveParsing = true });

        [Fact]
        public void CreateCovid19PandemiRegister()
        {
            string path = Path.Combine(".", "TestData", "covid-19-pandemiregister", "questionnaire-Rapport-R4.xml");
            string xml;
            using (TextReader reader = new StreamReader(path))
                xml = reader.ReadToEnd();
            R4Model.Questionnaire r4Questionnaire = _r4XmlParser.Parse<R4Model.Questionnaire>(xml);

            FhirConverter converter = new FhirConverter();
            Questionnaire questionnaire = converter.ConvertResource<Questionnaire, R4Model.Questionnaire>(r4Questionnaire);

            ValueSet valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-_testname_en_-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-agens_at0043-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-agens-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-analysenavn_at0024-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-analysenavn-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-bildediagnostiskdiagnose-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-eksponeringssituasjon_at0045-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-eksponeringssituasjon-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-erdetnoentilstandertilstede_-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-innenlands_utenlands-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-innleggelsefra-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-iverksatt__at0005-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-iverksatt_-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-legemiddelibruk__at0024-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-legemiddelibruk_-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-legemiddelklasseibruk_-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-legemiddelklasse-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-legemiddelnavn_at0021-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-legemiddelnavn-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-navnpaaktiviteten_at0004-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-navnpaaktiviteten-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-navnpatilstand_at0004-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-navnpatilstand-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-reise_-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-tilstede__at0005-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-tilstede_-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-tilstedeværelse_at0046-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "covid-19-pandemiregister", "ValueSet-report-v1-tilstedeværelse-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            FixContainedValueSetReferences(questionnaire.Contained.FindAll(r => r.ResourceType == ResourceType.ValueSet), questionnaire.Item);

            questionnaire.SerializeResourceToDiskAsXml($"questionnaire-Covid19-Pandemiregister.xml");
            questionnaire.SerializeResourceToDiskAsJson("questionnaire-Covid19-Pandemiregister.json");
        }

        [Fact]
        public void CanR4ResourceConvertToR3()
        {
            // questionnaire-Bloodpressure-R4.xml
            string path = Path.Combine(".", "TestData" ,"questionnaire-Bloodpressure-R4.xml");
            string xml;
            using (TextReader reader = new StreamReader(path))
                xml = reader.ReadToEnd();
            R4Model.Questionnaire r4Questionnaire = _r4XmlParser.Parse<R4Model.Questionnaire>(xml);

            FhirConverter converter = new FhirConverter();
            Questionnaire questionnaire = converter.ConvertResource<Questionnaire, R4Model.Questionnaire>(r4Questionnaire);

            // ValueSet-blood-pressure-v2-cuffsize-R4.xml
            ValueSet valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "ValueSet-blood-pressure-v2-cuffsize-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            // ValueSet-blood-pressure-v2-diastolicendpoint-R4.xml
            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "ValueSet-blood-pressure-v2-diastolicendpoint-R4.xml"));
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            // ValueSet-blood-pressure-v2-locationofmeasurement-R4.xml
            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "ValueSet-blood-pressure-v2-locationofmeasurement-R4.xml"));
            using (TextReader reader = new StreamReader(path))
                xml = reader.ReadToEnd();
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            // ValueSet-blood-pressure-v2-method-R4.xml
            valueSet = ConvertValueSetFromR4ToR3(Path.Combine(".", "TestData", "ValueSet-blood-pressure-v2-method-R4.xml"));
            using (TextReader reader = new StreamReader(path))
                xml = reader.ReadToEnd();
            valueSet = RemoveDesignation(valueSet);
            questionnaire.Contained.Add(valueSet);

            FixContainedValueSetReferences(questionnaire.Contained.FindAll(r => r.ResourceType == ResourceType.ValueSet), questionnaire.Item);

            questionnaire.SerializeResourceToDiskAsXml("questionnaire-Bloodpressure-R3.xml");
            questionnaire.SerializeResourceToDiskAsJson("questionnaire-Bloodpressure-R3.json");
        }

        [Fact]
        public void CreateGenereltBlodTrykk()
        {
            string path = Path.Combine(".", "TestData", "Generelt blodtrykk", "questionnaire-Blodtrykk-R4.xml");
            string xml;
            using (TextReader reader = new StreamReader(path))
                xml = reader.ReadToEnd();
            R4Model.Questionnaire r4Questionnaire = _r4XmlParser.Parse<R4Model.Questionnaire>(xml);
            
            FhirConverter converter = new FhirConverter();
            Questionnaire questionnaire = converter.ConvertResource<Questionnaire, R4Model.Questionnaire>(r4Questionnaire);

            questionnaire.SerializeResourceToDiskAsXml("questionnaire-Blodtrykk-R3.xml");
            questionnaire.SerializeResourceToDiskAsJson("questionnaire-Blodtrykk-R3.json");
        }

        private bool AddType(List<Questionnaire.ItemComponent> items, string linkId, Questionnaire.QuestionnaireItemType type, Coding unitCoding = null)
        {
            bool result = false;

            foreach(var item in items)
            {
                if(item.LinkId == linkId)
                {
                    item.Type = type;

                    if(unitCoding != null)
                        item.SetExtension(Constants.QuestionnaireUnitUri, unitCoding);

                    return true;
                }

                if(item.Item != null && item.Item.Count > 0)
                {
                    result = AddType(item.Item, linkId, type, unitCoding);
                }

                if (result) break;
            }

            return result;
        }

        private void FixContainedValueSetReferences(IEnumerable<Resource> valueSets, IEnumerable<Questionnaire.ItemComponent> items)
        {
            foreach(var item in items)
            {
                if (item.Type == Questionnaire.QuestionnaireItemType.Choice)
                {
                    var options = item.Options;
                    if (options != null)
                    {
                        foreach (var resource in valueSets)
                        {
                            if (resource.ResourceType != ResourceType.ValueSet) continue;
                            var valueSet = resource as ValueSet;

                            if (options.Reference.EndsWith(valueSet.Id))
                            {
                                item.Options.Reference = $"#{valueSet.Id}";
                            }
                        }
                    }
                }

                FixContainedValueSetReferences(valueSets, item.Item);
            }
        }

        private ValueSet RemoveDesignation(ValueSet valueSet)
        {
            foreach(var include in valueSet.Compose.Include)
            {
                foreach(var concept in include.Concept)
                {
                    concept.Designation.RemoveAll(d => true);
                }
            }
            return valueSet;
        }

        private ValueSet ConvertValueSetFromR4ToR3(string path)
        {
            string data;
            using (TextReader reader = new StreamReader(path))
                data = reader.ReadToEnd();

            R4Model.ValueSet valueSet = null;
            if (SerializationUtil.ProbeIsXml(data))
                valueSet = _r4XmlParser.Parse<R4Model.ValueSet>(data);
            else if (SerializationUtil.ProbeIsJson(data))
                valueSet = _r4JsonParser.Parse<R4Model.ValueSet>(data);
            else
                Assert.True(false, $"File '{path}' does not contain valid Xml or Json");

            if (valueSet == null) return null;

            FhirConverter converter = new FhirConverter();
            return converter.ConvertResource<ValueSet, R4Model.ValueSet>(valueSet);
        }
    }

}
