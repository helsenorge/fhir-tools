extern alias R4;

using FhirTool.Core.Model;
using FhirTool.Core.Model.FlatFile;
using FileHelpers.MasterDetail;
using R4::Hl7.Fhir.Model;
using R4::Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using FhirTool.Core.Operations;

namespace FhirTool.Core
{
    internal class QuestionnaireGenerator
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        public QuestionnaireGenerator(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<QuestionnaireGenerator>();
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

        public Questionnaire GenerateQuestionnaireFromFlatFile(GenerateQuestionnaireOperationOptions arguments)
        {
            Questionnaire questionnaire = null;
            _logger.LogInformation($"Loading Questionnaire from file: {arguments.Questionnaire}");
            if (arguments.ExcelSheetVersion == ExcelSheetVersion.v1)
            {
                questionnaire = GetQuestionnairesFromFlatFileFormatV1(arguments.Questionnaire.Path).FirstOrDefault();
            }
            else if (arguments.ExcelSheetVersion == ExcelSheetVersion.v2)
            {
                questionnaire = GetQuestionnairesFromFlatFileFormatV2(arguments.Questionnaire.Path).FirstOrDefault();
            }

            IList<ValueSet> valueSets = null;
            if (!string.IsNullOrWhiteSpace(arguments.ValueSet.Path))
            {
                _logger.LogInformation($"Loading ValueSet(s) from file: '{arguments.ValueSet}'.");
                valueSets = GetValueSetsFromFlatFileFormat(arguments.ValueSet.Path, false);
            }
            if (valueSets != null && valueSets.Count > 0)
            {
                _logger.LogDebug("Adding ValueSet(s) to contained section of Questionnaire.");
                foreach (ValueSet valueSet in valueSets)
                {
                    questionnaire.Contained.Add(valueSet);
                }
            }

            return questionnaire;
        }

        private IList<Questionnaire> GetQuestionnairesFromFlatFileFormatV2(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path)) throw new FileNotFoundException($"File not found: '{path}'", path);

            IList<Questionnaire> questionnaires = new List<Questionnaire>();

