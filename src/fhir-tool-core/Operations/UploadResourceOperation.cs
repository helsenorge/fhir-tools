extern alias R3;

using R3::Hl7.Fhir.Model;
using R3::Hl7.Fhir.Rest;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using System.Linq;
using Tasks = System.Threading.Tasks;
using CommandLine;
using FhirTool.Core.ArgumentHelpers;
using System;

namespace FhirTool.Core.Operations
{
    [Verb("upload", HelpText = "uploads a resource")]
    public class UploadResourceOperationOptions
    {
        [Option('u', "fhir-base-url", Group = "url", Required = true, HelpText = "fhir server url")]
        public WithFhirBaseUrl FhirBaseUrl { get; set; } = new WithFhirBaseUrl();

        [Option('e', "environment", Group = "url", Required = true, HelpText = "fhir server from environment")]
        public WithEnvironment Environment { get; set; }

        [Option('c', "credentials", HelpText = "credentials")]
        public string Credentials { get; set; }

        [Option('r', "resolve-url", HelpText = "try to resolve url")]
        public bool ResolveUrl { get; set; }

        [Option('q', "questionnaire", Required = true, HelpText = "path to questionnaire")]
        public string QuestionnairePath { get; set; }

        [Option('f', "format", HelpText = "mime type")]
        public string MimeType { get; set; }
    }

    public class UploadResourceOperation : Operation
    {
        private readonly UploadResourceOperationOptions _arguments;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<UploadResourceOperation> _logger;

        public UploadResourceOperation(UploadResourceOperationOptions arguments, ILoggerFactory loggerFactory)
        {
            _arguments = arguments;
            _loggerFactory = loggerFactory;

            _logger = loggerFactory.CreateLogger<UploadResourceOperation>();

            arguments.FhirBaseUrl.Uri ??= arguments.Environment?.FhirBaseUrl;

            Validate(arguments);
        }

        public override async Tasks.Task<OperationResultEnum> Execute()
        {
            _logger.LogInformation($"Deserializing Fhir resource at '{_arguments.QuestionnairePath}'.");
            _logger.LogInformation($"Expecting format: '{_arguments.MimeType}'.");
            Questionnaire questionnaire = await SerializationUtility.DeserializeResource<Questionnaire>(_arguments.QuestionnairePath);
            if (questionnaire == null)
            {
                _issues.Add(new Issue
                {
                    Details = $"Failed to deserialize Questionnaire from file\nLocation: '{_arguments.QuestionnairePath}'.",
                    Severity = IssueSeverityEnum.Error,
                });
                return OperationResultEnum.Failed;
            }

            _logger.LogInformation($"Uploading Questionnaire to endpoint: '{_arguments.FhirBaseUrl.Uri}'");
            // Set a relative url before posting to the server
            if (!string.IsNullOrWhiteSpace(questionnaire.Id))
            {
                questionnaire.Url = $"{ResourceType.Questionnaire.GetLiteral()}/{questionnaire.Id}";
            }

            // Initialize a FhirClient and POST or PUT Questionnaire to server.
            FhirClient fhirClient = new FhirClient(_arguments.FhirBaseUrl.Uri);
            if (string.IsNullOrWhiteSpace(questionnaire.Id))
                questionnaire = await fhirClient.CreateAsync(questionnaire);
            else
                questionnaire = await fhirClient.UpdateAsync(questionnaire);

            _logger.LogInformation($"Successfully uploaded Questionnaire to endpoint: '{_arguments.FhirBaseUrl.Uri}'.");
            _logger.LogInformation($"Questionnaire was assigned the Id: '{questionnaire.Id}'");
            _logger.LogInformation($"Questionnaire can be accessed at: '{fhirClient.Endpoint.AbsoluteUri}{ResourceType.Questionnaire.GetLiteral()}/{questionnaire.Id}'");

            return _issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)
                ? OperationResultEnum.Failed
                : OperationResultEnum.Succeeded;
        }

        private void Validate(UploadResourceOperationOptions arguments)
        {
            arguments.Environment?.Validate(nameof(arguments.Environment));
            arguments.FhirBaseUrl?.Validate(nameof(arguments.FhirBaseUrl), arguments.ResolveUrl, arguments.Credentials);
        }

    }
}