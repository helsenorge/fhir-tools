using FhirTool.Extensions;
using FhirTool.Model;
using FhirTool.Model.FlatFile;
using FileHelpers.MasterDetail;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
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
        public const string ItemControlUri = "http://hl7.org/fhir/StructureDefinition/questionnaire-itemControl";
        public const string MaxOccursUri = "http://hl7.org/fhir/StructureDefinition/questionnaire-maxOccurs";
        public const string MinOccursUri = "http://hl7.org/fhir/StructureDefinition/questionnaire-minOccurs";
        
        public const string ItemControlSystem = "http://hl7.org/fhir/ValueSet/questionnaire-item-control";

        //private static string FhirBaseUrl = "http://nde-fhir-ehelse.azurewebsites.net/fhir";
        //private static string FhirBaseUrl = "http://localhost:49911/fhir";

        private static string FileNameReservedCharacters = "<>:\"/\\|?*";
        private static FhirToolArguments _arguments = null;
        private static TextWriter _out = null;

        // First example
        // fhir-tool.exe --version 1 --questionnaire HELFO_E106_NB.txt --valueset HELFO_E106_NB_Kodeverk.txt --fhir-base-url http://nde-fhir-ehelse.azurewebsites.net/fhir --resolve-url

        // Splitting up operations
        // fhir-tool.exe generate --version 1 --format json --questionnaire HELFO_E121_NB.txt --valueset HELFO_E121_NB_Kodeverk.txt
        // fhir-tool.exe upload --format json --questionnaire Questionnaire-Helfo_E121_NB-no.json --fhir-base-url http://nde-fhir-ehelse.azurewebsites.net/fhir --resolve-url

        // Unsure if we should handle kith and messaging in this tool
        // fhir-tool.exe generate-kith --questionnaire Questionnaire-Helfo_E121_NB-no.xml --fhir-base-url http://nde-fhir-ehelse.azurewebsites.net/fhir --resolve-url
        // fhir-tool.exe sendasync --questionnaire Questionnaire-Helfo_E121_NB-no.xml --fhir-base-url http://nde-fhir-ehelse.azurewebsites.net/fhir --resolve-url
        static void Main(string[] args)
        {
            try
            {
                _out = Console.Out;
                _arguments = FhirToolArguments.Create(args);

                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
                WriteLineToOutput($"{versionInfo.ProductName} [Version {Assembly.GetEntryAssembly().GetName().Version}]");
                WriteLineToOutput($"(c) 2018 {versionInfo.CompanyName}. All rights reserved.");
                WriteLineToOutput();

                DebugWriteLineToOutput("Validating command line arguments.");
                if (!ValidateArguments(_arguments))
                {
                    ErrorWriteLineToOutput("One or more arguments did not validate. Please verify your arguments according to output.");
                    goto exit;
                }

                switch(_arguments.Operation)
                {
                    case OperationEnum.Generate:
                        GenerateFromFlatFileOperation(_arguments);
                        break;
                    case OperationEnum.Upload:
                        UploadToFhirServerOperation(_arguments);
                        break;
                    default:
                        throw new NotSupportedOperationException(_arguments.Operation);
                }
            }
            catch (Exception ex)
            {
                ErrorWriteLineToOutput($"{ex.Message}");
            }

            exit:
            WriteLineToOutput("\nPress any key to exit. . .");
            Console.ReadKey(true);
        }

        private static void GenerateFromFlatFileOperation(FhirToolArguments arguments)
        {
            if (string.IsNullOrEmpty(arguments.QuestionnairePath))
            {
                ErrorWriteLineToOutput($"Operation '{FhirToolArguments.GENERATE_OP}' requires argument '{FhirToolArguments.QUESTIONNAIRE_ARG}'.");
                return;
            }

            WriteLineToOutput($"Expecting flat file format to conform to version: {arguments.Version}");
            IList<ValueSet> valueSets = null;
            if (!string.IsNullOrEmpty(arguments.ValueSetPath))
            {
                WriteLineToOutput($"Loading ValueSet(s) from file: '{arguments.ValueSetPath}'.");
                valueSets = GetValueSetsFromFlatFileFormat(arguments.ValueSetPath, false);
            }
            
            Questionnaire questionnaire = null;
            WriteLineToOutput($"Loading Questionnaire from file: {arguments.QuestionnairePath}");
            if (arguments.Version == "1")
            {
                questionnaire = GetQuestionnairesFromFlatFileFormatV1(arguments.QuestionnairePath).FirstOrDefault();
            }
            else if (arguments.Version == "2")
            {
                questionnaire = GetQuestionnairesFromFlatFileFormatV2(arguments.QuestionnairePath).FirstOrDefault();
            }
            else
            {
                string version = string.IsNullOrEmpty(arguments.Version) ? "missing" : arguments.Version;
                ErrorWriteLineToOutput($"Operation {FhirToolArguments.GENERATE_OP} requires argument '{FhirToolArguments.VERSION_ARG}'. Argument is either missing or an unknown value was set.\nValue: '{version}'");
                return;
            }

            if (questionnaire == null)
            {
                ErrorWriteLineToOutput($"Failed to extract Questionnaire from flat file format\nLocation: '{arguments.QuestionnairePath}'.");
                return;
            }
            
            if (valueSets.Count > 0)
            {
                WriteLineToOutput("Adding ValueSet(s) to contained section of Questionnaire.");
                foreach (ValueSet valueSet in valueSets)
                {
                    questionnaire.Contained.Add(valueSet);
                }
            }

            if (arguments.MimeType == "xml")
            {
                string path = $"Questionnaire-{questionnaire.Name}.xml";
                WriteLineToOutput($"Writing Questionnaire in xml format to local disk: {path}");
                questionnaire.SerializeResourceToDiskAsXml(GenerateLegalFilename(path));
            }
            if (arguments.MimeType == "json")
            {
                string path = $"Questionnaire-{questionnaire.Name}.json";
                WriteLineToOutput($"Writing Questionnaire in json format to local disk: {path}");
                questionnaire.SerializeResourceToDiskAsJson(GenerateLegalFilename(path));
            }
        }

        private static void UploadToFhirServerOperation(FhirToolArguments arguments)
        {
            if (string.IsNullOrEmpty(arguments.FhirBaseUrl))
            {
                ErrorWriteLineToOutput($"Operation '{FhirToolArguments.UPLOAD_OP}' requires argument '{FhirToolArguments.FHIRBASEURL_ARG}'.");
                return;
            }
            if (string.IsNullOrEmpty(arguments.QuestionnairePath))
            {
                ErrorWriteLineToOutput($"Operation '{FhirToolArguments.UPLOAD_OP}' requires argument '{FhirToolArguments.QUESTIONNAIRE_ARG}'.");
                return;
            }

            WriteLineToOutput($"Deserializing Fhir resource at '{arguments.QuestionnairePath}'.");
            WriteLineToOutput($"Expecting format: '{arguments.MimeType}'.");
            Questionnaire questionnaire = null;
            switch(arguments.MimeType)
            {
                case "xml":
                    questionnaire = DeserializeXmlResource<Questionnaire>(arguments.QuestionnairePath);
                    break;
                case "json":
                    questionnaire = DeserializeJsonResource<Questionnaire>(arguments.QuestionnairePath);
                    break;
                default:
                    break;
            }
            if (questionnaire == null)
            {
                ErrorWriteLineToOutput($"Failed to extract Questionnaire from flat file format\nLocation: '{arguments.QuestionnairePath}'.");
                return;
            }

            WriteLineToOutput($"Uploading Questionnaire to endpoint: {arguments.FhirBaseUrl}");
            // Set a relative url before posting to the server
            if(!string.IsNullOrEmpty(questionnaire.Id))
            {
                questionnaire.Url = $"{ResourceType.Questionnaire.GetLiteral()}/{questionnaire.Id}";
            }
            else
            {
                questionnaire.Url = string.Empty;
            }
            
            // Initialize a FhirClient and POST or PUT Questionnaire to server.
            FhirClient fhirClient = new FhirClient(arguments.FhirBaseUrl);
            if (string.IsNullOrEmpty(questionnaire.Id))
                questionnaire = fhirClient.Create(questionnaire);
            else
                questionnaire = fhirClient.Update(questionnaire);

            WriteLineToOutput($"Successfully uploaded Questionnaire to endpoint.");
            WriteLineToOutput($"Questionnaire was assigned the Id: {questionnaire.Id}");
            WriteLineToOutput($"Questionnaire can be accessed at: {fhirClient.Endpoint.AbsoluteUri}{ResourceType.Questionnaire.GetLiteral()}/{questionnaire.Id}");
        }

        public static T DeserializeJsonResource<T>(string path)
            where T : Resource
        {
            T resource = null;

            using (StreamReader reader = new StreamReader(path))
            {
                IElementNavigator navigator = JsonDomFhirNavigator.Create(reader.ReadToEnd());
                BaseFhirParser parser = new FhirJsonParser();
                resource = parser.Parse<T>(navigator);
            }
            return resource;
        }

        public static T DeserializeXmlResource<T>(string path)
            where T : Resource
        {
            T resource = null;
            using (StreamReader reader = new StreamReader(path))
            {
                IElementNavigator navigator = XmlDomFhirNavigator.Create(reader.ReadToEnd());
                BaseFhirParser parser = new FhirXmlParser();
                resource = parser.Parse<T>(navigator);
            }
            return resource;
        }

        private static void DebugWriteLineToOutput(string value)
        {
            if (_arguments.Verbose)
                _out.WriteLine($"DEBUG: {value}");
        }

        private static void InfoWriteLineToOutput(string value)
        {
            _out.WriteLine($"INFO: {value}");
        }

        private static void ErrorWriteLineToOutput(string value)
        {
            _out.WriteLine($"ERROR: {value}");
        }

        private static void WriteLineToOutput(string value = "")
        {
            _out.WriteLine(value);
        }

        private static bool ValidateArguments(FhirToolArguments arguments)
        {
            bool validated = true;
            if(!string.IsNullOrEmpty(arguments.QuestionnairePath))
            {
                if (!File.Exists(arguments.QuestionnairePath))
                {
                    ErrorWriteLineToOutput($"File not found: '{arguments.QuestionnairePath}'");
                    validated = false;
                }
            }
            else
            {
                ErrorWriteLineToOutput($"Argument {FhirToolArguments.QUESTIONNAIRE_ARG} | {FhirToolArguments.QUESTIONNAIRE_SHORT_ARG} is required.");
                validated = false;
            }

            if (!string.IsNullOrEmpty(arguments.ValueSetPath))
            {
                if (!File.Exists(arguments.ValueSetPath))
                {
                    ErrorWriteLineToOutput($"File not found: '{arguments.ValueSetPath}'");
                    validated = false;
                }
            }
            if(arguments.Operation == OperationEnum.Generate && string.IsNullOrEmpty(arguments.Version))
            {
                ErrorWriteLineToOutput($"Argument {FhirToolArguments.VERSION_ARG} | {FhirToolArguments.VERSION_SHORT_ARG} is required.");
                validated = false;
            }
            if (!string.IsNullOrEmpty(arguments.FhirBaseUrl))
            {
                if (arguments.ResolveUrl && !ResolveUrl(new Uri(arguments.FhirBaseUrl)).IsSuccessStatusCode)
                {
                    ErrorWriteLineToOutput($"Could not resolve url: {arguments.FhirBaseUrl}");
                    validated = false;
                }
            }

            return validated;
        }

        private static HttpResponseMessage ResolveUrl(Uri uri)
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Head,
                RequestUri = new Uri($"{uri.Scheme}://{uri.Host}")
            };
            using (HttpClient client = new HttpClient())
                return client.SendAsync(request).GetAwaiter().GetResult();
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

        private static IList<Questionnaire> GetQuestionnairesFromFlatFileFormatV2(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path)) throw new FileNotFoundException($"File not found: '{path}'", path);

            IList<Questionnaire> questionnaires = new List<Questionnaire>();

            var engine = new MasterDetailEngine<QuestionnaireHeader2, QuestionnaireItem2>(new MasterDetailSelector(RecordSelector))
            {
                Encoding = new UTF8Encoding()
            };
            MasterDetails<QuestionnaireHeader2, QuestionnaireItem2>[] masterDetails = engine.ReadFile(path);
            foreach (MasterDetails<QuestionnaireHeader2, QuestionnaireItem2> masterDetail in masterDetails)
            {
                DebugWriteLineToOutput($"Questionnaire: {masterDetail.Master.Name} - {masterDetail.Master.Title}");

                Questionnaire questionnaire = new Questionnaire
                {
                    Id = masterDetail.Master.Id,
                    Url = masterDetail.Master.Url,
                    Version = masterDetail.Master.Version,

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
                    ApprovalDate = masterDetail.Master.ApprovalDate,
                    LastReviewDate = masterDetail.Master.LastReviewDate,
                    //EffectivePeriod = masterDetail.Master.EffectivePeriod
                    Contact = new List<ContactDetail> { new ContactDetail { Name = masterDetail.Master.ContactName } },
                    Copyright = new Markdown(masterDetail.Master.Copyright)
                };

                if (!string.IsNullOrEmpty(masterDetail.Master.SubjectType))
                {
                    IList<ResourceType?> resourceTypes = new List<ResourceType?>();
                    string[] subjectTypes = masterDetail.Master.SubjectType.Split('|');
                    foreach (string subjectType in subjectTypes)
                    {
                        ResourceType? resourceType = EnumUtility.ParseLiteral<ResourceType>(subjectType);
                        if (resourceType.HasValue)
                            resourceTypes.Add(resourceType);
                    }
                    questionnaire.SubjectType = resourceTypes;
                }
                
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
                for (int i = 0; i < masterDetail.Details.Length; i++)
                {
                    QuestionnaireItem2 questionnaireItem = masterDetail.Details[i];

                    if (linkIds.IndexOf(questionnaireItem.LinkId) > 0) throw new DuplicateLinkIdException(questionnaireItem.LinkId);

                    DebugWriteLineToOutput($"Questionnaire Item: {questionnaireItem.LinkId} - {questionnaireItem.Type} - {questionnaireItem.Text}");

                    int level = questionnaireItem.LinkId.Split('.').Length - 1;
                    if (level > 0)
                    {
                        i = DiveV2(i, level, item.Item, masterDetail.Details);
                    }
                    else
                    {
                        item = CreateItemComponentV2(questionnaireItem);
                        questionnaire.Item.Add(item);
                    }
                }

                questionnaires.Add(questionnaire);
            }

            return questionnaires;
        }

        private static int DiveV2(int index, int level, List<Questionnaire.ItemComponent> itemComponents, QuestionnaireItem2[] questionnaireItems)
        {
            int currentIndex = index;

            Questionnaire.ItemComponent item = null;
            for (; currentIndex < questionnaireItems.Length; currentIndex++)
            {
                QuestionnaireItem2 questionnaireItem = questionnaireItems[currentIndex];
                DebugWriteLineToOutput($"Questionnaire Item: {questionnaireItem.LinkId} - {questionnaireItem.Type} - {questionnaireItem.Text}");

                int currentLevel = questionnaireItem.LinkId.Split('.').Length - 1;
                if (currentLevel == level)
                {
                    item = CreateItemComponentV2(questionnaireItem);
                    itemComponents.Add(item);
                }
                else if (currentLevel > level)
                {
                    if (item == null) throw new Exception("LinkId cannot bypass a level, i.e. jumping from 1.1 to 1.1.1.1");
                    currentIndex = DiveV2(currentIndex, currentLevel, item.Item, questionnaireItems);

                }
                else if (currentLevel < level)
                {
                    // If current level is less than the entry level then break out of loop and return from recursion level.
                    break;
                }
            }
            return currentIndex - 1;
        }

        private static Questionnaire.ItemComponent CreateItemComponentV2(QuestionnaireItem2 item)
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
                Initial = GetElement(itemType.Value, item.Initial),
                MaxLength = item.MaxLength,
            };
            if (!string.IsNullOrEmpty(item.ValidationText))
                itemComponent.SetStringExtension(ValidationTextUri, item.ValidationText);

            if (!string.IsNullOrEmpty(item.Options) && item.Options.IndexOf('#') == 0)
                itemComponent.Options = new ResourceReference($"#{item.Options.Substring(1)}");

            if (!string.IsNullOrEmpty(item.EnableWhen))
                itemComponent.EnableWhen = ParseEnableWhen(item.EnableWhen).ToList();

            if (!string.IsNullOrEmpty(item.EntryFormat))
                itemComponent.SetStringExtension(EntryFormatUri, item.EntryFormat);

            if (item.MaxValueInteger.HasValue)
                itemComponent.SetIntegerExtension(MaxValueUri, item.MaxValueInteger.Value);
            if (item.MinValueInteger.HasValue)
                itemComponent.SetIntegerExtension(MinValueUri, item.MinValueInteger.Value);

            if (item.MaxValueDate.HasValue)
                itemComponent.SetExtension(MaxValueUri, new FhirDateTime(item.MaxValueDate.Value));
            if (item.MinValueDate.HasValue)
                itemComponent.SetExtension(MinValueUri, new FhirDateTime(item.MinValueDate.Value));

            if (item.MaxValueDate.HasValue)
                itemComponent.SetExtension(MaxValueUri, new FhirDateTime(item.MaxValueDate.Value));
            if (item.MinValueDate.HasValue)
                itemComponent.SetExtension(MinValueUri, new FhirDateTime(item.MinValueDate.Value));

            if (item.MinLength.HasValue)
                itemComponent.SetIntegerExtension(MinLenghtUri, item.MinLength.Value);

            if (item.MaxDecimalPlaces.HasValue)
                itemComponent.SetIntegerExtension(MaxDecimalPlacesUri, item.MaxDecimalPlaces.Value);

            if (!string.IsNullOrEmpty(item.RepeatsText))
                itemComponent.SetStringExtension(RepeatsTextUri, item.RepeatsText);

            if (!string.IsNullOrEmpty(item.ItemControl))
            {
                Coding coding = new Coding
                {
                    System = ItemControlSystem,
                    Code = item.ItemControl
                };
                itemComponent.SetExtension(ItemControlUri, coding);
            }

            if (item.MaxOccurs.HasValue)
                itemComponent.SetIntegerExtension(MaxOccursUri, item.MaxOccurs.Value);
            if (item.MinOccurs.HasValue)
                itemComponent.SetIntegerExtension(MinOccursUri, item.MinOccurs.Value);

            if (!string.IsNullOrEmpty(item.Regex))
                itemComponent.SetStringExtension(RegexUri, item.Regex);
            
            return itemComponent;
        }

        private static IList<Questionnaire> GetQuestionnairesFromFlatFileFormatV1(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path)) throw new FileNotFoundException($"File not found: '{path}'", path);

            IList<Questionnaire> questionnaires = new List<Questionnaire>();

            var engine = new MasterDetailEngine<QuestionnaireHeader, QuestionnaireItem>(new MasterDetailSelector(RecordSelector))
            {
                Encoding = new UTF8Encoding()
            };
            MasterDetails<QuestionnaireHeader, QuestionnaireItem>[] masterDetails = engine.ReadFile(path);
            foreach (MasterDetails<QuestionnaireHeader, QuestionnaireItem> masterDetail in masterDetails)
            {
                DebugWriteLineToOutput($"Questionnaire: {masterDetail.Master.Name} - {masterDetail.Master.Title}");

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

                    DebugWriteLineToOutput($"Questionnaire Item: {questionnaireItem.LinkId} - {questionnaireItem.Type} - {questionnaireItem.Text}");

                    int level = questionnaireItem.LinkId.Split('.').Length - 1;
                    if(level > 0)
                    {
                        i = DiveV1(i, level, item.Item, masterDetail.Details);
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

        private static int DiveV1(int index, int level, List<Questionnaire.ItemComponent> itemComponents, QuestionnaireItem[] questionnaireItems)
        {
            int currentIndex = index;

            Questionnaire.ItemComponent item = null;
            for (; currentIndex < questionnaireItems.Length; currentIndex++)
            {
                QuestionnaireItem questionnaireItem = questionnaireItems[currentIndex];
                DebugWriteLineToOutput($"Questionnaire Item: {questionnaireItem.LinkId} - {questionnaireItem.Type} - {questionnaireItem.Text}");

                int currentLevel = questionnaireItem.LinkId.Split('.').Length - 1;
                if(currentLevel == level)
                {
                    item = CreateItemComponentV1(questionnaireItem);
                    itemComponents.Add(item);
                }
                else if(currentLevel > level)
                {
                    if (item == null) throw new Exception("LinkId cannot bypass a level, i.e. jumping from 1.1 to 1.1.1.1");
                    currentIndex = DiveV1(currentIndex, currentLevel, item.Item, questionnaireItems);       

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

        private static IList<ValueSet> GetValueSetsFromFlatFileFormat(string path, bool genereateNarrative = true)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path)) throw new FileNotFoundException($"File not found: '{path}'.", path);

            IList<ValueSet> valueSets = new List<ValueSet>();
            var engine = new MasterDetailEngine<ValueSetHeader, ValueSetCodeReferences>(new MasterDetailSelector(RecordSelector))
            {
                Encoding = new UTF8Encoding()
            };
            MasterDetails<ValueSetHeader, ValueSetCodeReferences>[] masterDetails = engine.ReadFile(path);
            foreach (MasterDetails<ValueSetHeader, ValueSetCodeReferences> masterDetail in masterDetails)
            {
                DebugWriteLineToOutput($"ValueSet: {masterDetail.Master.Id} - {masterDetail.Master.Title}");

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
                    DebugWriteLineToOutput($"ValueSetCodeReference: {valueSetCodeReference.Code} - {valueSetCodeReference.Display}");

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
        
        static void ImportQuestionnaire(string fhirBaseUrl, string path)
        {
            IList<Questionnaire> questionnaires = GetQuestionnairesFromFlatFileFormatV1(path);

            Bundle bundleOfQuestionnaires = new Bundle();

            foreach (Questionnaire questionnaire in questionnaires)
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
    }
}