            var engine = new MasterDetailEngine<QuestionnaireHeader2, QuestionnaireItem2>(new MasterDetailSelector(RecordSelector))
            {
                Encoding = new UTF8Encoding()
            };
            MasterDetails<QuestionnaireHeader2, QuestionnaireItem2>[] masterDetails = engine.ReadFile(path);
            foreach (MasterDetails<QuestionnaireHeader2, QuestionnaireItem2> masterDetail in masterDetails)
            {
                _logger.LogDebug($"Questionnaire: {masterDetail.Master.Name} - {masterDetail.Master.Title}");

                Questionnaire questionnaire = new Questionnaire
                {
                    Meta = new Meta
                    {
                        Profile = new string[] { Constants.QuestionnaireProfileUri }
                    },

                    Id = string.IsNullOrWhiteSpace(masterDetail.Master.Id) ? null : masterDetail.Master.Id,
                    Url = string.IsNullOrWhiteSpace(masterDetail.Master.Url) ? null : masterDetail.Master.Url,
                    Version = string.IsNullOrWhiteSpace(masterDetail.Master.Version) ? null : masterDetail.Master.Version,
                    Name = string.IsNullOrWhiteSpace(masterDetail.Master.Name) ? null : masterDetail.Master.Name,
                    Title = string.IsNullOrWhiteSpace(masterDetail.Master.Title) ? null : masterDetail.Master.Title,
                    Status = string.IsNullOrWhiteSpace(masterDetail.Master.Status) ? null : EnumUtility.ParseLiteral<PublicationStatus>(masterDetail.Master.Status)
                };
                if (!string.IsNullOrWhiteSpace(masterDetail.Master.Date))
                {
                    if (!DateUtility.IsValidFhirDateTime(masterDetail.Master.Date)) throw new Exception($"The date {masterDetail.Master.Date} is not conforming to the expected format: 'yyyy-MM-dd'");
                    questionnaire.DateElement = new FhirDateTime(masterDetail.Master.Date);
                }
                questionnaire.Publisher = string.IsNullOrWhiteSpace(masterDetail.Master.Publisher) ? null : masterDetail.Master.Publisher;
                questionnaire.Description = string.IsNullOrWhiteSpace(masterDetail.Master.Description) ? null : new Markdown(masterDetail.Master.Description);
                questionnaire.Purpose = string.IsNullOrWhiteSpace(masterDetail.Master.Purpose) ? null : new Markdown(masterDetail.Master.Purpose);
                if (!string.IsNullOrWhiteSpace(masterDetail.Master.ApprovalDate))
                {
                    if (!DateUtility.IsValidFhirDate(masterDetail.Master.ApprovalDate)) throw new Exception($"The date {masterDetail.Master.ApprovalDate} is not conforming to the expected format: 'yyyy-MM-dd'");
                    questionnaire.ApprovalDateElement = new Date(masterDetail.Master.ApprovalDate);
                }
                if (!string.IsNullOrWhiteSpace(masterDetail.Master.ApprovalDate))
                {
                    if (!DateUtility.IsValidFhirDate(masterDetail.Master.LastReviewDate)) throw new Exception($"The date {masterDetail.Master.LastReviewDate} is not conforming to the expected format: 'yyyy-MM-dd'");
                    questionnaire.LastReviewDateElement = new Date(masterDetail.Master.LastReviewDate);
                }
                questionnaire.Contact = string.IsNullOrWhiteSpace(masterDetail.Master.ContactName) ? null : new List<ContactDetail> { new ContactDetail { Name = masterDetail.Master.ContactName } };
                questionnaire.Copyright = string.IsNullOrWhiteSpace(masterDetail.Master.Copyright) ? null : new Markdown(masterDetail.Master.Copyright);

                if (!string.IsNullOrWhiteSpace(masterDetail.Master.SubjectType))
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

                if (!string.IsNullOrWhiteSpace(masterDetail.Master.Language))
                {
                    questionnaire.Language = masterDetail.Master.Language;
                    string displayName = CultureInfo.GetCultureInfo(LanguageCodeUtility.GetLanguageCode(questionnaire.Language))?.NativeName.UpperCaseFirstCharacter();
                    questionnaire.Meta.Tag.Add(new Coding("urn:ietf:bcp:47", questionnaire.Language, displayName == null ? string.Empty : displayName));
                }

                if (!string.IsNullOrWhiteSpace(masterDetail.Master.UseContext))
                {
                    questionnaire.UseContext = ParseUsageContext(masterDetail.Master.UseContext);
                }

                if (!string.IsNullOrWhiteSpace(masterDetail.Master.Endpoint))
                {
                    questionnaire.SetExtension(Constants.EndPointUri, new ResourceReference(masterDetail.Master.Endpoint));
                }

                if (!string.IsNullOrWhiteSpace(masterDetail.Master.AuthenticationRequirement))
                {
                    questionnaire.SetExtension(Constants.AuthenticationRequirementUri, new Coding(Constants.AuthenticationRequirementSystem, masterDetail.Master.AuthenticationRequirement));
                }

                if (!string.IsNullOrWhiteSpace(masterDetail.Master.AccessibilityToResponse))
                {
                    questionnaire.SetExtension(Constants.AccessibilityToResponseUri, new Coding(Constants.AccessibilityToResponseSystem, masterDetail.Master.AccessibilityToResponse));
                }

                if (!string.IsNullOrWhiteSpace(masterDetail.Master.CanBePerformedBy))
                {
                    questionnaire.SetExtension(Constants.CanBePerformedByUri, new Coding(Constants.CanBePerformedBySystem, masterDetail.Master.CanBePerformedBy));
                }

                if (!string.IsNullOrWhiteSpace(masterDetail.Master.Discretion))
                {
                    questionnaire.SetExtension(Constants.DiscretionUri, new Coding(Constants.DiscretionSystem, masterDetail.Master.Discretion));
                }

                if (masterDetail.Master.GeneratePdf.HasValue)
                {
                    questionnaire.SetExtension(Constants.GeneratePdfUri, new FhirBoolean(masterDetail.Master.GeneratePdf.Value));
                }
                else
                {
                    questionnaire.SetExtension(Constants.GeneratePdfUri, new FhirBoolean(true));
                }

                if (masterDetail.Master.GenerateNarrative.HasValue)
                {
                    questionnaire.SetExtension(Constants.GenerateNarrativeUri, new FhirBoolean(masterDetail.Master.GenerateNarrative.Value));
                }
                else
                {
                    questionnaire.SetExtension(Constants.GenerateNarrativeUri, new FhirBoolean(true));
                }

                if (!string.IsNullOrWhiteSpace(masterDetail.Master.PresentationButtons))
                {
                    questionnaire.SetExtension(Constants.PresentationButtonsUri, new Coding(Constants.PresentationButtonsSystem, masterDetail.Master.PresentationButtons));
                }
                else
                {
                    questionnaire.SetExtension(Constants.PresentationButtonsUri, new Coding(Constants.PresentationButtonsSystem, "sticky"));
                }

                if (!string.IsNullOrWhiteSpace(masterDetail.Master.Code))
                {
                    questionnaire.Code = ParseArrayOfCoding(masterDetail.Master.Code);
                }

                IList<string> linkIds = new List<string>();
                Questionnaire.ItemComponent item = null;
                for (int i = 0; i < masterDetail.Details.Length; i++)
                {
                    QuestionnaireItem2 questionnaireItem = masterDetail.Details[i];

                    if (linkIds.IndexOf(questionnaireItem.LinkId) > 0) throw new DuplicateLinkIdException(questionnaireItem.LinkId);

                    _logger.LogDebug($"Questionnaire Item: {questionnaireItem.LinkId} - {questionnaireItem.Type} - {questionnaireItem.Text}");

                    int level = questionnaireItem.LinkId.Split('.').Length - 1;
                    if (level > 0)
                    {
                        // item could be null if a line in the .txt-file is empty or blank
                        if (item == null) continue;

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

        private int DiveV2(int index, int level, List<Questionnaire.ItemComponent> itemComponents, QuestionnaireItem2[] questionnaireItems)
        {
            int currentIndex = index;

            Questionnaire.ItemComponent item = null;
            for (; currentIndex < questionnaireItems.Length; currentIndex++)
            {
                QuestionnaireItem2 questionnaireItem = questionnaireItems[currentIndex];
                _logger.LogDebug($"Questionnaire Item: {questionnaireItem.LinkId} - {questionnaireItem.Type} - {questionnaireItem.Text}");

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

        private Questionnaire.ItemComponent CreateItemComponentV2(QuestionnaireItem2 item)
        {
            Questionnaire.QuestionnaireItemType? itemType = EnumUtility.ParseLiteral<Questionnaire.QuestionnaireItemType>(item.Type);
            if (!itemType.HasValue) throw new Exception($"QuestionnaireItemType at question with linkId: {item.LinkId} is not conforming to any valid literals. QuestionnaireItemType: {item.Type}");

            Questionnaire.ItemComponent itemComponent = new Questionnaire.ItemComponent
            {
                Type = itemType,
            };

            itemComponent.LinkId = string.IsNullOrWhiteSpace(item.LinkId) ? null : item.LinkId;
            itemComponent.Prefix = string.IsNullOrWhiteSpace(item.Prefix) ? null : item.Prefix;
            itemComponent.Text = string.IsNullOrWhiteSpace(item.Text) ? null : item.Text;
            if (!string.IsNullOrWhiteSpace(item.EnableWhen))
            {
                itemComponent.EnableWhen = ParseEnableWhen(item.EnableWhen).ToList();
                // TODO: Defaults to 'any' in the first iteration of "migrate to R4".
                itemComponent.EnableBehavior = Questionnaire.EnableWhenBehavior.Any;
            }
            if (itemType != Questionnaire.QuestionnaireItemType.Group && itemType != Questionnaire.QuestionnaireItemType.Display)
            {
                itemComponent.Required = item.Required.HasValue ? item.Required : null;
                itemComponent.ReadOnly = item.ReadOnly;
                itemComponent.Initial = string.IsNullOrEmpty(item.Initial)
                    ? null
                    : new List<Questionnaire.InitialComponent> { new Questionnaire.InitialComponent { Value = GetElement(itemType.Value, item.Initial) } };
                itemComponent.MaxLength = item.MaxLength.HasValue ? item.MaxLength : null;
            }

            if (itemType != Questionnaire.QuestionnaireItemType.Display)
            {
                itemComponent.Repeats = item.Repeats;
            }

            if (!string.IsNullOrWhiteSpace(item.ValidationText))
                itemComponent.SetStringExtension(Constants.ValidationTextUri, item.ValidationText);

            if (!string.IsNullOrWhiteSpace(item.Options) && item.Options.IndexOf('#') == 0)
                itemComponent.AnswerValueSetElement = new Canonical($"#{item.Options.Substring(1)}");

            if (!string.IsNullOrWhiteSpace(item.EntryFormat))
                itemComponent.SetStringExtension(Constants.EntryFormatUri, item.EntryFormat);

            if (item.MaxValueInteger.HasValue)
                itemComponent.SetIntegerExtension(Constants.MaxValueUri, item.MaxValueInteger.Value);
            if (item.MinValueInteger.HasValue)
                itemComponent.SetIntegerExtension(Constants.MinValueUri, item.MinValueInteger.Value);

            if (item.MaxValueDate.HasValue)
                itemComponent.SetExtension(Constants.MaxValueUri, new FhirDateTime(new DateTimeOffset(item.MaxValueDate.Value.ToUniversalTime())));
            if (item.MinValueDate.HasValue)
                itemComponent.SetExtension(Constants.MinValueUri, new FhirDateTime(new DateTimeOffset(item.MinValueDate.Value.ToUniversalTime())));

            if (item.MinLength.HasValue)
                itemComponent.SetIntegerExtension(Constants.MinLenghtUri, item.MinLength.Value);

            if (item.MaxDecimalPlaces.HasValue)
                itemComponent.SetIntegerExtension(Constants.MaxDecimalPlacesUri, item.MaxDecimalPlaces.Value);

            if (!string.IsNullOrWhiteSpace(item.RepeatsText))
                itemComponent.SetStringExtension(Constants.RepeatsTextUri, item.RepeatsText);

            if (!string.IsNullOrWhiteSpace(item.ItemControl))
            {
                CodeableConcept codeableConcept = new CodeableConcept
                {
                    Coding = new List<Coding> { new Coding
                        {
                            System = Constants.ItemControlSystem,
                            Code = item.ItemControl
                        }
                    }
                };

                itemComponent.SetExtension(Constants.ItemControlUri, codeableConcept);
            }

            if (item.MaxOccurs.HasValue)
                itemComponent.SetIntegerExtension(Constants.MaxOccursUri, item.MaxOccurs.Value);
            if (item.MinOccurs.HasValue)
                itemComponent.SetIntegerExtension(Constants.MinOccursUri, item.MinOccurs.Value);

            if (!string.IsNullOrWhiteSpace(item.Regex))
                itemComponent.SetStringExtension(Constants.RegexUri, item.Regex);

            if (!string.IsNullOrWhiteSpace(item.Markdown))
            {
                if (itemComponent.Text == null) throw new MissingRequirementException($"Question with linkId: {item.LinkId}. The 'Text' attribute is required when setting the 'Markdown' extension so that form fillers which do not support the 'Markdown' extension still can display informative text to the user.");
                itemComponent.TextElement.SetExtension(Constants.RenderingMarkdownUri, new Markdown(item.Markdown));
            }
            if (!string.IsNullOrWhiteSpace(item.Unit))
            {
                Coding unitCoding = ParseCoding(item.Unit);
                itemComponent.SetExtension(Constants.QuestionnaireUnitUri, unitCoding);
            }

            if (!string.IsNullOrWhiteSpace(item.Code))
            {
                itemComponent.Code = ParseArrayOfCoding(item.Code);
            }

            if (!string.IsNullOrWhiteSpace(item.Option))
            {
                List<Element> options = ParseArrayOfElement(item.Option);
                foreach (Element element in options)
                {
                    if (element is ResourceReference)
                    {
                        itemComponent.AddExtension(Constants.OptionReferenceUri, element);
                    }
                    else
                    {
                        itemComponent.AnswerOption.Add(new Questionnaire.AnswerOptionComponent { Value = element });
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(item.FhirPathExpression))
            {
                itemComponent.SetStringExtension(Constants.FhirPathUri, item.FhirPathExpression);
            }

            if (item.Hidden)
            {
                itemComponent.SetBoolExtension(Constants.QuestionnaireItemHidden, item.Hidden);
            }

            if (item.AttachmentMaxSize.HasValue && itemComponent.Type == Questionnaire.QuestionnaireItemType.Attachment)
            {
                itemComponent.SetExtension(Constants.QuestionnaireAttachmentMaxSize, new FhirDecimal(item.AttachmentMaxSize));
            }

            if (!string.IsNullOrWhiteSpace(item.CalculatedExpression))
            {
                itemComponent.SetStringExtension(Constants.CalculatedExpressionUri, item.CalculatedExpression);
            }

            if (!string.IsNullOrWhiteSpace(item.GuidanceAction))
            {
                itemComponent.SetStringExtension(Constants.GuidanceActionUri, item.GuidanceAction.Trim());
            }

            if (!string.IsNullOrWhiteSpace(item.GuidanceParameter))
            {
                itemComponent.SetStringExtension(Constants.GuidanceParameterUri, $"hn_frontend_{item.GuidanceParameter.Trim()}");
            }

            if (!string.IsNullOrWhiteSpace(item.FhirPathValidation))
            {
                itemComponent.SetStringExtension(Constants.FhirPathValidationUri, item.FhirPathValidation);
            }

            if (!string.IsNullOrWhiteSpace(item.FhirPathMaxValue))
            {
                itemComponent.SetStringExtension(Constants.SdfMaxValueUri, item.FhirPathMaxValue);
            }

            if (!string.IsNullOrWhiteSpace(item.FhirPathMinValue))
            {
                itemComponent.SetStringExtension(Constants.SdfMinValueUri, item.FhirPathMinValue);
            }

            return itemComponent;
        }

        private IList<Questionnaire> GetQuestionnairesFromFlatFileFormatV1(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path)) throw new FileNotFoundException($"File not found: '{path}'", path);

            IList<Questionnaire> questionnaires = new List<Questionnaire>();

            var engine = new MasterDetailEngine<QuestionnaireHeader, QuestionnaireItem>(new MasterDetailSelector(RecordSelector))
            {
                Encoding = new UTF8Encoding()
            };
            MasterDetails<QuestionnaireHeader, QuestionnaireItem>[] masterDetails = engine.ReadFile(path);
            foreach (MasterDetails<QuestionnaireHeader, QuestionnaireItem> masterDetail in masterDetails)
            {
                _logger.LogDebug($"Questionnaire: {masterDetail.Master.Name} - {masterDetail.Master.Title}");

                Questionnaire questionnaire = new Questionnaire();

                questionnaire.Meta = new Meta
                {
                    Profile = new string[] { "http://ehelse.no/fhir/StructureDefinition/sdf-Questionnaire" }
                };

                questionnaire.Id = string.IsNullOrWhiteSpace(masterDetail.Master.Id) ? null : masterDetail.Master.Id;
                questionnaire.Url = string.IsNullOrWhiteSpace(masterDetail.Master.Url) ? null : masterDetail.Master.Url;
                questionnaire.Version = string.IsNullOrWhiteSpace(masterDetail.Master.Version) ? null : masterDetail.Master.Version;
                questionnaire.Name = string.IsNullOrWhiteSpace(masterDetail.Master.Name) ? null : masterDetail.Master.Name;
                questionnaire.Title = string.IsNullOrWhiteSpace(masterDetail.Master.Title) ? null : masterDetail.Master.Title;
                questionnaire.Status = string.IsNullOrWhiteSpace(masterDetail.Master.Status) ? null : EnumUtility.ParseLiteral<PublicationStatus>(masterDetail.Master.Status);
                questionnaire.Date = string.IsNullOrWhiteSpace(masterDetail.Master.Date) ? null : masterDetail.Master.Date;
                questionnaire.Publisher = string.IsNullOrWhiteSpace(masterDetail.Master.Publisher) ? null : masterDetail.Master.Publisher;
                questionnaire.Description = string.IsNullOrWhiteSpace(masterDetail.Master.Description) ? null : new Markdown(masterDetail.Master.Description);
                questionnaire.Purpose = string.IsNullOrWhiteSpace(masterDetail.Master.Purpose) ? null : new Markdown(masterDetail.Master.Purpose);
                questionnaire.Contact = string.IsNullOrWhiteSpace(masterDetail.Master.Contact) ? null : new List<ContactDetail> { new ContactDetail { Telecom = new List<ContactPoint> { new ContactPoint { System = ContactPoint.ContactPointSystem.Url, Value = masterDetail.Master.Contact } } } };

                if (!string.IsNullOrWhiteSpace(masterDetail.Master.Language))
                {
                    questionnaire.Language = masterDetail.Master.Language;
                    // TODO: Vi trenger definere Visningsnavn for språket, eksempelvis: Norsk (bokmål), osv.
                    questionnaire.Meta.Tag.Add(new Coding("urn:ietf:bcp:47", questionnaire.Language));
                }

                IList<string> linkIds = new List<string>();
                Questionnaire.ItemComponent item = null;
                for (int i = 0; i < masterDetail.Details.Length; i++)
                {
                    QuestionnaireItem questionnaireItem = masterDetail.Details[i];

                    if (linkIds.IndexOf(questionnaireItem.LinkId) > 0) throw new DuplicateLinkIdException(questionnaireItem.LinkId);

                    _logger.LogDebug($"Questionnaire Item: {questionnaireItem.LinkId} - {questionnaireItem.Type} - {questionnaireItem.Text}");

                    int level = questionnaireItem.LinkId.Split('.').Length - 1;
                    if (level > 0)
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

        private int DiveV1(int index, int level, List<Questionnaire.ItemComponent> itemComponents, QuestionnaireItem[] questionnaireItems)
        {
            int currentIndex = index;

            Questionnaire.ItemComponent item = null;
            for (; currentIndex < questionnaireItems.Length; currentIndex++)
            {
                QuestionnaireItem questionnaireItem = questionnaireItems[currentIndex];
                _logger.LogDebug($"Questionnaire Item: {questionnaireItem.LinkId} - {questionnaireItem.Type} - {questionnaireItem.Text}");

                int currentLevel = questionnaireItem.LinkId.Split('.').Length - 1;
                if (currentLevel == level)
                {
                    item = CreateItemComponentV1(questionnaireItem);
                    itemComponents.Add(item);
                }
                else if (currentLevel > level)
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

        private static Questionnaire.ItemComponent CreateItemComponentV1(QuestionnaireItem item)
        {
            Questionnaire.QuestionnaireItemType? itemType = EnumUtility.ParseLiteral<Questionnaire.QuestionnaireItemType>(item.Type);
            if (!itemType.HasValue) throw new Exception($"QuestionnaireItemType at question with linkId: {item.LinkId} is not conforming to any valid literals. QuestionnaireItemType: {item.Type}");

            Questionnaire.ItemComponent itemComponent = new Questionnaire.ItemComponent
            {
                Type = itemType,
            };

            itemComponent.LinkId = string.IsNullOrWhiteSpace(item.LinkId) ? null : item.LinkId;
            itemComponent.Prefix = string.IsNullOrWhiteSpace(item.Prefix) ? null : item.Prefix;
            itemComponent.Text = string.IsNullOrWhiteSpace(item.Text) ? null : item.Text;
            if (!string.IsNullOrWhiteSpace(item.EnableWhen))
            {
                itemComponent.EnableWhen = ParseEnableWhen(item.EnableWhen).ToList();
                // TODO: Defaults to 'any' in the first iteration of "migrate to R4".
                itemComponent.EnableBehavior = Questionnaire.EnableWhenBehavior.Any;
            }

            if (itemType != Questionnaire.QuestionnaireItemType.Group && itemType != Questionnaire.QuestionnaireItemType.Display)
            {
                itemComponent.Required = item.Required.HasValue ? item.Required : null;
                itemComponent.ReadOnly = item.ReadOnly;
                itemComponent.Initial = string.IsNullOrEmpty(item.Initial)
                    ? null
                    : new List<Questionnaire.InitialComponent> { new Questionnaire.InitialComponent { Value = GetElement(itemType.Value, item.Initial) } };
            }

            if (itemType != Questionnaire.QuestionnaireItemType.Display)
            {
                itemComponent.Repeats = item.Repeats;
            }

            if (!string.IsNullOrWhiteSpace(item.ValidationText))
                itemComponent.SetStringExtension(Constants.ValidationTextUri, item.ValidationText);
            if (!string.IsNullOrWhiteSpace(item.ReferenceValue) && item.ReferenceValue.IndexOf('#') == 0)
                itemComponent.AnswerValueSetElement = new Canonical($"#{item.ReferenceValue.Substring(1)}");
            if (!string.IsNullOrWhiteSpace(item.EntryFormat))
                itemComponent.SetStringExtension(Constants.EntryFormatUri, item.EntryFormat);
            if (item.MaxValue.HasValue)
                itemComponent.SetIntegerExtension(Constants.MaxValueUri, item.MaxValue.Value);
            if (item.MinValue.HasValue)
                itemComponent.SetIntegerExtension(Constants.MinValueUri, item.MinValue.Value);

            return itemComponent;
        }

        private static Element GetElement(Questionnaire.QuestionnaireItemType itemType, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            switch (itemType)
            {
                case Questionnaire.QuestionnaireItemType.Boolean:
                    return new FhirBoolean(bool.Parse(value));
                case Questionnaire.QuestionnaireItemType.Integer:
                    return new Integer(int.Parse(value));
                case Questionnaire.QuestionnaireItemType.Decimal:
                    return new FhirDecimal(decimal.Parse(value, CultureInfo.InvariantCulture));
                case Questionnaire.QuestionnaireItemType.DateTime:
                    return new FhirDateTime(DateTimeOffset.Parse(value).ToUniversalTime());
                case Questionnaire.QuestionnaireItemType.Date:
                    return new Date(value);
                case Questionnaire.QuestionnaireItemType.Time:
                    return new Time(value);
                case Questionnaire.QuestionnaireItemType.String:
                case Questionnaire.QuestionnaireItemType.Text:
                    return new FhirString(value);
                case Questionnaire.QuestionnaireItemType.Choice:
                    return ParseElement(value);
                default:
                    return null;
            }
        }

        private IList<ValueSet> GetValueSetsFromFlatFileFormat(string path, bool genereateNarrative = true)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path)) throw new FileNotFoundException($"File not found: '{path}'.", path);

            IList<ValueSet> valueSets = new List<ValueSet>();
            var engine = new MasterDetailEngine<ValueSetHeader, ValueSetCodeReferences>(new MasterDetailSelector(RecordSelector))
            {
                Encoding = new UTF8Encoding()
            };
            MasterDetails<ValueSetHeader, ValueSetCodeReferences>[] masterDetails = engine.ReadFile(path);
            foreach (MasterDetails<ValueSetHeader, ValueSetCodeReferences> masterDetail in masterDetails)
            {
                _logger.LogDebug($"ValueSet: {masterDetail.Master.Id} - {masterDetail.Master.Title}");

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
                    _logger.LogDebug($"ValueSetCodeReference: {valueSetCodeReference.Code} - {valueSetCodeReference.Display}");

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

        private static List<UsageContext> ParseUsageContext(string value)
        {
            List<UsageContext> usageContexts = new List<UsageContext>();

            JObject usageContextObject = JObject.Parse(value);
            JArray usageContextArray = usageContextObject["useContext"] as JArray;

            List<UsageContextElement> usageContextElements = JsonConvert.DeserializeObject<List<UsageContextElement>>(usageContextArray.ToString());
            foreach (UsageContextElement usageContextElement in usageContextElements)
            {
                UsageContext usageContext = new UsageContext();
                if (usageContextElement.Code != null)
                {
                    usageContext.Code = new Coding
                    {
                        System = string.IsNullOrWhiteSpace(usageContextElement.Code.System) ? null : usageContextElement.Code.System,
                        Code = string.IsNullOrWhiteSpace(usageContextElement.Code.Code) ? null : usageContextElement.Code.Code,
                        Display = string.IsNullOrWhiteSpace(usageContextElement.Code.Display) ? null : usageContextElement.Code.Display
                    };
                }
                if (usageContextElement.ValueCodeableConcept != null)
                {
                    foreach (CodingElement codingElement in usageContextElement.ValueCodeableConcept.Coding)
                    {
                        usageContext.Value = new CodeableConcept
                        {
                            Coding = new List<Coding>
                            {
                                new Coding
                                {
                                    System = string.IsNullOrWhiteSpace(codingElement.System) ? null : codingElement.System,
                                    Code = string.IsNullOrWhiteSpace(codingElement.Code) ? null : codingElement.Code,
                                    Display = string.IsNullOrWhiteSpace(codingElement.Display) ? null : codingElement.Display
                                }
                            }
                        };
                    }
                }

                usageContexts.Add(usageContext);
            }

            return usageContexts;
        }

        private static IEnumerable<Questionnaire.EnableWhenComponent> ParseEnableWhen(string value)
        {
            JObject enableWhenObject = JObject.Parse(value);
            JArray enableWhenArray = enableWhenObject["EnableWhen"] as JArray;
            IList<EnableWhenElement> enableWhenList = JsonConvert.DeserializeObject<List<EnableWhenElement>>(enableWhenArray.ToString());
            foreach (EnableWhenElement enableWhen in enableWhenList)
            {
                Questionnaire.EnableWhenComponent enableWhenComponent = new Questionnaire.EnableWhenComponent
                {
                    Question = enableWhen.Question,
                    // TODO: Defaults to Equal for the first iteration of "migrate to R4"
                    Operator = Questionnaire.QuestionnaireItemOperator.Equal,
                };
                if (enableWhen.HasAnswer.HasValue)
                {
                    if (enableWhen.Answer == null)
                    {
                        enableWhenComponent.Operator = Questionnaire.QuestionnaireItemOperator.Exists;
                        enableWhenComponent.Answer = new FhirBoolean(enableWhen.HasAnswer);
                    }
                }
                if (enableWhen.AnswerBoolean.HasValue)
                    enableWhenComponent.Answer = new FhirBoolean(enableWhen.AnswerBoolean);
                if (enableWhen.AnswerDecimal.HasValue)
                    enableWhenComponent.Answer = new FhirDecimal(enableWhen.AnswerDecimal);
                if (enableWhen.AnswerInteger.HasValue)
                    enableWhenComponent.Answer = new Integer(enableWhen.AnswerInteger);
                if (!string.IsNullOrWhiteSpace(enableWhen.AnswerDate))
                    enableWhenComponent.Answer = new Date(enableWhen.AnswerDate);
                if (!string.IsNullOrWhiteSpace(enableWhen.AnswerDateTime))
                    enableWhenComponent.Answer = new FhirDateTime(enableWhen.AnswerDateTime);
                if (!string.IsNullOrWhiteSpace(enableWhen.AnswerTime))
                    enableWhenComponent.Answer = new Time(enableWhen.AnswerTime);
                if (!string.IsNullOrWhiteSpace(enableWhen.AnswerString))
                    enableWhenComponent.Answer = new FhirString(enableWhen.AnswerString);
                if (enableWhen.AnswerCoding != null)
                    enableWhenComponent.Answer = new Coding(enableWhen.AnswerCoding.System, enableWhen.AnswerCoding.Code);
                if (enableWhen.AnswerQuantity != null)
                {
                    Quantity quantity = new Quantity();
                    if (enableWhen.AnswerQuantity.Value.HasValue)
                        quantity.Value = enableWhen.AnswerQuantity.Value.Value;
                    if (!string.IsNullOrWhiteSpace(enableWhen.AnswerQuantity.System))
                        quantity.System = enableWhen.AnswerQuantity.System;
                    if (!string.IsNullOrWhiteSpace(enableWhen.AnswerQuantity.Code))
                        quantity.Code = enableWhen.AnswerQuantity.Code;
                    if (!string.IsNullOrWhiteSpace(enableWhen.AnswerQuantity.Unit))
                        quantity.Unit = enableWhen.AnswerQuantity.Unit;
                    enableWhenComponent.Answer = quantity;
                }
                if (enableWhen.AnswerReference != null)
                {
                    enableWhenComponent.Answer = new ResourceReference(enableWhen.AnswerReference.Reference);
                }

                yield return enableWhenComponent;
            }
        }

        private static List<Element> ParseArrayOfElement(string value)
        {
            List<Element> elements = new List<Element>();

            JArray arrayOfCoding = JArray.Parse(value);
            foreach (JToken token in arrayOfCoding)
            {
                elements.Add(ParseElement(token.ToString()));
            }

            return elements;
        }

        private static Element ParseElement(string value)
        {
            JObject elementJObject = JObject.Parse(value);
            Element element = null;
            IList<Extension> extension = new List<Extension>();

            if (elementJObject.ContainsKey("extension"))
            {
                FhirJsonParser parser = new FhirJsonParser();
                JArray arrayOfExtension = JArray.Parse(elementJObject["extension"].ToString());
                foreach (JToken token in arrayOfExtension)
                {
                    extension.Add(parser.Parse<Extension>(token.ToString()));
                }
            }

            if (elementJObject.ContainsKey("valueBoolean") && FhirBoolean.IsValidValue(elementJObject["valueBoolean"].ToString()))
            {
                bool? valueBoolean = null;
                if (bool.TryParse(elementJObject["valueBoolean"].ToString(), out bool b)) valueBoolean = b;
                element = new FhirBoolean(valueBoolean);
            }
            else if (elementJObject.ContainsKey("valueDecimal") && FhirDecimal.IsValidValue(elementJObject["valueDecimal"].ToString()))
            {
                decimal? valueDecimal = null;
                if (decimal.TryParse(elementJObject["valueDecimal"].ToString(), out decimal d)) valueDecimal = d;
                element = new FhirDecimal(valueDecimal);
            }
            else if (elementJObject.ContainsKey("valueInteger"))
            {
                element = new Integer((int?)elementJObject["valueInteger"]);
            }
            else if (elementJObject.ContainsKey("valueDate"))
            {
                element = new Date(elementJObject["valueDate"].ToString());
            }
            else if (elementJObject.ContainsKey("valueDateTime"))
            {
                element = new FhirDateTime(elementJObject["valueDateTime"].ToString());
            }
            else if (elementJObject.ContainsKey("valueTime"))
            {
                element = new Time(elementJObject["valueTime"].ToString());
            }
            else if (elementJObject.ContainsKey("valueString"))
            {
                element = new FhirString(elementJObject["valueString"].ToString());
            }
            else if (elementJObject.ContainsKey("valueUri"))
            {
                element = new FhirUri(elementJObject["valueUri"].ToString());
            }
            else if (elementJObject.ContainsKey("valueAttachment"))
            {
                FhirJsonParser parser = new FhirJsonParser();
                element = parser.Parse<Attachment>(elementJObject["valueAttachment"].ToString());
            }
            else if (elementJObject.ContainsKey("valueCoding"))
            {
                FhirJsonParser parser = new FhirJsonParser();
                element = parser.Parse<Coding>(elementJObject["valueCoding"].ToString());
            }
            else if (elementJObject.ContainsKey("valueQuantity"))
            {
                FhirJsonParser parser = new FhirJsonParser();
                element = parser.Parse<Quantity>(elementJObject["valueQuantity"].ToString());
            }
            else if (elementJObject.ContainsKey("valueReference"))
            {
                //element = new ResourceReference(c);
                FhirJsonParser parser = new FhirJsonParser();
                element = parser.Parse<ResourceReference>(elementJObject["valueReference"].ToString());
            }

            if (element != null && extension.Count > 0)
                element.Extension.AddRange(extension);

            return element;
        }

        private static List<Coding> ParseArrayOfCoding(string value)
        {
            List<Coding> codings = new List<Coding>();
            JArray arrayOfCoding = JArray.Parse(value);
            foreach (JToken token in arrayOfCoding)
            {
                codings.Add((Coding)ParseElement(token.ToString()));
            }

            return codings;
        }

        private static Coding ParseCoding(string value)
        {
            JObject codingJObject = JObject.Parse(value);
            CodingElement valueCoding = codingJObject.ToObject<CodingElement>();
            if (string.IsNullOrWhiteSpace(valueCoding.System))
                throw new RequiredAttributeException("When parsing a Coding type required attribute System does not have a value.", "System");
            if (string.IsNullOrWhiteSpace(valueCoding.Code))
                throw new RequiredAttributeException("When parsing a Coding type required attribute Code does not have a value.", "Code");

            Coding coding = new Coding
            {
                System = valueCoding.System,
                Code = valueCoding.Code
            };
            if (!string.IsNullOrWhiteSpace(valueCoding.Display))
                coding.Display = valueCoding.Display;

            return coding;
        }
    }
}
