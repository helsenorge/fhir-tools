/*
 * Copyright (c) 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 *
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

extern alias R4;
using System;
using System.Collections.Generic;
using System.IO;
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
    [Verb("get", HelpText = "Gets a resource")]
    public class GetResourceOperationOptions
    {
        [Option('u', "fhir-base-url", Group = "url", Required = true, HelpText = "fhir server url")]
        public WithFhirBaseUrl FhirBaseUrl { get; set; } = new();

        [Option('e', "environment", Group = "url", Required = true, HelpText = "fhir server from environment")]
        public WithEnvironment Environment { get; set; }

        [Option('r', "resource", Required = true, HelpText = "id of resource")]
        public string ResourceId { get; set; }

        [Option('t', "resource-type", Required = true, HelpText = "name of resource type")]
        public ResourceType ResourceType { get; set; }

        [Option('v', "resource-version", Required = false, HelpText = "version of resource")]
        public string ResourceVersion { get; set; }

        [Option('a', "authorization-url", HelpText = "authorization url when using argument '-u' or '--fhir-base-url'")]
        public string AuthorizationUrl { get; set; }

        [Option('c', "credentials", HelpText = "credentials")]
        public string Credentials { get; set; }

        [Option('f', "format", MetaValue = "xml/json", HelpText = "json or xml")]
        public FhirMimeType? MimeType { get; set; }

        [Option("fhir-version", Required = false, HelpText = "which fhir version to assume")]
        public FhirVersion? FhirVersion { get; set; }
    }


    public class GetResourceOperation : Operation
    {
        private readonly GetResourceOperationOptions _arguments;
        private readonly ILogger<GetResourceOperation> _logger;

        public GetResourceOperation(GetResourceOperationOptions arguments, ILoggerFactory loggerFactory)
        {
            _arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            if (loggerFactory is null) throw new ArgumentNullException(nameof(loggerFactory));

            _logger = loggerFactory.CreateLogger<GetResourceOperation>();
        }

        public override async Task<OperationResultEnum> Execute()
        {
            Validate(_arguments);

            TokenResponse tokenResponse = null;
            if (!string.IsNullOrEmpty(_arguments.Credentials))
                tokenResponse = await GetToken(_arguments.Environment?.AuthorizationUrl, _arguments.Credentials, _arguments.AuthorizationUrl);

            var endpoint = _arguments.FhirBaseUrl?.Uri;
            if (string.IsNullOrEmpty(endpoint))
                endpoint = tokenResponse == null ? _arguments.Environment.FhirBaseUrl : _arguments.Environment.ProxyBaseUrl;

            var resourceType = _arguments.ResourceType.GetLiteral();

            _logger.LogInformation($"Starting get operation of FHIR {resourceType} with id: '{_arguments.ResourceId}'");

            var client = new FhirClientWrapper(endpoint, _logger, _arguments.FhirVersion, tokenResponse.AccessToken);
            var resource = await client.GetAsync(resourceType, _arguments.ResourceId, _arguments.ResourceVersion);

            var statusCode = (int)client.LastStatusCode;
            _logger.LogInformation($"Get operation of FHIR {resourceType} with id: '{_arguments.ResourceId}' was successful. Status code: '{statusCode}");

            var serializer = new SerializationWrapper(_arguments.FhirVersion ?? FhirVersion.R4);
            var serializeResource = serializer.Serialize(resource, _arguments.MimeType ?? FhirMimeType.Json);

            var filename = GetAvailableFilename($"{resourceType}-{_arguments.ResourceId}.{(_arguments.MimeType ?? FhirMimeType.Json).GetLiteral()}");

            await File.WriteAllTextAsync(filename, serializeResource);

            _logger.LogInformation($"Resource was written to file: '{filename}'");

            return _issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)
                ? OperationResultEnum.Failed
                : OperationResultEnum.Succeeded;
        }

        private string GetAvailableFilename(string filenamePrototype)
        {
            if (!File.Exists(filenamePrototype))
                return filenamePrototype;

            var filename = Path.GetFileNameWithoutExtension(filenamePrototype);
            var extension = Path.GetExtension(filenamePrototype);
            var counter = 1;
            while (File.Exists($"{filename}-({counter}){extension}"))
                counter++;

            return $"{filename}-({counter}){extension}";
        }

        private void Validate(GetResourceOperationOptions arguments)
        {
            _arguments.Environment?.Validate(nameof(arguments.Environment));
            ValidateResourceType(arguments.ResourceType.GetLiteral());
        }

        private void ValidateResourceType(string resourceType)
        {
            var validResourceTypes = new List<string>
            {
                "Endpoint",
                "Questionnaire"
            };
            if (!validResourceTypes.Contains(resourceType))
            {
                var known = string.Join(", ", validResourceTypes.AsEnumerable());
                throw new SemanticArgumentException($"Resource Type {resourceType} is not a known Resource Type. Known Resource Types are {known}", "ResourceType");
            }
        }
    }
}
