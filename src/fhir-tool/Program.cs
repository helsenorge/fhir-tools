using FhirTool.Configuration;
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace FhirTool
{
    partial class Program
    {
        private static readonly string ProxyBaseUrl = "https://skjemakatalog-test-fhir-apimgmt.azure-api.net/api/v1/";

        private static string FileNameReservedCharacters = "<>:\"/\\|?*";
        private static FhirToolArguments _arguments = null;
        private static TextWriter _out = null;

        // First example
        // fhir-tool.exe --version 1 --questionnaire HELFO_E106_NB.txt --valueset HELFO_E106_NB_Kodeverk.txt --fhir-base-url https://skjemakatalog-dev-fhir-api.azurewebsites.net/ --resolve-url

        // Splitting up operations
        // fhir-tool.exe generate --version 1 --format json --questionnaire HELFO_E121_NB.txt --valueset HELFO_E121_NB_Kodeverk.txt
        // fhir-tool.exe upload --format json --questionnaire Questionnaire-Helfo_E121_NB-no.json --fhir-base-url https://skjemakatalog-dev-fhir-api.azurewebsites.net/ --resolve-url

        // fhir-tool.exe generate --version 2 --format xml --questionnaire KlamydiatestHelseVest_NN.txt --valueset KlamydiatestHelseVest_Kodeverk_NN.txt
        // fhir-tool.exe generate --version 2 --format xml --questionnaire HelseVestFøde_NB.txt --valueset HelseVestFøde_Kodeverk_NB.txt
        // fhir-tool.exe generate --version 2 --format xml --questionnaire AlleKonstruksjoner_NB.txt --valueset AlleKonstruksjoner_Kodeverk_NB.txt
        // fhir-tool.exe upload --format xml --questionnaire HV-KGBS-nb-NN-1.xml --fhir-base-url https://skjemakatalog-dev-fhir-api.azurewebsites.net/ --resolve-url
        // fhir-tool.exe upload --format xml --questionnaire HVIIFJ-nb-NO-0.1.xml --fhir-base-url https://skjemakatalog-dev-fhir-api.azurewebsites.net/ --resolve-url
        // fhir-tool.exe upload --format xml --questionnaire Ehelse_AlleKonstruksjoner-nb-NO-0.1.xml --fhir-base-url https://skjemakatalog-dev-fhir-api.azurewebsites.net/ --resolve-url
        // fhir-tool.exe upload --format xml --questionnaire Ehelse_AlleKonstruksjoner-nb-NO-0.1.xml --environment dev
        
        // fhir-tool.exe verify-validation --questionnaire Ehelse_AlleKonstruksjoner-nb-NO-0.1.xml

        // fhir-tool.exe upload-definitions --format xml --source C:\dev\src\fhir-sdf\resources\StructureDefinition --fhir-base-url https://skjemakatalog-dev-fhir-api.azurewebsites.net/ --resolve-url --credentials user:password

        // fhir-tool.exe bundle --format xml --source C:\dev\src\fhir-sdf\resources\StructureDefinition --out C:\dev\src\fhir-sdf\
        // fhir-tool.exe bundle --format xml --source F:\dev\src\fhir-sdf\resources --out F:\dev\src\fhir-sdf\

        // fhir-tool.exe upload-resources -- format xml --source C:\dev\src\fhir-sdf\resources\StructureDefinition

        // fhir-tool.exe upload-definitions --format xml --source F:\patient-observations.xml --fhir-base-url https://smart-fhir-api.azurewebsites.net/fhir
        // fhir-tool.exe upload-definitions --format xml --source F:\dev\src\fhir-sdf\resources\StructureDefinition --fhir-base-url https://stu3.simplifier.net/Digitaleskjema --credentials <username>:<password>

        // fhir-tool.exe split-bundle --format xml --source F:\patient-observations.xml --format xml

        // Unsure if we should handle kith and messaging in this tool
        // fhir-tool.exe generate-kith --questionnaire Questionnaire-Helfo_E121_NB-no.xml --fhir-base-url https://skjemakatalog-dev-fhir-api.azurewebsites.net/ --resolve-url
        // fhir-tool.exe sendasync --questionnaire Questionnaire-Helfo_E121_NB-no.xml --fhir-base-url https://skjemakatalog-dev-fhir-api.azurewebsites.net/ --resolve-url

        // fhir-tool.exe transfer-data --environment-source test --environment-destination qa --resourcetype Questionnaire --searchcount 1000
        static void Main(string[] args)
        {
            try
            {
                _out = Console.Out;
                _arguments = FhirToolArguments.Create(args);
                if (string.IsNullOrWhiteSpace(_arguments.ProxyBaseUrl))
                    _arguments.ProxyBaseUrl = ProxyBaseUrl;

                Logger.Initialize(_out, _arguments.Verbose);

                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
                Logger.WriteLineToOutput($"{versionInfo.ProductName} [Version {Assembly.GetEntryAssembly().GetName().Version}]");
                Logger.WriteLineToOutput($"(c) 2018 {versionInfo.CompanyName}. All rights reserved.");
                Logger.WriteLineToOutput();

                Logger.DebugWriteLineToOutput("Validating command line arguments.");
                if (!_arguments.Validate())
                {
                    Logger.ErrorWriteLineToOutput("One or more arguments did not validate. Please verify your arguments according to output.");
                    goto exit;
                }

                switch(_arguments.Operation)
                {
                    case OperationEnum.Generate:
                        string filename = GenerateFromFlatFileOperation(_arguments);
                        if (!string.IsNullOrWhiteSpace(filename))
                        {
                            if (!_arguments.SkipValidation)
                            {
                                Logger.WriteLineToOutput("\nVerifying validation requirements");
                                VerifyItemValidation(new FhirToolArguments { QuestionnairePath = filename, MimeType = _arguments.MimeType });
                            }
                        }
                        break;
                    case OperationEnum.Upload:
                        if (_arguments.SkipValidation)
                        {
                            UploadToFhirServerOperation(_arguments);
                        }
                        else
                        {
                            Logger.WriteLineToOutput("\nVerifying validation requirements");
                            if (VerifyItemValidation(_arguments)) UploadToFhirServerOperation(_arguments);
                        }
                        break;
                    case OperationEnum.UploadDefinitions:
                        UploadDefintitionsOperation(_arguments);
                        break;
                    case OperationEnum.Bundle:
                        BundleResourcesOperation(_arguments);
                        break;
                    case OperationEnum.SplitBundle:
                        SplitBundleOperation(_arguments);
                        break;
                    case OperationEnum.TransferData:
                        TransferData(_arguments);
                        break;
                    case OperationEnum.VerifyValidation:
                        Console.WriteLine($"VerifyItemValidation: {VerifyItemValidation(_arguments)}");
                        break;
                    default:
                        throw new NotSupportedOperationException(_arguments.Operation);
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorWriteLineToOutput($"{ex.Message}");
            }

            exit:
            Logger.WriteLineToOutput("\nPress any key to exit. . .");
            Console.ReadKey(true);
        }

        private static bool VerifyItemValidation(FhirToolArguments arguments)
        {
            if (string.IsNullOrEmpty(arguments.QuestionnairePath))
            {
                Logger.ErrorWriteLineToOutput($"Operation '{arguments.Operation}' requires argument '{FhirToolArguments.QUESTIONNAIRE_ARG}' or '{FhirToolArguments.QUESTIONNAIRE_SHORT_ARG}'.");
                return false;
            }
            string mimeType = arguments.MimeType;
            if(string.IsNullOrEmpty(mimeType))
            {
                string extension = Path.GetExtension(arguments.QuestionnairePath).ToLowerInvariant();
                // Remove the '.' in front of the extension
                if (!string.IsNullOrEmpty(extension)) extension = extension.Substring(1);
                // Is supported MimeType?
                if(!FhirToolArguments.SUPPORTED_MIMETYPES.Contains(extension))
                {
                    Logger.ErrorWriteLineToOutput($"Operation '{arguments.Operation}' requires argument '{FhirToolArguments.MIMETYPE_ARG}' or '{FhirToolArguments.MIMETYPE_SHORT_ARG}'.");
                }
                mimeType = extension;
            }

            Logger.WriteLineToOutput($"Deserializing Fhir resource at '{arguments.QuestionnairePath}'.");
            Logger.WriteLineToOutput($"Expecting format: '{mimeType}'.");
            Questionnaire questionnaire = null;
            switch (mimeType)
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
                Logger.ErrorWriteLineToOutput($"Failed to extract Questionnaire from flat file format\nLocation: '{arguments.QuestionnairePath}'.");
                return false;
            }

            IEnumerable<MissingValidationIssue> issues = VerifyItemValidation(questionnaire);

            foreach (MissingValidationIssue issue in issues)
            {
                switch (issue.Severity)
                {
                    case MissingValidationSeverityEnum.Error:
                        Logger.ErrorWriteLineToOutput($"LinkId: {issue.LinkId}, Severity: {issue.Severity}, Details: {issue.Details}");
                        break;
                    case MissingValidationSeverityEnum.Warning:
                        Logger.WarnWriteLineToOutput($"LinkId: {issue.LinkId}, Severity: {issue.Severity}, Details: {issue.Details}");
                        break;
                    case MissingValidationSeverityEnum.Information:
                        Logger.InfoWriteLineToOutput($"LinkId: {issue.LinkId}, Severity: {issue.Severity}, Details: {issue.Details}");
                        break;
                    default:
                        Logger.WriteLineToOutput($"LinkId: {issue.LinkId}, Severity: {issue.Severity}, Details: {issue.Details}");
                        break;
                }
            }

            return !issues.Any(i => i.Severity == MissingValidationSeverityEnum.Error);
        }

        private static IEnumerable<MissingValidationIssue> VerifyItemValidation(Questionnaire questionnaire)
        {
            var issues = new List<MissingValidationIssue>();
            foreach(Questionnaire.ItemComponent item in questionnaire.Item)
            {
                issues.AddRange(questionnaire.VerifyItemValidation(item));
            }

            return issues;
        }

        private static void TransferData(FhirToolArguments arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments.SourceEnvironment)) throw new RequiredArgumentException($"{FhirToolArguments.ENVIRONMENT_SOURCE_ARG}|{FhirToolArguments.ENVIRONMENT_SOURCE_SHORT_ARG}");
            if (string.IsNullOrWhiteSpace(arguments.DestinationEnvironment)) throw new RequiredArgumentException($"{FhirToolArguments.ENVIRONMENT_DESTINATION_ARG}|{FhirToolArguments.ENVIRONMENT_DESTINATION_SHORT_ARG}");
            if (!FhirToolArguments.IsKnownEnvironment(arguments.SourceEnvironment)) throw new UnknownEnvironmentNameException($"Argument {FhirToolArguments.ENVIRONMENT_SOURCE_ARG}|{FhirToolArguments.ENVIRONMENT_SOURCE_SHORT_ARG} with value '{arguments.SourceEnvironment}'  is not known.", arguments.SourceEnvironment);
            if (!FhirToolArguments.IsKnownEnvironment(arguments.DestinationEnvironment)) throw new UnknownEnvironmentNameException($"Argument {FhirToolArguments.ENVIRONMENT_SOURCE_ARG}|{FhirToolArguments.ENVIRONMENT_SOURCE_SHORT_ARG} with value '{arguments.DestinationEnvironment}' is not known.", arguments.DestinationEnvironment);
            if (!arguments.ResourceType.HasValue) throw new RequiredArgumentException($"{FhirToolArguments.RESOURCETYPE_ARG}|{FhirToolArguments.RESOURCETYPE_SHORT_ARG}");

            EnvironmentElement sourceEnvironment = FhirToolArguments.GetEnvironmentElement(arguments.SourceEnvironment);
            EnvironmentElement destinationEnvironment = FhirToolArguments.GetEnvironmentElement(arguments.DestinationEnvironment);

            FhirJsonSerializer serializer = new FhirJsonSerializer();
            FhirClient sourceClient = new FhirClient(sourceEnvironment.FhirBaseUrl);
            sourceClient.ParserSettings = new ParserSettings
            {
                PermissiveParsing = true
            };
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(destinationEnvironment.FhirBaseUrl)
            };
            string relativeUrl = $"{arguments.ResourceType.GetLiteral()}";
            if (_arguments.SearchCount > 0)
                relativeUrl += $"?_count={_arguments.SearchCount}";
            Bundle sourceBundle = sourceClient.Get(relativeUrl) as Bundle;
            foreach(Bundle.EntryComponent entry in sourceBundle.Entry)
            {
                Resource resource = entry.Resource;
                string resourceType = resource.ResourceType.GetLiteral();

                if(resource is Questionnaire)
                {
                    Questionnaire questionnaire = (Questionnaire)resource;
                    // This part gets rid of some legacy
                    // TODO: Remove when we have gotten rid of the legacy
                    if (questionnaire.ApprovalDate == string.Empty) questionnaire.ApprovalDate = null;
                    if (questionnaire.LastReviewDate == string.Empty) questionnaire.LastReviewDate = null;
                    if (questionnaire.Copyright != null && questionnaire.Copyright.Value == string.Empty) questionnaire.Copyright = null;

                    // Update known properties and extensions with urls that points to the old source instance.
                    // TODO: The lines referring FhirBaseUrl is legacy and can be removed in a future version.
                    questionnaire.Url = questionnaire.Url.Replace(sourceEnvironment.ProxyBaseUrl, string.Empty);
                    questionnaire.Url = questionnaire.Url.Replace(sourceEnvironment.FhirBaseUrl, string.Empty);

                    IEnumerable<Extension> extensions = questionnaire.GetExtensions(Constants.EndPointUri);
                    foreach(Extension extension in extensions)
                    {
                        if(extension.Value is ResourceReference)
                        {
                            ResourceReference v = (ResourceReference)extension.Value;
                            v.Reference = v.Reference.Replace(sourceEnvironment.ProxyBaseUrl, string.Empty);
                            v.Reference = v.Reference.Replace(sourceEnvironment.FhirBaseUrl, string.Empty);
                        }
                    }

                    extensions = questionnaire.GetExtensions(Constants.OptionReferenceUri);
                    foreach (Extension extension in extensions)
                    {
                        if (extension.Value is ResourceReference)
                        {
                            ResourceReference v = (ResourceReference)extension.Value;
                            v.Reference = v.Reference.Replace(sourceEnvironment.ProxyBaseUrl, string.Empty);
                            v.Reference = v.Reference.Replace(sourceEnvironment.FhirBaseUrl, string.Empty);
                        }
                    }
                }

                Logger.WriteLineToOutput($"Preparing to write resource of type '{resourceType}' to '{destinationEnvironment.FhirBaseUrl}'");
                HttpContent content = new StringContent(serializer.SerializeToString(resource));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/fhir+json");
                HttpResponseMessage response;
                if (string.IsNullOrWhiteSpace(resource.Id))
                    response = client.PostAsync($"{resource.ResourceType.GetLiteral()}", content).WaitResult();
                else
                    response = client.PutAsync($"{resource.ResourceType.GetLiteral()}/{resource.Id}", content).WaitResult();

                Logger.WriteLineToOutput($"{response.StatusCode} - {response.RequestMessage.Method} {response.RequestMessage.RequestUri}");
                Logger.WriteLineToOutput();
            }
        }

        private static bool IsValidFhirDateTime(string dateTime)
        {
            try
            {
                FhirDateTime dt = new FhirDateTime(dateTime);
                DateTimeOffset offset = dt.ToDateTimeOffset(TimeSpan.Zero);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private static bool IsValidFhirDate(string date)
        {
            try
            {
                Date d = new Date(date);
                DateTimeOffset? offset = d.ToDateTimeOffset();
            }
            catch
            {
                return false;
            }

            return true;
        }

        private static string GenerateFromFlatFileOperation(FhirToolArguments arguments)
        {
            // Validate path to Questionnaire Flat File.
            if (!string.IsNullOrEmpty(_arguments.QuestionnairePath))
            {
                if (!File.Exists(_arguments.QuestionnairePath))
                {
                    Logger.ErrorWriteLineToOutput($"File specified by argument {FhirToolArguments.QUESTIONNAIRE_ARG} | {FhirToolArguments.QUESTIONNAIRE_SHORT_ARG} was not found: '{_arguments.QuestionnairePath}'");
                    return null;
                }
            }
            else
            {
                Logger.ErrorWriteLineToOutput($"Operation '{FhirToolArguments.GENERATE_OP}' requires argument {FhirToolArguments.QUESTIONNAIRE_ARG} | {FhirToolArguments.QUESTIONNAIRE_SHORT_ARG} is required.");
                return null;
            }

            Logger.WriteLineToOutput($"Expecting flat file format to conform to version: {arguments.Version}");
            IList<ValueSet> valueSets = null;
            if (!string.IsNullOrEmpty(arguments.ValueSetPath))
            {
                Logger.WriteLineToOutput($"Loading ValueSet(s) from file: '{arguments.ValueSetPath}'.");
                valueSets = GetValueSetsFromFlatFileFormat(arguments.ValueSetPath, false);
            }
            
            Questionnaire questionnaire = null;
            Logger.WriteLineToOutput($"Loading Questionnaire from file: {arguments.QuestionnairePath}");
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
                Logger.ErrorWriteLineToOutput($"Operation {FhirToolArguments.GENERATE_OP} requires argument '{FhirToolArguments.VERSION_ARG}'. Argument is either missing or an unknown value was set.\nValue: '{version}'");
                return null;
            }

            if (questionnaire == null)
            {
                Logger.ErrorWriteLineToOutput($"Failed to extract Questionnaire from flat file format\nLocation: '{arguments.QuestionnairePath}'.");
                return null;
            }
            
            if (valueSets != null && valueSets.Count > 0)
            {
                Logger.WriteLineToOutput("Adding ValueSet(s) to contained section of Questionnaire.");
                foreach (ValueSet valueSet in valueSets)
                {
                    questionnaire.Contained.Add(valueSet);
                }
            }

            string filename = $"{questionnaire.Name}-{ questionnaire.Language}-{questionnaire.Version}";
            if (arguments.MimeType == "xml")
            {
                string path = $"{filename}.xml";
                filename = GenerateLegalFilename(path);
                Logger.WriteLineToOutput($"Writing Questionnaire in xml format to local disk: {filename}");
                questionnaire.SerializeResourceToDiskAsXml(filename);
            }
            else if (arguments.MimeType == "json")
            {
                string path = $"{filename}.json";
                filename = GenerateLegalFilename(path);
                Logger.WriteLineToOutput($"Writing Questionnaire in json format to local disk: {filename}");
                questionnaire.SerializeResourceToDiskAsJson(filename);
            }
            Logger.WriteLineToOutput("Successfully generated Questionnaire.");
            Logger.WriteLineToOutput($"Questionnaire will be assigned the Id: {questionnaire.Id}");

            return filename;
        }

        private static void UploadToFhirServerOperation(FhirToolArguments arguments)
        {
            if (string.IsNullOrEmpty(arguments.FhirBaseUrl))
            {
                Logger.ErrorWriteLineToOutput($"Operation '{FhirToolArguments.UPLOAD_OP}' requires argument '{FhirToolArguments.FHIRBASEURL_ARG}' or '{FhirToolArguments.ENVIRONMENT_ARG}'.");
                return;
            }
            if (string.IsNullOrEmpty(arguments.QuestionnairePath))
            {
                Logger.ErrorWriteLineToOutput($"Operation '{FhirToolArguments.UPLOAD_OP}' requires argument '{FhirToolArguments.QUESTIONNAIRE_ARG}'.");
                return;
            }

            Logger.WriteLineToOutput($"Deserializing Fhir resource at '{arguments.QuestionnairePath}'.");
            Logger.WriteLineToOutput($"Expecting format: '{arguments.MimeType}'.");
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
                Logger.ErrorWriteLineToOutput($"Failed to extract Questionnaire from flat file format\nLocation: '{arguments.QuestionnairePath}'.");
                return;
            }

            Logger.WriteLineToOutput($"Uploading Questionnaire to endpoint: {arguments.FhirBaseUrl}");
            // Set a relative url before posting to the server
            if(!string.IsNullOrEmpty(questionnaire.Id))
            {
                questionnaire.Url = $"{ResourceType.Questionnaire.GetLiteral()}/{questionnaire.Id}";
            }
            
            // Initialize a FhirClient and POST or PUT Questionnaire to server.
            FhirClient fhirClient = new FhirClient(arguments.FhirBaseUrl);
            if (string.IsNullOrEmpty(questionnaire.Id))
                questionnaire = fhirClient.Create(questionnaire);
            else
                questionnaire = fhirClient.Update(questionnaire);

            Logger.WriteLineToOutput($"Successfully uploaded Questionnaire to endpoint.");
            Logger.WriteLineToOutput($"Questionnaire was assigned the Id: {questionnaire.Id}");
            Logger.WriteLineToOutput($"Questionnaire can be accessed at: {fhirClient.Endpoint.AbsoluteUri}{ResourceType.Questionnaire.GetLiteral()}/{questionnaire.Id}");
        }

        private static void UploadDefintitionsOperation(FhirToolArguments arguments)
        {
            if (string.IsNullOrEmpty(arguments.FhirBaseUrl))
            {
                Logger.ErrorWriteLineToOutput($"Operation '{FhirToolArguments.UPLOAD_OP}' requires argument '{FhirToolArguments.FHIRBASEURL_ARG}' or '{FhirToolArguments.ENVIRONMENT_ARG}'.");
                return;
            }
            if (string.IsNullOrEmpty(arguments.SourcePath))
            {
                Logger.ErrorWriteLineToOutput($"Operation '{FhirToolArguments.UPLOAD_DEFINITIONS_OP}' requires argument '{FhirToolArguments.SOURCE_ARG}'.");
                return;
            }
            if(!(Directory.Exists(arguments.SourcePath) || File.Exists(arguments.SourcePath)))
            {
                Logger.ErrorWriteLineToOutput($"Operation '{FhirToolArguments.UPLOAD_DEFINITIONS_OP}' argument '{FhirToolArguments.SOURCE_ARG}' must point to a existing file or directory.");
                return;
            }

            IEnumerable<Resource> resources = null;
            if (IOHelpers.IsDirectory(arguments.SourcePath))
            {
                resources = FhirLoader.ImportFolder(arguments.SourcePath);
            }
            else
            {
                resources = FhirLoader.ImportFile(arguments.SourcePath);
            }

            Logger.WriteLineToOutput($"Uploading resources to endpoint: '{arguments.FhirBaseUrl}'");

            FhirClient fhirClient = new FhirClient(arguments.FhirBaseUrl);
            fhirClient.OnBeforeRequest += fhirClient_BeforeRequest;
            foreach (Resource resource in resources)
            {
                Resource resource2;
                if(resource is Questionnaire && !string.IsNullOrEmpty(resource.Id))
                {
                    Questionnaire questionnaire = resource as Questionnaire;
                    questionnaire.Url = $"{_arguments.ProxyBaseUrl}Questionnaire/{resource.Id}";
                }
                if (string.IsNullOrEmpty(resource.Id))
                {
                    Logger.WriteLineToOutput($"Creating new resource of type: '{resource.TypeName}'");
                    resource2 = fhirClient.Create(resource);
                }
                else
                {
                    Logger.WriteLineToOutput($"Updating resource with Id: '{resource.Id}' of type: '{resource.TypeName}'");
                    resource2 = fhirClient.Update(resource);
                }

                Logger.WriteLineToOutput($"Resource was assigned the Id: '{resource2.Id}'");
                Logger.WriteLineToOutput($"Resource can be accessed at: '{fhirClient.Endpoint.AbsoluteUri}{ResourceType.Questionnaire.GetLiteral()}/{resource2.Id}'");
            }

            Logger.WriteLineToOutput($"Successfully uploaded resources to endpoint: {arguments.FhirBaseUrl}");
        }
                
        private static void BundleResourcesOperation(FhirToolArguments arguments)
        {
            if (string.IsNullOrEmpty(arguments.SourcePath))
            {
                Logger.ErrorWriteLineToOutput($"Operation '{FhirToolArguments.BUNDLE_OP}' requires argument '{FhirToolArguments.SOURCE_ARG}'.");
                return;
            }

            Bundle bundle = null;
            Uri uri = new Uri(arguments.SourcePath);
            if (uri.IsHttpScheme())
            {
                throw new NotImplementedException("argument '--source | -S' do not currently support a HTTP scheme.");
            }
            else
            {
                bundle = FhirLoader.ImportFolder(arguments.SourcePath).ToBundle();
            }
            string filePath = $"bundle-{Guid.NewGuid().ToString("N").Substring(0, 5)}";
            if (!string.IsNullOrEmpty(arguments.OutPath) && Directory.Exists(arguments.OutPath))
                filePath = Path.Combine(arguments.OutPath, filePath);
            string mimeType = string.IsNullOrEmpty(arguments.MimeType) ? "xml" : arguments.MimeType;
            if (mimeType == "xml")
            {
                filePath = $"{filePath}.xml";
                Logger.WriteLineToOutput($"Writing Bundle in xml format to local disk: {filePath}");
                bundle.SerializeResourceToDiskAsXml(filePath);
            }
            else if (mimeType == "json")
            {
                filePath = $"{filePath}.json";
                Logger.WriteLineToOutput($"Writing Bundle in json format to local disk: {filePath}");
                bundle.SerializeResourceToDiskAsJson(filePath);
            }
        }
        
        private static void SplitBundleOperation(FhirToolArguments arguments)
        {
            if (string.IsNullOrEmpty(arguments.SourcePath))
            {
                Logger.ErrorWriteLineToOutput($"Operation '{FhirToolArguments.SPLIT_BUNDLE_OP}' requires argument '{FhirToolArguments.SOURCE_ARG}'.");
                return;
            }
            if(!File.Exists(_arguments.SourcePath))
            {
                Logger.ErrorWriteLineToOutput($"Operation '{FhirToolArguments.SPLIT_BUNDLE_OP}' argument '{FhirToolArguments.SOURCE_ARG}' must point to an actual file.");
                return;
            }
            string mimeType = string.IsNullOrEmpty(arguments.MimeType) ? "xml" : arguments.MimeType;
            string outPath = !string.IsNullOrEmpty(arguments.OutPath) && Directory.Exists(arguments.OutPath) ? arguments.OutPath : @".\";
            IEnumerable<Resource> resources = FhirLoader.ImportFile(_arguments.SourcePath);
            foreach(Resource resource in resources)
            {
                string filePath = Path.Combine(outPath, $"{resource.ResourceType.ToString().ToLower()}-{(string.IsNullOrEmpty(resource.Id) ? Guid.NewGuid().ToString("N").ToLower() : resource.Id)}");
                if (mimeType == "xml")
                {
                    filePath = $"{filePath}.xml";
                    Logger.WriteLineToOutput($"Writing Resource in xml format to local disk: {filePath}");
                    resource.SerializeResourceToDiskAsXml(filePath);
                }
                else if (mimeType == "json")
                {
                    filePath = $"{filePath}.json";
                    Logger.WriteLineToOutput($"Writing Resource in json format to local disk: {filePath}");
                    resource.SerializeResourceToDiskAsJson(filePath);
                }
            }
        }

        private static void fhirClient_BeforeRequest(object sender, BeforeRequestEventArgs e)
        {
            if (!string.IsNullOrEmpty(_arguments.Credentials))
            {
                e.RawRequest.Headers.Add(System.Net.HttpRequestHeader.Authorization, $"Basic {_arguments.Credentials.ToBase64()}");
            }
        }

        public static T DeserializeJsonResource<T>(string path)
            where T : Base
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
            where T : Base
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
       
        private static string GetLanguageCode(string languageAndCountryCode)
        {
            string languageCode = languageAndCountryCode;
            int index = languageCode.IndexOf("-");
            if (index > 0)
            {
                languageCode = languageCode.Substring(0, index);
            }

            return languageCode;
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
                Logger.DebugWriteLineToOutput($"Questionnaire: {masterDetail.Master.Name} - {masterDetail.Master.Title}");

                Questionnaire questionnaire = new Questionnaire();

                questionnaire.Meta = new Meta
                {
                    Profile = new string[] { Constants.QuestionnaireProfileUri }
                };

                questionnaire.Id = string.IsNullOrEmpty(masterDetail.Master.Id) ? null : masterDetail.Master.Id;
                questionnaire.Url = string.IsNullOrEmpty(masterDetail.Master.Url) ? null : masterDetail.Master.Url;
                questionnaire.Version = string.IsNullOrEmpty(masterDetail.Master.Version) ? null : masterDetail.Master.Version;
                questionnaire.Name = string.IsNullOrEmpty(masterDetail.Master.Name) ? null : masterDetail.Master.Name;
                questionnaire.Title = string.IsNullOrEmpty(masterDetail.Master.Title) ? null : masterDetail.Master.Title;
                questionnaire.Status = string.IsNullOrEmpty(masterDetail.Master.Status) ? null : EnumUtility.ParseLiteral<PublicationStatus>(masterDetail.Master.Status);
                if (!string.IsNullOrEmpty(masterDetail.Master.Date))
                {
                    if (!IsValidFhirDateTime(masterDetail.Master.Date)) throw new Exception($"The date {masterDetail.Master.Date} is not conforming to the expected format: 'yyyy-MM-dd'");
                    questionnaire.DateElement = new FhirDateTime(masterDetail.Master.Date);
                }
                questionnaire.Publisher = string.IsNullOrEmpty(masterDetail.Master.Publisher) ? null : masterDetail.Master.Publisher;
                questionnaire.Description = string.IsNullOrEmpty(masterDetail.Master.Description) ? null : new Markdown(masterDetail.Master.Description);
                questionnaire.Purpose = string.IsNullOrEmpty(masterDetail.Master.Purpose) ? null : new Markdown(masterDetail.Master.Purpose);
                if (!string.IsNullOrEmpty(masterDetail.Master.ApprovalDate))
                {
                    if (!IsValidFhirDate(masterDetail.Master.ApprovalDate)) throw new Exception($"The date {masterDetail.Master.ApprovalDate} is not conforming to the expected format: 'yyyy-MM-dd'");
                    questionnaire.ApprovalDateElement = new Date(masterDetail.Master.ApprovalDate);
                }
                if (!string.IsNullOrEmpty(masterDetail.Master.ApprovalDate))
                {
                    if (!IsValidFhirDate(masterDetail.Master.LastReviewDate)) throw new Exception($"The date {masterDetail.Master.LastReviewDate} is not conforming to the expected format: 'yyyy-MM-dd'");
                    questionnaire.LastReviewDateElement = new Date(masterDetail.Master.LastReviewDate);
                }
                questionnaire.Contact = string.IsNullOrEmpty(masterDetail.Master.ContactName) ? null : new List<ContactDetail> { new ContactDetail { Name = masterDetail.Master.ContactName } };
                questionnaire.Copyright = string.IsNullOrEmpty(masterDetail.Master.Copyright) ? null : new Markdown(masterDetail.Master.Copyright);

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

                if (!string.IsNullOrEmpty(masterDetail.Master.Language))
                {
                    questionnaire.Language = masterDetail.Master.Language;
                    string displayName = CultureInfo.GetCultureInfo(GetLanguageCode(questionnaire.Language))?.NativeName.UpperCaseFirstCharacter();
                    questionnaire.Meta.Tag.Add(new Coding("urn:ietf:bcp:47", questionnaire.Language, displayName == null ? string.Empty : displayName));
                }

                if(!string.IsNullOrEmpty(masterDetail.Master.UseContext))
                {
                    questionnaire.UseContext = ParseUsageContext(masterDetail.Master.UseContext);
                }

                if (!string.IsNullOrEmpty(masterDetail.Master.Endpoint))
                {
                    questionnaire.SetExtension(Constants.EndPointUri, new ResourceReference($"{_arguments.ProxyBaseUrl}{masterDetail.Master.Endpoint}"));
                }

                if(!string.IsNullOrEmpty(masterDetail.Master.AuthenticationRequirement))
                {
                    questionnaire.SetExtension(Constants.AuthenticationRequirementUri, new Coding(Constants.AuthenticationRequirementSystem, masterDetail.Master.AuthenticationRequirement));
                }

                if (!string.IsNullOrEmpty(masterDetail.Master.AccessibilityToResponse))
                {
                    questionnaire.SetExtension(Constants.AccessibilityToResponseUri, new Coding(Constants.AccessibilityToResponseSystem, masterDetail.Master.AccessibilityToResponse));
                }

                if (!string.IsNullOrEmpty(masterDetail.Master.CanBePerformedBy))
                {
                    questionnaire.SetExtension(Constants.CanBePerformedByUri, new Coding(Constants.CanBePerformedBySystem, masterDetail.Master.CanBePerformedBy));
                }

                if (!string.IsNullOrEmpty(masterDetail.Master.Discretion))
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

                if (!string.IsNullOrEmpty(masterDetail.Master.PresentationButtons))
                {
                    questionnaire.SetExtension(Constants.PresentationButtonsUri, new Coding(Constants.PresentationButtonsSystem, masterDetail.Master.PresentationButtons));
                }
                else
                {
                    questionnaire.SetExtension(Constants.PresentationButtonsUri, new Coding(Constants.PresentationButtonsSystem, "sticky"));
                }

                IList<string> linkIds = new List<string>();
                Questionnaire.ItemComponent item = null;
                for (int i = 0; i < masterDetail.Details.Length; i++)
                {
                    QuestionnaireItem2 questionnaireItem = masterDetail.Details[i];

                    if (linkIds.IndexOf(questionnaireItem.LinkId) > 0) throw new DuplicateLinkIdException(questionnaireItem.LinkId);

                    Logger.DebugWriteLineToOutput($"Questionnaire Item: {questionnaireItem.LinkId} - {questionnaireItem.Type} - {questionnaireItem.Text}");

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
                Logger.DebugWriteLineToOutput($"Questionnaire Item: {questionnaireItem.LinkId} - {questionnaireItem.Type} - {questionnaireItem.Text}");

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
            if (!itemType.HasValue) throw new Exception($"QuestionnaireItemType at question with linkId: {item.LinkId} is not conforming to any valid literals. QuestionnaireItemType: {item.Type}");

            Questionnaire.ItemComponent itemComponent = new Questionnaire.ItemComponent
            {
                Type = itemType,
            };

            itemComponent.LinkId = string.IsNullOrEmpty(item.LinkId) ? null : item.LinkId;
            itemComponent.Prefix = string.IsNullOrEmpty(item.Prefix) ? null : item.Prefix;
            itemComponent.Text = string.IsNullOrEmpty(item.Text) ? null  : item.Text;
            itemComponent.EnableWhen = string.IsNullOrEmpty(item.EnableWhen) ? null : ParseEnableWhen(item.EnableWhen).ToList();

            if (itemType != Questionnaire.QuestionnaireItemType.Group && itemType != Questionnaire.QuestionnaireItemType.Display)
            {
                itemComponent.Required = item.Required.HasValue ? item.Required : null;
                itemComponent.ReadOnly = item.ReadOnly;
                itemComponent.Initial = string.IsNullOrEmpty(item.Initial) ? null : GetElement(itemType.Value, item.Initial);
                itemComponent.MaxLength = item.MaxLength.HasValue ? item.MaxLength : null;
            }

            if(itemType != Questionnaire.QuestionnaireItemType.Display)
            {
                itemComponent.Repeats = item.Repeats;
            }

            if (!string.IsNullOrEmpty(item.ValidationText))
                itemComponent.SetStringExtension(Constants.ValidationTextUri, item.ValidationText);

            if (!string.IsNullOrEmpty(item.Options) && item.Options.IndexOf('#') == 0)
                itemComponent.Options = new ResourceReference($"#{item.Options.Substring(1)}");
            
            if (!string.IsNullOrEmpty(item.EntryFormat))
                itemComponent.SetStringExtension(Constants.EntryFormatUri, item.EntryFormat);

            if (item.MaxValueInteger.HasValue)
                itemComponent.SetIntegerExtension(Constants.MaxValueUri, item.MaxValueInteger.Value);
            if (item.MinValueInteger.HasValue)
                itemComponent.SetIntegerExtension(Constants.MinValueUri, item.MinValueInteger.Value);

            if (item.MaxValueDate.HasValue)
                itemComponent.SetExtension(Constants.MaxValueUri, new FhirDateTime(item.MaxValueDate.Value.ToUniversalTime()));
            if (item.MinValueDate.HasValue)
                itemComponent.SetExtension(Constants.MinValueUri, new FhirDateTime(item.MinValueDate.Value.ToUniversalTime()));

            if (item.MaxValueDate.HasValue)
                itemComponent.SetExtension(Constants.MaxValueUri, new FhirDateTime(item.MaxValueDate.Value.ToUniversalTime()));
            if (item.MinValueDate.HasValue)
                itemComponent.SetExtension(Constants.MinValueUri, new FhirDateTime(item.MinValueDate.Value.ToUniversalTime()));

            if (item.MinLength.HasValue)
                itemComponent.SetIntegerExtension(Constants.MinLenghtUri, item.MinLength.Value);

            if (item.MaxDecimalPlaces.HasValue)
                itemComponent.SetIntegerExtension(Constants.MaxDecimalPlacesUri, item.MaxDecimalPlaces.Value);

            if (!string.IsNullOrEmpty(item.RepeatsText))
                itemComponent.SetStringExtension(Constants.RepeatsTextUri, item.RepeatsText);

            if (!string.IsNullOrEmpty(item.ItemControl))
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

            if (!string.IsNullOrEmpty(item.Regex))
                itemComponent.SetStringExtension(Constants.RegexUri, item.Regex);

            if (!string.IsNullOrEmpty(item.Markdown))
            {
                if (itemComponent.Text == null) throw new MissingRequirementException($"Question with linkId: {item.LinkId}. The 'Text' attribute is required when setting the 'Markdown' extension so that form fillers which do not support the 'Markdown' extension still can display informative text to the user.");
                itemComponent.TextElement.SetExtension(Constants.RenderingMarkdownUri, new Markdown(item.Markdown));
            }
            if (!string.IsNullOrEmpty(item.Unit))
            {
                Coding unitCoding = ParseCoding(item.Unit);
                itemComponent.SetExtension(Constants.QuestionnaireUnitUri, unitCoding);
            }

            if(!string.IsNullOrEmpty(item.Code))
            {
                itemComponent.Code = ParseArrayOfCoding(item.Code);
            }

            if(!string.IsNullOrEmpty(item.Option))
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
                        itemComponent.Option.Add(new Questionnaire.OptionComponent { Value = element });
                    }
                }
            }

            if(!string.IsNullOrEmpty(item.FhirPathExpression))
            {
                itemComponent.SetStringExtension(Constants.FhirPathUri, item.FhirPathExpression);
            }

            if (item.Hidden)
            {
                itemComponent.SetBoolExtension(Constants.QuestionnaireItemHidden, item.Hidden);
            }

            if(item.AttachmentMaxSize.HasValue && itemComponent.Type == Questionnaire.QuestionnaireItemType.Attachment)
            {
                itemComponent.SetExtension(Constants.QuestionnaireAttachmentMaxSize, new FhirDecimal(item.AttachmentMaxSize));
            }

            if(!string.IsNullOrWhiteSpace(item.CalculatedExpression))
            {
                itemComponent.SetStringExtension(Constants.CalculatedExpressionUri, item.CalculatedExpression);
            }

            if (!string.IsNullOrEmpty(item.GuidanceAction))
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
                Logger.DebugWriteLineToOutput($"Questionnaire: {masterDetail.Master.Name} - {masterDetail.Master.Title}");

                Questionnaire questionnaire = new Questionnaire();

                questionnaire.Meta = new Meta
                {
                    Profile = new string[] { "http://ehelse.no/fhir/StructureDefinition/sdf-Questionnaire" }
                };

                questionnaire.Id = string.IsNullOrEmpty(masterDetail.Master.Id) ? null : masterDetail.Master.Id;
                questionnaire.Url = string.IsNullOrEmpty(masterDetail.Master.Url) ? null : masterDetail.Master.Url;
                questionnaire.Version = string.IsNullOrEmpty(masterDetail.Master.Version) ? null : masterDetail.Master.Version;
                questionnaire.Name = string.IsNullOrEmpty(masterDetail.Master.Name) ? null : masterDetail.Master.Name;
                questionnaire.Title = string.IsNullOrEmpty(masterDetail.Master.Title) ? null : masterDetail.Master.Title;
                questionnaire.Status = string.IsNullOrEmpty(masterDetail.Master.Status) ? null : EnumUtility.ParseLiteral<PublicationStatus>(masterDetail.Master.Status);
                questionnaire.Date = string.IsNullOrEmpty(masterDetail.Master.Date) ? null : masterDetail.Master.Date;
                questionnaire.Publisher = string.IsNullOrEmpty(masterDetail.Master.Publisher) ? null : masterDetail.Master.Publisher;
                questionnaire.Description = string.IsNullOrEmpty(masterDetail.Master.Description) ? null : new Markdown(masterDetail.Master.Description);
                questionnaire.Purpose = string.IsNullOrEmpty(masterDetail.Master.Purpose) ? null : new Markdown(masterDetail.Master.Purpose);
                questionnaire.Contact = string.IsNullOrEmpty(masterDetail.Master.Contact) ? null : new List<ContactDetail> { new ContactDetail { Telecom = new List<ContactPoint> { new ContactPoint { System = ContactPoint.ContactPointSystem.Url, Value = masterDetail.Master.Contact } } } };

                if (!string.IsNullOrEmpty(masterDetail.Master.Language))
                {
                    questionnaire.Language = masterDetail.Master.Language;
                    // TODO: Vi trenger definere Visningsnavn for språket, eksempelvis: Norsk (bokmål), osv.
                    questionnaire.Meta.Tag.Add(new Coding("urn:ietf:bcp:47", questionnaire.Language));
                }

                IList<string> linkIds = new List<string>();
                Questionnaire.ItemComponent item = null;
                for(int i = 0; i < masterDetail.Details.Length; i++)
                {
                    QuestionnaireItem questionnaireItem = masterDetail.Details[i];

                    if (linkIds.IndexOf(questionnaireItem.LinkId) > 0) throw new DuplicateLinkIdException(questionnaireItem.LinkId);

                    Logger.DebugWriteLineToOutput($"Questionnaire Item: {questionnaireItem.LinkId} - {questionnaireItem.Type} - {questionnaireItem.Text}");

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
                Logger.DebugWriteLineToOutput($"Questionnaire Item: {questionnaireItem.LinkId} - {questionnaireItem.Type} - {questionnaireItem.Text}");

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
                    return new FhirDecimal(decimal.Parse(value, CultureInfo.InvariantCulture));
                case Questionnaire.QuestionnaireItemType.DateTime:
                    return new FhirDateTime(DateTime.Parse(value).ToUniversalTime());
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

        private static Questionnaire.ItemComponent CreateItemComponentV1(QuestionnaireItem item)
        {
            Questionnaire.QuestionnaireItemType? itemType = EnumUtility.ParseLiteral<Questionnaire.QuestionnaireItemType>(item.Type);
            if (!itemType.HasValue) throw new Exception(string.Format("QuestionnaireItemType at question with linkId: {} is not conforming to any valid literals. QuestionnaireItemType: {1}", item.LinkId, item.Type));

            Questionnaire.ItemComponent itemComponent = new Questionnaire.ItemComponent
            {
                Type = itemType,
            };

            itemComponent.LinkId = string.IsNullOrEmpty(item.LinkId) ? null : item.LinkId;
            itemComponent.Prefix = string.IsNullOrEmpty(item.Prefix) ? null : item.Prefix;
            itemComponent.Text = string.IsNullOrEmpty(item.Text) ? null : item.Text;
            itemComponent.EnableWhen = string.IsNullOrEmpty(item.EnableWhen) ? null : ParseEnableWhen(item.EnableWhen).ToList();

            if (itemType != Questionnaire.QuestionnaireItemType.Group && itemType != Questionnaire.QuestionnaireItemType.Display)
            {
                itemComponent.Required = item.Required.HasValue ? item.Required : null;
                itemComponent.ReadOnly = item.ReadOnly;
                itemComponent.Initial = string.IsNullOrEmpty(item.Initial) ? null : GetElement(itemType.Value, item.Initial);
            }

            if (itemType != Questionnaire.QuestionnaireItemType.Display)
            {
                itemComponent.Repeats = item.Repeats;
            }

            if (!string.IsNullOrEmpty(item.ValidationText))
                itemComponent.SetStringExtension(Constants.ValidationTextUri, item.ValidationText);
            if (!string.IsNullOrEmpty(item.ReferenceValue) && item.ReferenceValue.IndexOf('#') == 0)
                itemComponent.Options = new ResourceReference($"#{item.ReferenceValue.Substring(1)}");
            if (!string.IsNullOrEmpty(item.EntryFormat))
                itemComponent.SetStringExtension(Constants.EntryFormatUri, item.EntryFormat);
            if (item.MaxValue.HasValue)
                itemComponent.SetIntegerExtension(Constants.MaxValueUri, item.MaxValue.Value);
            if (item.MinValue.HasValue)
                itemComponent.SetIntegerExtension(Constants.MinValueUri, item.MinValue.Value);

            return itemComponent;
        }

        private static List<UsageContext> ParseUsageContext(string value)
        {
            List<UsageContext> usageContexts = new List<UsageContext>();

            JObject usageContextObject = JObject.Parse(value);
            JArray usageContextArray = usageContextObject["useContext"] as JArray;

            List<UsageContextElement> usageContextElements = JsonConvert.DeserializeObject<List<UsageContextElement>>(usageContextArray.ToString());
            foreach(UsageContextElement usageContextElement in usageContextElements)
            {
                UsageContext usageContext = new UsageContext();
                if (usageContextElement.Code != null)
                {
                    usageContext.Code = new Coding
                    {
                        System = string.IsNullOrEmpty(usageContextElement.Code.System) ? null : usageContextElement.Code.System,
                        Code = string.IsNullOrEmpty(usageContextElement.Code.Code) ? null : usageContextElement.Code.Code,
                        Display = string.IsNullOrEmpty(usageContextElement.Code.Display) ? null : usageContextElement.Code.Display
                    };
                }
                if(usageContextElement.ValueCodeableConcept != null)
                {
                    foreach(CodingElement codingElement in usageContextElement.ValueCodeableConcept.Coding)
                    {
                        usageContext.Value = new CodeableConcept
                        {
                            Coding = new List<Coding>
                            {
                                new Coding
                                {
                                    System = string.IsNullOrEmpty(codingElement.System) ? null : codingElement.System,
                                    Code = string.IsNullOrEmpty(codingElement.Code) ? null : codingElement.Code,
                                    Display = string.IsNullOrEmpty(codingElement.Display) ? null : codingElement.Display
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
                if(enableWhen.AnswerReference != null)
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
                foreach(JToken token in arrayOfExtension)
                {
                    extension.Add(parser.Parse<Extension>(token.ToString()));
                }
            }

            if(elementJObject.ContainsKey("valueBoolean") && FhirBoolean.IsValidValue(elementJObject["valueBoolean"].ToString()))
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
            else if(elementJObject.ContainsKey("valueReference"))
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
            foreach(JToken token in arrayOfCoding)
            {
                codings.Add((Coding) ParseElement(token.ToString()));
            }

            return codings;
        }

        private static Coding ParseCoding(string value)
        {
            JObject codingJObject = JObject.Parse(value);
            CodingElement valueCoding = codingJObject.ToObject<CodingElement>();
            if (string.IsNullOrEmpty(valueCoding.System))
                throw new RequiredAttributeException("When parsing a Coding type required attribute System does not have a value.", "System");
            if (string.IsNullOrEmpty(valueCoding.Code))
                throw new RequiredAttributeException("When parsing a Coding type required attribute Code does not have a value.", "Code");

            Coding coding = new Coding
            {
                System = valueCoding.System,
                Code = valueCoding.Code
            };
            if (!string.IsNullOrEmpty(valueCoding.Display))
                coding.Display = valueCoding.Display;

            return coding;
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
                Logger.DebugWriteLineToOutput($"ValueSet: {masterDetail.Master.Id} - {masterDetail.Master.Title}");

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
                    Logger.DebugWriteLineToOutput($"ValueSetCodeReference: {valueSetCodeReference.Code} - {valueSetCodeReference.Display}");

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