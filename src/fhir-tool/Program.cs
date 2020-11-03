using CommandLine;
using FhirTool.Core.ArgumentHelpers;
using FhirTool.Core.Configuration;
using FhirTool.Core.Operations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FhirTool
{
    partial class Program
    {
        //private const string ProxyBaseUrl = "https://skjemakatalog-test-fhir-apimgmt.azure-api.net/api/v1/";

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
        // fhir-tool.exe convert --source F:\patient-observations.xml --convert-from r3 --convert-to r4

        // Unsure if we should handle kith and messaging in this tool
        // fhir-tool.exe generate-kith --questionnaire Questionnaire-Helfo_E121_NB-no.xml --fhir-base-url https://skjemakatalog-dev-fhir-api.azurewebsites.net/ --resolve-url
        // fhir-tool.exe sendasync --questionnaire Questionnaire-Helfo_E121_NB-no.xml --fhir-base-url https://skjemakatalog-dev-fhir-api.azurewebsites.net/ --resolve-url

        // fhir-tool.exe transfer-data --environment-source test --environment-destination qa --resourcetype Questionnaire --searchcount 1000

        //fhir-tool.exe generate-binary --i testid6 --c application/pdf --s  DocumentReference/testid6 --f F:\test.pdf --m 2 --fhir-version 4  --p F:\testjson

        //generate-documentreference --i jalla --c application/pdf --p F:\testjson --m 2 --fhir-version 4
        static async Task Main(string[] args)
        {
            var configurationRoot = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", false)
                .Build();

            var environmentSettings = new List<EnvironmentSettings>();
            configurationRoot.GetSection("EnvironmentSettings").Bind(environmentSettings);
            DefinedEnvironments.Environments = environmentSettings;

            using (var loggerFactory = LoggerFactory.Create(builder => builder
                 .AddConsole(options => options.IncludeScopes = false)
                 .SetMinimumLevel(LogLevel.Information)
             ))
            {
                var logger = loggerFactory.CreateLogger<Program>();

                try
                {
                    var result = await Parser.Default
                        .ParseArguments<DownloadResourcesOperationOptions,
                                      GenerateQuestionnaireOperationOptions,
                                      UploadResourceOperationOptions,
                                      UploadDefinitionsOperation,
                                      TransferDataOperationOptions,
                                      VerifyValidationItemsOptions,
                                      ConvertOperationOptions,
                                      UploadTransactionOperationOptions,
                                      ValidateMongoDumpOperationOptions,
                                      TransformOperationOptions,
                                      GenerateBinaryOperationOptions,
                                      GenerateDocumentReferenceOptions>(args)
                      .MapResult(
                          (DownloadResourcesOperationOptions opts) => new DownloadResourcesOperation(opts, loggerFactory).Execute(),
                          (GenerateQuestionnaireOperationOptions opts) => new GenerateQuestionnaireOperation(opts, loggerFactory).Execute(),
                          (UploadResourceOperationOptions opts) => new UploadResourceOperation(opts, loggerFactory).Execute(),
                          (UploadDefinitionOperationOptions opts) => new UploadDefinitionsOperation(opts, loggerFactory).Execute(),
                          (TransferDataOperationOptions opts) => new TransferDataOperation(opts, loggerFactory).Execute(),
                          (VerifyValidationItemsOptions opts) => new VerifyValidationItems(opts, loggerFactory).Execute(),
                          (ConvertOperationOptions opts) => new ConvertOperation(opts, loggerFactory).Execute(),
                          (UploadTransactionOperationOptions opts) => new UploadTransactionOperation(opts, loggerFactory).Execute(),
                          (ValidateMongoDumpOperationOptions opts) => new ValidateMongoDumpOperation(opts, loggerFactory).Execute(),
                          (TransformOperationOptions opts) => new TransformOperation(opts, loggerFactory).Execute(),
                          (GenerateBinaryOperationOptions opts) => new GenerateBinaryOperation(opts, loggerFactory).Execute(),
                          (GenerateDocumentReferenceOptions opts) => new GenerateDocumentReferenceOption(opts, loggerFactory).Execute(),
                          errs => Task.FromResult(OperationResultEnum.Failed));
                }
                catch (SemanticArgumentException e)
                {
                    Console.Error.WriteLine($"Parameter {e.Parameter}: {e.Message}");
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                    throw;
                }
            }

            if (Debugger.IsAttached)
            {
                Console.WriteLine("\nPress any key to exit. . .");
                Console.ReadKey(true);
            }
        }
    }
}