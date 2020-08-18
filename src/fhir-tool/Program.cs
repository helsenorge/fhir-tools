using FhirTool.Core;
using FhirTool.Core.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Tasks = System.Threading.Tasks;

namespace FhirTool
{
    partial class Program
    {
        //private const string ProxyBaseUrl = "https://skjemakatalog-test-fhir-apimgmt.azure-api.net/api/v1/";

        private static FhirToolArguments _arguments = null;
        private static TextWriter _out = null;
        private static ILoggerFactory _loggerFactory = null;
        private static ILogger<Program> _logger = null;

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
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Tasks.Task MainAsync(string[] args)
        {
            try
            {

                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
                Console.WriteLine($"{versionInfo.ProductName} [Version {Assembly.GetEntryAssembly().GetName().Version}]");
                Console.WriteLine($"(c) 2018 {versionInfo.CompanyName}. All rights reserved.");
                Console.WriteLine();

                _arguments = HandleFhirToolArguments.Create(args);

                _loggerFactory = LoggerFactory.Create(builder => builder
                    .AddConsole(options => options.IncludeScopes = false)
                    .SetMinimumLevel(_arguments.Verbose ? LogLevel.Debug : LogLevel.Information)
                 );

                _logger = _loggerFactory.CreateLogger<Program>();
                FhirToolArgumentsExtensions.Logger = _loggerFactory.CreateLogger(typeof(FhirToolArgumentsExtensions));



                _logger.LogDebug("Validating command line arguments.");
                if (!_arguments.Validate())
                {
                    _logger.LogError("One or more arguments did not validate. Please verify your arguments according to output.");
                    goto exit;
                }
                OperationFactory operationFactory = new OperationFactory(_arguments, _loggerFactory);
                IOperation operation = operationFactory.Create(_arguments.Operation);
                OperationResultEnum operationResult = await operation.Execute();
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

        exit:
            if (!Debugger.IsAttached)
            {
                Console.WriteLine("\nPress any key to exit. . .");
                Console.ReadKey(true);
            }
        }
    }
}