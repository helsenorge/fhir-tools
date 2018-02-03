using FhirTool.Extensions;
using FhirTool.Model;
using FhirTool.Model.FlatFile;
using FileHelpers.MasterDetail;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FhirTool
{
    class Program
    {
        public const string EntryFormatUri = "http://hl7.org/fhir/StructureDefinition/entryFormat";
        public const string MaxDecimalPlacesUri = "http://hl7.org/fhir/StructureDefinition/maxDecimalPlaces";
        public const string MinLenghtUri = "http://hl7.org/fhir/StructureDefinition/minLength";
        public const string RegexUri = "http://hl7.org/fhir/StructureDefinition/regex";
        public const string MaxValueUri = "http://hl7.org/fhir/StructureDefinition/maxValue";
        public const string MinValueUri = "http://hl7.org/fhir/StructureDefinition/minValue";
        public const string ValidationTextUri = "http://ehelse.no/fhir/StructureDefinition/validationtext";
        public const string RepeatsTextUri = "http://ehelse.no/fhir/StructureDefinition/repeatstext";

        //private static string FhirBaseUrl = "http://nde-fhir-ehelse.azurewebsites.net/fhir";
        //private static string FhirBaseUrl = "http://localhost:49911/fhir";

        private static string FileNameReservedCharacters = "<>:\"/\\|?*";

        // Example
        // fhir-tool.exe --version 1 --questionnaire HELFO_E106_NB.txt --valueset HELFO_E106_NB_Kodeverk.txt --fhir-base-url http://nde-fhir-ehelse.azurewebsites.net/fhir
        static void Main(string[] args)
        {
            FhirToolArguments arguments = FhirToolArguments.Create(args);
            try
            {
                IList<ValueSet> valueSets = GetValueSetsFromFlatFileFormat(arguments.ValueSetPath, false);
                Questionnaire questionnaire = GetQuestionnairesFromFlatFileFormatV1(arguments.QuestionnairePath).FirstOrDefault();
                
                if (questionnaire == null)
                {
                    Console.WriteLine("Failed to extract Questionnaire from flat file format.");
                    goto exit;
                }
                
                foreach (ValueSet valueSet in valueSets)
                {
                    questionnaire.Contained.Add(valueSet);
                }

                questionnaire.SerializeResourceToDiskAsJson(GenerateLegalFilename($"Questionnaire-{questionnaire.Name}.json"));
                questionnaire.SerializeResourceToDiskAsXml(GenerateLegalFilename($"Questionnaire-{questionnaire.Name}.xml"));

                FhirClient fhirClient = new FhirClient(arguments.FhirBaseUrl);
                fhirClient.Update(questionnaire);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nERROR: {ex.Message}\n");
            }

            exit:
            Console.WriteLine("Press any key to exit. . .");
            Console.ReadKey(true);
        }

        static string GenerateLegalFilename(string path)
        {
            string legalFilename = path;
            foreach(char c in FileNameReservedCharacters)
            {
                legalFilename = legalFilename.Replace(c, '_');
            }

            return legalFilename;
        }

        static void ImportQuestionnaire(string fhirBaseUrl, string path)
        {
            IList<Questionnaire> questionnaires = GetQuestionnairesFromFlatFileFormatV1(path);

            Bundle bundleOfQuestionnaires = new Bundle();

            foreach(Questionnaire questionnaire in questionnaires)
            {
                questionnaire.SerializeResourceToDiskAsXml(GenerateLegalFilename($"Questionnaire-{questionnaire.Name}.xml"));

                bundleOfQuestionnaires.Entry.Add(new Bundle.EntryComponent
                {
                    Request = new Bundle.RequestComponent
                    {
                        Url = string.IsNullOrEmpty(questionnaire.Id)
                                    ? string.Empty
                                    : $"{fhirBaseUrl}Questionnaire/{questionnaire.Id}",
                        Method = string.IsNullOrEmpty(questionnaire.Id)
                                    ? Bundle.HTTPVerb.POST
                                    : Bundle.HTTPVerb.PUT
                    },
                    Resource = questionnaire
                });
            }

            FhirClient fhirClient = new FhirClient(fhirBaseUrl);
            fhirClient.Transaction(bundleOfQuestionnaires);
        }

        static void ImportValueSet(string fhirBaseUrl, string path)
        {
            IList<ValueSet> valueSets = GetValueSetsFromFlatFileFormat(path);

            Bundle bundleOfValueSets = new Bundle();

            foreach (ValueSet valueSet in valueSets)
            {
                valueSet.SerializeResourceToDiskAsXml(GenerateLegalFilename($"ValueSet-{valueSet.Name}.xml"));

                bundleOfValueSets.Entry.Add(new Bundle.EntryComponent
                {
                    Request = new Bundle.RequestComponent
                    {
                        Url = string.IsNullOrEmpty(valueSet.Id) 
                                    ? string.Empty
                                    : $"{fhirBaseUrl}ValueSet/{valueSet.Id}",
                        Method = string.IsNullOrEmpty(valueSet.Id)
                                    ? Bundle.HTTPVerb.POST
                                    : Bundle.HTTPVerb.PUT
                    },
                    Resource = valueSet
                });
            }

            FhirClient fhirClient = new FhirClient(fhirBaseUrl);
            fhirClient.Transaction(bundleOfValueSets);
        }

        private static IList<Questionnaire> GetQuestionnairesFromFlatFileFormatV1(string path)
        {
            IList<Questionnaire> questionnaires = new List<Questionnaire>();

            var engine = new MasterDetailEngine<QuestionnaireHeader, QuestionnaireItem>(new MasterDetailSelector(RecordSelector))
            {
                Encoding = new UTF8Encoding()
            };
            MasterDetails<QuestionnaireHeader, QuestionnaireItem>[] masterDetails = engine.ReadFile(path);
            foreach (MasterDetails<QuestionnaireHeader, QuestionnaireItem> masterDetail in masterDetails)
            {
                Console.WriteLine($"Questionnaire: {masterDetail.Master.Name} - {masterDetail.Master.Title}");

                Questionnaire questionnaire = new Questionnaire
                {
                    Id = masterDetail.Master.Id,
                    Url = masterDetail.Master.Url,

                    //
                    // Helse Vest
                    //
                    // MRSA
                    //Id = "120",
                    //Url = "urn:uuid:659c3c2a-8715-4cf1-8abd-afc1d8972b72",
                    //Language = "nb-NO",

                    // TODO: Name er søkbart, vi må definere dette som en unik del enten kun ved seg selv eller i kombinasjon med eksempelvis en Tag
                    Name = masterDetail.Master.Name,
                    Title = masterDetail.Master.Title,
                    Status = EnumUtility.ParseLiteral<PublicationStatus>(masterDetail.Master.Status),
                    Date = masterDetail.Master.Date,
                    Publisher = masterDetail.Master.Publisher,
                    Description = new Markdown(masterDetail.Master.Description),
                    Purpose = string.IsNullOrEmpty(masterDetail.Master.Purpose) ? null : new Markdown(masterDetail.Master.Purpose),
                    
                    //UseContext = masterDetail.Master.UseContext,
                    Contact = new List<ContactDetail> { new ContactDetail { Telecom = new List<ContactPoint> { new ContactPoint { System = ContactPoint.ContactPointSystem.Url, Value = masterDetail.Master.Contact } } } },
                    //SubjectType = masterDetail.Master.SubjectType
                };

                questionnaire.Meta = new Meta
                {
                    Profile = new string[] { "http://ehelse.no/fhir/StructureDefinition/sdf-questionnaire" },
                    Tag = new List<Coding>
                    {
                        // TODO: Vi trenger definere en Tag som anigr det tekniske navnet til
                        //new Coding("http://fhi.no/fylkeshelseundersokelse", "1.0"),
                        //new Coding("http://helfo.no/questionnaire", "E121"),
                        
                        // TODO: Vi trenger definere tilgangsstyring i Excel-arket
                        new Coding("urn:2.16.578.1.12.4.1.1.7607", "3")
                    }
                };

                if (!string.IsNullOrEmpty(masterDetail.Master.Language))
                {
                    questionnaire.Language = masterDetail.Master.Language;
                    // TODO: Vi trenger definere Visningsnavn for språket, eksempelvis: Norsk (bokmål), osv.
                    questionnaire.Meta.Tag.Add(new Coding("urn:ietf:bcp:47", questionnaire.Language));
                }

                questionnaire.SetExtension("http://ehelse.no/fhir/StructureDefinition/sdf-endpoint", new ResourceReference("http://nde-fhir-ehelse.azurewebsites.net/fhir/Endpoint/1"));

                IList<string> linkIds = new List<string>();
                Questionnaire.ItemComponent item = null;
                for(int i = 0; i < masterDetail.Details.Length; i++)
                {
                    QuestionnaireItem questionnaireItem = masterDetail.Details[i];

                    if (linkIds.IndexOf(questionnaireItem.LinkId) > 0) throw new DuplicateLinkIdException(questionnaireItem.LinkId);

                    Console.WriteLine($"Questionnaire Item: {questionnaireItem.LinkId} - {questionnaireItem.Type} - {questionnaireItem.Text}");

                    int level = questionnaireItem.LinkId.Split('.').Length - 1;
                    if(level > 0)
                    {
                        i = Dive(i, level, item.Item, masterDetail.Details);
                    }
                    else
                    {
                        item = CreateItemComponentV1(questionnaireItem);
                        questionnaire.Item.Add(item);
                    }
                }

                questionnaires.Add(questionnaire);
            }

            return questionnaires;
        }

        private static int Dive(int index, int level, List<Questionnaire.ItemComponent> itemComponents, QuestionnaireItem[] questionnaireItems)
        {
            int currentIndex = index;

            Questionnaire.ItemComponent item = null;
            for (; currentIndex < questionnaireItems.Length; currentIndex++)
            {
                QuestionnaireItem questionnaireItem = questionnaireItems[currentIndex];
                Console.WriteLine($"Questionnaire Item: {questionnaireItem.LinkId} - {questionnaireItem.Type} - {questionnaireItem.Text}");

                int currentLevel = questionnaireItem.LinkId.Split('.').Length - 1;
                if(currentLevel == level)
                {
                    item = CreateItemComponentV1(questionnaireItem);
                    itemComponents.Add(item);
                }
                else if(currentLevel > level)
                {
                    if (item == null) throw new Exception("LinkId cannot bypass a level, i.e. jumping from 1.1 to 1.1.1.1");
                    currentIndex = Dive(currentIndex, currentLevel, item.Item, questionnaireItems);       

                }
                else if (currentLevel < level)
                {
                    // If current level is less than the entry level then break out of loop and return from recursion level.
                    break;
                }
            }
            return currentIndex - 1;
        }

        private static DataTypeEnum GetDataType(string str)
        {

            bool boolValue;
            Int32 intValue;
            Int64 bigintValue;
            double doubleValue;
            DateTime dateValue;

            // Place checks higher in if-else statement to give higher priority to type.

            if (bool.TryParse(str, out boolValue))
                return DataTypeEnum.Boolean;
            else if (Int32.TryParse(str, out intValue))
                return DataTypeEnum.Int32;
            else if (Int64.TryParse(str, out bigintValue))
                return DataTypeEnum.Int64;
            else if (double.TryParse(str, out doubleValue))
                return DataTypeEnum.Double;
            else if (DateTime.TryParse(str, out dateValue))
                return DataTypeEnum.DateTime;
            else return DataTypeEnum.String;

        }

        private static Element GetElement(Questionnaire.QuestionnaireItemType itemType, string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            switch(itemType)
            {
                case Questionnaire.QuestionnaireItemType.Boolean:
                    return new FhirBoolean(bool.Parse(value));
                case Questionnaire.QuestionnaireItemType.Integer:
                    return new Integer(int.Parse(value));
                case Questionnaire.QuestionnaireItemType.Decimal:
                    return new FhirDecimal(decimal.Parse(value));
                case Questionnaire.QuestionnaireItemType.DateTime:
                    return new FhirDateTime(DateTime.Parse(value));
                case Questionnaire.QuestionnaireItemType.String:
                    return new FhirString(value);
                default:
                    return null;
            }
        }

        private static Questionnaire.ItemComponent CreateItemComponentV1(QuestionnaireItem item)
        {
            Questionnaire.QuestionnaireItemType? itemType = EnumUtility.ParseLiteral<Questionnaire.QuestionnaireItemType>(item.Type);
            if (!itemType.HasValue) throw new Exception(string.Format("QuestionnaireItemType at question with linkId: {} is not conforming to any valid literals. QuestionnaireItemType: {1}", item.LinkId, item.Type));

            Questionnaire.ItemComponent itemComponent = new Questionnaire.ItemComponent
            {
                Type = itemType,
                LinkId = item.LinkId,
                Prefix = string.IsNullOrEmpty(item.Prefix) ? null : item.Prefix,
                Text = string.IsNullOrEmpty(item.Text) ? null : item.Text,
                Required = item.Required.HasValue ? item.Required : null,
                Repeats = item.Repeats,
                ReadOnly = item.ReadOnly,
                Initial = GetElement(itemType.Value, item.Initial)
            };
            if (!string.IsNullOrEmpty(item.ValidationText))
                itemComponent.SetStringExtension(ValidationTextUri, item.ValidationText);
            if (!string.IsNullOrEmpty(item.ReferenceValue) && item.ReferenceValue.IndexOf('#') == 0)
                itemComponent.Options = new ResourceReference($"#{item.ReferenceValue.Substring(1)}");
            if (!string.IsNullOrEmpty(item.EnableWhen))
                itemComponent.EnableWhen = ParseEnableWhen(item.EnableWhen).ToList();
            if (!string.IsNullOrEmpty(item.EntryFormat))
                itemComponent.SetStringExtension(EntryFormatUri, item.EntryFormat);
            if (item.MaxValue.HasValue)
                itemComponent.SetIntegerExtension(MaxValueUri, item.MaxValue.Value);
            if (item.MinValue.HasValue)
                itemComponent.SetIntegerExtension(MinValueUri, item.MinValue.Value);

            return itemComponent;
        }

        private static IEnumerable<Questionnaire.EnableWhenComponent> ParseEnableWhen(string value)
        {
            JObject enableWhenObject = JObject.Parse(value);
            JArray enableWhenArray = enableWhenObject["EnableWhen"] as JArray;
            IList<EnableWhen> enableWhenList = JsonConvert.DeserializeObject<List<EnableWhen>>(enableWhenArray.ToString());
            foreach (EnableWhen enableWhen in enableWhenList)
            {
                Questionnaire.EnableWhenComponent enableWhenComponent = new Questionnaire.EnableWhenComponent
                {
                    Question = enableWhen.Question,
                };
                if (enableWhen.HasAnswer.HasValue)
                    enableWhenComponent.HasAnswer = enableWhen.HasAnswer;
                if (enableWhen.AnswerBoolean.HasValue)
                    enableWhenComponent.Answer = new FhirBoolean(enableWhen.AnswerBoolean);
                if (enableWhen.AnswerDecimal.HasValue)
                    enableWhenComponent.Answer = new FhirDecimal(enableWhen.AnswerDecimal);
                if (enableWhen.AnswerInteger.HasValue)
                    enableWhenComponent.Answer = new Integer(enableWhen.AnswerInteger);
                if (!string.IsNullOrEmpty(enableWhen.AnswerDate))
                    enableWhenComponent.Answer = new Date(enableWhen.AnswerDate);
                if (!string.IsNullOrEmpty(enableWhen.AnswerDateTime))
                    enableWhenComponent.Answer = new FhirDateTime(enableWhen.AnswerDateTime);
                if (!string.IsNullOrEmpty(enableWhen.AnswerTime))
                    enableWhenComponent.Answer = new Time(enableWhen.AnswerTime);
                if (!string.IsNullOrEmpty(enableWhen.AnswerString))
                    enableWhenComponent.Answer = new FhirString(enableWhen.AnswerString);
                if (enableWhen.AnswerCoding != null)
                    enableWhenComponent.Answer = new Coding(enableWhen.AnswerCoding.System, enableWhen.AnswerCoding.Code);
                if (enableWhen.AnswerQuantity != null)
                {
                    Quantity quantity = new Quantity();
                    if (enableWhen.AnswerQuantity.Value.HasValue)
                        quantity.Value = enableWhen.AnswerQuantity.Value.Value;
                    if (!string.IsNullOrEmpty(enableWhen.AnswerQuantity.System))
                        quantity.System = enableWhen.AnswerQuantity.System;
                    if (!string.IsNullOrEmpty(enableWhen.AnswerQuantity.Code))
                        quantity.Code = enableWhen.AnswerQuantity.Code;
                    if (!string.IsNullOrEmpty(enableWhen.AnswerQuantity.Unit))
                        quantity.Unit = enableWhen.AnswerQuantity.Unit;
                    enableWhenComponent.Answer = quantity;
                }

                yield return enableWhenComponent;
            }
        }

        private static IList<ValueSet> GetValueSetsFromFlatFileFormat(string filename, bool genereateNarrative = true)
        {
            IList<ValueSet> valueSets = new List<ValueSet>();

            var engine = new MasterDetailEngine<ValueSetHeader, ValueSetCodeReferences>(new MasterDetailSelector(RecordSelector))
            {
                Encoding = new UTF8Encoding()
            };
            MasterDetails<ValueSetHeader, ValueSetCodeReferences>[] masterDetails = engine.ReadFile(filename);
            foreach (MasterDetails<ValueSetHeader, ValueSetCodeReferences> masterDetail in masterDetails)
            {
                Console.WriteLine($"ValueSet: {masterDetail.Master.Id} - {masterDetail.Master.Title}");

                ValueSet valueSet = new ValueSet
                {
                    Id = masterDetail.Master.Id,
                    Name = masterDetail.Master.Name,
                    Title = masterDetail.Master.Title,
                    Status = PublicationStatus.Draft,
                    Version = masterDetail.Master.Version,
                    Publisher = masterDetail.Master.Publisher,
                };

                ValueSet.ConceptSetComponent conceptSet = new ValueSet.ConceptSetComponent
                {
                    System = masterDetail.Master.System
                };
                valueSet.Compose = new ValueSet.ComposeComponent
                {
                    Include = new List<ValueSet.ConceptSetComponent>
                    {
                        conceptSet
                    }
                };
                foreach (ValueSetCodeReferences valueSetCodeReference in masterDetail.Details)
                {
                    Console.WriteLine($"ValueSetCodeReference: {valueSetCodeReference.Code} - {valueSetCodeReference.Display}");

                    conceptSet.Concept.Add(new ValueSet.ConceptReferenceComponent { Code = valueSetCodeReference.Code, Display = valueSetCodeReference.Display });
                }

                if (genereateNarrative)
                {
                    valueSet.GenerateAndSetNarrative();
                }

                valueSets.Add(valueSet);
            }
            
            return valueSets;
        }

        private static RecordAction RecordSelector(string record)
        {
            switch (record[0])
            {
                case 'M':
                    return RecordAction.Master;
                case 'D':
                    return RecordAction.Detail;
                default:
                    return RecordAction.Skip;
            }
        }
    }
}