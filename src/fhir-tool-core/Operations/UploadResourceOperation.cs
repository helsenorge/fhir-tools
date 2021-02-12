/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using System.Linq;
using Tasks = System.Threading.Tasks;
using CommandLine;
using FhirTool.Core.ArgumentHelpers;
using FhirTool.Core.FhirWrappers;
using System.IO;
using IdentityModel.Client;
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

        [Option('a', "authorization-url", HelpText = "authorization url when using argument '-u' or '--fhir-base-url'")]
        public string AuthorizationUrl { get; set; }

        [Option('c', "credentials", HelpText = "credentials")]
        public string Credentials { get; set; }

        [Option('q', "questionnaire", Required = true, HelpText = "path to questionnaire")]
        public string QuestionnairePath { get; set; }

        [Option('f', "format", MetaValue = "xml/json", HelpText = "json or xml")]
        public FhirMimeType? MimeType { get; set; }

        [Option("fhir-version", Required = false, HelpText = "which fhir version to assume")]
        public FhirVersion FhirVersion { get; set; }
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
        }

        public override async Tasks.Task<OperationResultEnum> Execute()
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

            _logger.LogInformation($"Deserializing Fhir resource at '{_arguments.QuestionnairePath}'.");
            _logger.LogInformation($"Expecting format: '{_arguments.MimeType}'.");

            var serializer = new SerializationWrapper(_arguments.FhirVersion);

            var content = await File.ReadAllTextAsync(_arguments.QuestionnairePath);
            var resource = serializer.Parse(content, _arguments.MimeType);
            if (resource == null)
            {
                _issues.Add(new Issue
                {
                    Details = $"Failed to deserialize Questionnaire from file\nLocation: '{_arguments.QuestionnairePath}'.",
                    Severity = IssueSeverityEnum.Error,
                });
                return OperationResultEnum.Failed;
            }

            _logger.LogInformation($"Uploading {resource.ResourceType} to endpoint: '{endpoint}'");
            // Set a relative url before posting to the server
            SetQuestionnaireUrl(resource);

            var client = new FhirClientWrapper(endpoint, _logger, _arguments.FhirVersion, tokenResponse?.AccessToken);
            var response = await UploadResource(resource, client);

            var resourceType = resource.ResourceType.GetLiteral();
            _logger.LogInformation($"Successfully uploaded {resourceType} to endpoint: '{endpoint}'.");
            if (response != null)
            {
                _logger.LogInformation($"Resource was assigned the Id: '{resource.Id}'");
                _logger.LogInformation($"Resource can be accessed at: '{resource.ResourceType.GetLiteral()}/{resource.Id}'");
            }

            return _issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)
                ? OperationResultEnum.Failed
                : OperationResultEnum.Succeeded;
        }

        private async Tasks.Task<ResourceWrapper> UploadResource(ResourceWrapper resource, FhirClientWrapper client)
        {
            if (string.IsNullOrWhiteSpace(resource.Id))
                return await client.CreateAsync(resource);
            else
                return await client.UpdateAsync(resource);
        }

        private void SetQuestionnaireUrl(ResourceWrapper resource)
        {
            if (resource.ResourceType == ResourceTypeWrapper.Questionnaire && !string.IsNullOrWhiteSpace(resource.Id))
            {
                resource.SetProperty("Url", $"Questionnaire/{resource.Id}");
            }
        }

        private void Validate(UploadResourceOperationOptions arguments)
        {
            arguments.Environment?.Validate(nameof(arguments.Environment));
        }
    }
}