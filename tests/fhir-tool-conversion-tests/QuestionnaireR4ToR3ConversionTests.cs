extern alias R3;


using Hl7.Fhir.ElementModel;
using R3::Hl7.Fhir.Model;
using R3::Hl7.Fhir.Serialization;
using FhirTool.Core;
using Hl7.Fhir.Serialization;
using System.IO;
using Xunit;
using System.Collections.Generic;
using Hl7.Fhir.Utility;

namespace FhirTool.Conversion.Tests
{
    public class QuestionnaireR4ToR3ConversionTests
    {
        [Fact]
        public void CreateCovid19PandemiRegister()
        {
            string path = Path.Combine(".", "TestData", "covid-19-pandemiregister", "questionnaire-Rapport-R4.xml");
            string xml;
            using (TextReader reader = new StreamReader(path))
                xml = reader.ReadToEnd();
            ISourceNode sourceNode = FhirXmlNode.Parse(xml);
            QuestionnaireR4ToR3Conversion questionnaireR4ToR3Conversion = new QuestionnaireR4ToR3Conversion();
            Questionnaire questionnaire = questionnaireR4ToR3Conversion.Convert(sourceNode);

            var parser = new FhirJsonParser(new ParserSettings { PermissiveParsing = false });

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

            ISourceNode sourceNode = FhirXmlNode.Parse(xml);
            QuestionnaireR4ToR3Conversion questionnaireR4ToR3Conversion = new QuestionnaireR4ToR3Conversion();
            Questionnaire questionnaire = questionnaireR4ToR3Conversion.Convert(sourceNode);

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

            questionnaire.SerializeResourceToDiskAsXml($"{questionnaire.Name}.xml");
        }

        [Fact]
        public void CreateGenereltBlodTrykk()
        {
            string path = Path.Combine(".", "TestData", "Generelt blodtrykk", "questionnaire-Blodtrykk-R4.xml");
            string xml;
            using (TextReader reader = new StreamReader(path))
                xml = reader.ReadToEnd();

            ISourceNode sourceNode = FhirXmlNode.Parse(xml);
            QuestionnaireR4ToR3Conversion questionnaireR4ToR3Conversion = new QuestionnaireR4ToR3Conversion();
            questionnaireR4ToR3Conversion.Convert(sourceNode);
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

            ISourceNode valueSet = null;
            if (SerializationUtil.ProbeIsXml(data))
                valueSet = FhirXmlNode.Parse(data);
            else if (SerializationUtil.ProbeIsJson(data))
                valueSet = FhirJsonNode.Parse(data);
            else
                Assert.True(false, $"File '{path}' does not contain valid Xml or Json");

            if (valueSet == null) return null;

            return ConvertValueSetFromR4ToR3(valueSet);
        }

        private ValueSet ConvertValueSetFromR4ToR3(ISourceNode valueSet)
        {
            ValueSetR4ToR3Conversion converter = new ValueSetR4ToR3Conversion();
            return converter.Convert(valueSet);
        }
    }

}
