extern alias R3;

using R3::Hl7.Fhir.Model;
using R3::Hl7.Fhir.Rest;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tasks = System.Threading.Tasks;
using CommandLine;
using System;
using FhirTool.Core.ArgumentHelpers;

namespace FhirTool.Core.Operations
{
    [Verb("upload-definitions", HelpText = "upload definitions")]
    public class UploadDefinitionOperationOptions
    {
        [Option('u', "fhir-base-url", Group = "url", Required = true, HelpText = "fhir server base url")]
        public WithFhirBaseUrl FhirBaseUrl { get; set; } = new WithFhirBaseUrl();

        [Option('u', "proxy-base-url", HelpText = "proxy server base url")]
        public WithFhirBaseUrl ProxyBaseUrl { get; set; } = new WithFhirBaseUrl();

        [Option('e', "environment", Group = "url", Required = true, HelpText = "fhir server from environment")]
        public WithEnvironment Environment { get; set; }

        [Option('r', "resolve-url", HelpText = "resolve url")]
        public bool ResolveUrl { get; set; }

        [Option('c', "credentials", HelpText = "credentials")]
        public string Credentials { get; set; }
    }

    public class UploadDefinitionsOperation : Operation
    {
        private readonly UploadDefinitionOperationOptions _arguments;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<UploadDefinitionsOperation> _logger;

        public UploadDefinitionsOperation(UploadDefinitionOperationOptions arguments, ILoggerFactory loggerFactory)
        {
            _arguments = arguments;
            _loggerFactory = loggerFactory;

            _logger = loggerFactory.CreateLogger<UploadDefinitionsOperation>();

            arguments.FhirBaseUrl.Uri ??= arguments.Environment?.FhirBaseUrl;
            arguments.ProxyBaseUrl.Uri ??= arguments.Environment?.ProxyBaseUrl;

            Validate(arguments);
        }

        public override async Tasks.Task<OperationResultEnum> Execute()
        {
            IEnumerable<Resource> resources = null;
            _logger.LogInformation($"Uploading resources to endpoint: '{_arguments.FhirBaseUrl}'");

            FhirClient fhirClient = new FhirClient(_arguments.FhirBaseUrl.Uri);
            fhirClient.OnBeforeRequest += fhirClient_BeforeRequest;
            foreach (Resource resource in resources)
            {
                Resource resource2;
                if (resource is Questionnaire && !string.IsNullOrWhiteSpace(resource.Id))
                {
                    Questionnaire questionnaire = resource as Questionnaire;
                    questionnaire.Url = $"{_arguments.ProxyBaseUrl}Questionnaire/{resource.Id}";
                }
                if (string.IsNullOrWhiteSpace(resource.Id))
                {
                    _logger.LogInformation($"Creating new resource of type: '{resource.TypeName}'");
                    resource2 = fhirClient.Create(resource);
                }
                else
                {
                    _logger.LogInformation($"Updating resource with Id: '{resource.Id}' of type: '{resource.TypeName}'");
                    resource2 = fhirClient.Update(resource);
                }

                _logger.LogInformation($"Resource was assigned the Id: '{resource2.Id}'");
                _logger.LogInformation($"Resource can be accessed at: '{fhirClient.Endpoint.AbsoluteUri}{ResourceType.Questionnaire.GetLiteral()}/{resource2.Id}'");
            }

            _logger.LogInformation($"Successfully uploaded resources to endpoint: {_arguments.FhirBaseUrl}");

            return await Tasks.Task.FromResult(_issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)
                ? OperationResultEnum.Failed
                : OperationResultEnum.Succeeded);
        }

        private void fhirClient_BeforeRequest(object sender, BeforeRequestEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_arguments.Credentials))
            {
                e.RawRequest.Headers.Add(System.Net.HttpRequestHeader.Authorization, $"Basic {_arguments.Credentials.ToBase64()}");
            }
        }

        private void Validate(UploadDefinitionOperationOptions arguments)
        {
            arguments.Environment?.Validate(nameof(arguments.Environment));
            arguments.FhirBaseUrl?.Validate(nameof(arguments.FhirBaseUrl), arguments.ResolveUrl, arguments.Credentials);
        }
    }
}
