extern alias R3;

using R3::Hl7.Fhir.Model;
using R3::Hl7.Fhir.Rest;
using R3::Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Tasks = System.Threading.Tasks;
using CommandLine;
using ParserSettings = R3::Hl7.Fhir.Serialization.ParserSettings;
using FhirTool.Core.ArgumentHelpers;

namespace FhirTool.Core.Operations
{
    [Verb("transfer-data", HelpText = "transfer")]
    public class TransferDataOperationOptions
    {
        [Option("environment-source", Required = true, HelpText = "source environment")]
        public WithEnvironment SourceEnvironment { get; set; }

        [Option("environment-destination", Required = true, HelpText = "destination environment")]
        public WithEnvironment DestinationEnvironment { get; set; }

        [Option('R', "resourcetype", Required = true, HelpText = "Resource type")]
        public ResourceType ResourceType { get; set; }

        [Option("searchcount", HelpText ="search count")]
        public int SearchCount { get; set; }
    }

    public class TransferDataOperation : Operation
    {
        private TransferDataOperationOptions _arguments;
        private ILoggerFactory _loggerFactory;
        private ILogger<TransferDataOperation> _logger;

        public TransferDataOperation(TransferDataOperationOptions arguments, ILoggerFactory loggerFactory)
        {
            _arguments = arguments;
            _loggerFactory = loggerFactory;

            _logger = loggerFactory.CreateLogger<TransferDataOperation>();

            Validate(arguments);
        }

        private void Validate(TransferDataOperationOptions arguments)
        {
            arguments.SourceEnvironment.Validate(nameof(arguments.SourceEnvironment));
            arguments.DestinationEnvironment.Validate(nameof(arguments.DestinationEnvironment));
        }

        public override async Tasks.Task<OperationResultEnum> Execute()
        {
            FhirJsonSerializer serializer = new FhirJsonSerializer();
            FhirClient sourceClient = new FhirClient(_arguments.SourceEnvironment.FhirBaseUrl)
            {
                ParserSettings = new ParserSettings
                {
                    PermissiveParsing = false,
                }
            };
            HttpClient destClient = new HttpClient()
            {
                BaseAddress = new Uri(_arguments.DestinationEnvironment.FhirBaseUrl),
            };

            string relativeUrl = $"{_arguments.ResourceType.GetLiteral()}";
            if (_arguments.SearchCount > 0)
                relativeUrl += $"?_count={_arguments.SearchCount}";

            Bundle sourceBundle = await sourceClient.GetAsync(relativeUrl) as Bundle;
            foreach (Bundle.EntryComponent entry in sourceBundle.Entry)
            {
                Resource resource = entry.Resource;
                string resourceType = resource.ResourceType.GetLiteral();

                if (resource is Questionnaire questionnaire)
                {
                    // This part gets rid of some legacy
                    // TODO: Remove when we have gotten rid of the legacy
                    if (string.IsNullOrWhiteSpace(questionnaire.ApprovalDate)) questionnaire.ApprovalDate = null;
                    if (string.IsNullOrWhiteSpace(questionnaire.LastReviewDate)) questionnaire.LastReviewDate = null;
                    if (questionnaire.Copyright != null && string.IsNullOrWhiteSpace(questionnaire.Copyright.Value)) questionnaire.Copyright = null;

                    // Update known properties and extensions with urls that points to the old source instance.
                    // TODO: The lines referring FhirBaseUrl is legacy and can be removed in a future version.
                    questionnaire.Url = questionnaire.Url.Replace(_arguments.SourceEnvironment.ProxyBaseUrl, string.Empty);
                    questionnaire.Url = questionnaire.Url.Replace(_arguments.SourceEnvironment.FhirBaseUrl, string.Empty);

                    IEnumerable<Extension> extensions = questionnaire.GetExtensions(Constants.EndPointUri);
                    foreach (Extension extension in extensions)
                    {
                        if (extension.Value is ResourceReference v)
                        {
                            v.Reference = v.Reference.Replace(_arguments.SourceEnvironment.ProxyBaseUrl, string.Empty);
                            v.Reference = v.Reference.Replace(_arguments.SourceEnvironment.FhirBaseUrl, string.Empty);
                        }
                    }

                    extensions = questionnaire.GetExtensions(Constants.OptionReferenceUri);
                    foreach (Extension extension in extensions)
                    {
                        if (extension.Value is ResourceReference v)
                        {
                            v.Reference = v.Reference.Replace(_arguments.SourceEnvironment.ProxyBaseUrl, string.Empty);
                            v.Reference = v.Reference.Replace(_arguments.SourceEnvironment.FhirBaseUrl, string.Empty);
                        }
                    }
                }

                _logger.LogInformation($"Preparing to write resource of type '{resourceType}' to '{_arguments.DestinationEnvironment.FhirBaseUrl}'");
                HttpContent content = new StringContent(serializer.SerializeToString(resource));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/fhir+json");
                HttpResponseMessage response;
                if (string.IsNullOrWhiteSpace(resource.Id))
                    response = await destClient.PostAsync($"{resource.ResourceType.GetLiteral()}", content);
                else
                    response = await destClient.PutAsync($"{resource.ResourceType.GetLiteral()}/{resource.Id}", content);

                _logger.LogInformation($"{response.StatusCode} - {response.RequestMessage.Method} {response.RequestMessage.RequestUri}");
            }

            return _issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)
            ? OperationResultEnum.Failed
            : OperationResultEnum.Succeeded;
        }
    }
}