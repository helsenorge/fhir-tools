extern alias R4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using FhirTool.Core.ArgumentHelpers;
using FhirTool.Core.FhirWrappers;
using Hl7.Fhir.Utility;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using R4::Hl7.Fhir.Model;

namespace FhirTool.Core.Operations
{
    [Verb("delete", HelpText = "Delete a resource")]
    public class DeleteResourceOperationOptions
    {
        [Option('u', "fhir-base-url", Group = "url", Required = true, HelpText = "fhir server url")]
        public WithFhirBaseUrl FhirBaseUrl { get; set; } = new WithFhirBaseUrl();

        [Option('e', "environment", Group = "url", Required = true, HelpText = "fhir server from environment")]
        public WithEnvironment Environment { get; set; }

        [Option('r', "resource", Required = true, HelpText = "id of resource")]
        public string ResourceId { get; set; }

        [Option('t', "resource-type", Required = true, HelpText = "name of resource type")]
        public ResourceType ResourceType { get; set; }

        [Option('a', "authorization-url", HelpText = "authorization url when using argument '-u' or '--fhir-base-url'")]
        public string AuthorizationUrl { get; set; }

        [Option('c', "credentials", HelpText = "credentials")]
        public string Credentials { get; set; }

        [Option("fhir-version", Required = false, HelpText = "which fhir version to assume")]
        public FhirVersion FhirVersion { get; set; }
    }

    public class DeleteResourceOperation : Operation
    {
        private readonly DeleteResourceOperationOptions _arguments;
        private readonly ILogger<UploadResourceOperation> _logger;

        public DeleteResourceOperation(DeleteResourceOperationOptions arguments, ILoggerFactory loggerFactory)
        {
            _arguments = arguments ?? throw new ArgumentNullException(nameof(loggerFactory));
            if (loggerFactory is null)
                throw new ArgumentNullException(nameof(loggerFactory));

            _logger = loggerFactory.CreateLogger<UploadResourceOperation>();
        }

        public override async Task<OperationResultEnum> Execute()
        {
            Validate(_arguments);
            
            TokenResponse tokenResponse = null;
            if (!string.IsNullOrEmpty(_arguments.Credentials))
            {
                tokenResponse = await GetToken(_arguments.Environment?.AuthorizationUrl, _arguments.Credentials, _arguments.AuthorizationUrl);
            }
            
            var endpoint = _arguments.FhirBaseUrl?.Uri;
            if (string.IsNullOrEmpty(endpoint))
            {
                endpoint = tokenResponse == null ? _arguments.Environment.FhirBaseUrl : _arguments.Environment.ProxyBaseUrl;
            }

            var resourceType = _arguments.ResourceType.GetLiteral();

            _logger.LogInformation($"Starting delete operation of FHIR {resourceType} with id: '{_arguments.ResourceId}'.");
            
            var client = new FhirClientWrapper(endpoint, _logger, _arguments.FhirVersion, tokenResponse?.AccessToken);

            try
            {
                await client.DeleteAsync(resourceType, _arguments.ResourceId);
                _logger.LogInformation($"Successfully deleted {resourceType} with id: '{_arguments.ResourceId}' at endpoint: '{endpoint}'.");
            }
            catch (Exception e)
            {
                _issues.Add(new Issue
                {
                    Details = e.Message,
                    Severity = IssueSeverityEnum.Error,
                });
            }

            return _issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)
                ? OperationResultEnum.Failed
                : OperationResultEnum.Succeeded;
        }

        private void Validate(DeleteResourceOperationOptions arguments)
        {
            arguments.Environment?.Validate(nameof(arguments.Environment));
            ValidateResourceType(arguments.ResourceType.GetLiteral());
        }
    }
}
