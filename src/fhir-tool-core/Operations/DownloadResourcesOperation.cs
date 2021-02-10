﻿/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FhirTool.Core.FhirWrappers;
using CommandLine;
using System.Collections.Generic;
using FhirTool.Core.ArgumentHelpers;
using IdentityModel.Client;

namespace FhirTool.Core.Operations
{
    [Verb("download", HelpText = "Downloads resources from fhir server")]
    public class DownloadResourcesOperationOptions
    {
        [Option('u', "fhir-base-url", Group = "url", Required = true, HelpText = "fhir server url")]
        public WithFhirBaseUrl FhirBaseUrl { get; set; } = new WithFhirBaseUrl();

        [Option('e', "environment", Group = "url", Required = true, HelpText = "fhir server from environment")]
        public WithEnvironment Environment { get; set; }

        [Option('a', "authorization-url", HelpText = "authorization url when using argument '-u' or '--fhir-base-url'")]
        public string AuthorizationUrl { get; set; }

        [Option('c', "credentials", HelpText = "credentials")]
        public string Credentials { get; set; }

        [Option("resources", Required = true, HelpText = "comma separated list of Fhir resource type names to download", Separator = ',')]
        public IEnumerable<string> Resources { get; set; }

        [Option("fhir-version", Required = false, HelpText = "which fhir version to assume")]
        public FhirVersion FhirVersion { get; set; }

        [Option("keep-server-url", Required = false, HelpText = "...")]
        public bool KeepServerUrl { get; set; }

        [Option('f', "format", Default = FhirMimeType.Json, MetaValue = "xml/json", HelpText = "json or xml")]
        public FhirMimeType MimeType { get; set; }
    }

    public class DownloadResourcesOperation : Operation
    {
        private readonly DownloadResourcesOperationOptions _arguments;
        private readonly ILogger<DownloadResourcesOperation> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public DownloadResourcesOperation(DownloadResourcesOperationOptions arguments, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _arguments = arguments;
            _logger = loggerFactory.CreateLogger<DownloadResourcesOperation>();
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

            var client = new FhirClientWrapper(endpoint, _logger, _arguments.FhirVersion, tokenResponse?.AccessToken);
            
            var baseDir = CreateBaseDirectory(new Uri(endpoint), client.FhirVersion);
            _logger.LogInformation($"Created local storage at {MakeRelativeToCurrentDirectory(baseDir.FullName)}");
            await DownloadAndStore(client, baseDir);

            return _issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)
                ? OperationResultEnum.Failed
                : OperationResultEnum.Succeeded;
        }

        private DirectoryInfo CreateBaseDirectory(Uri uri, FhirVersion fhirVersion)
        {
            var path = Path.Combine(SafeDirectoryName(uri.Host), SafeFilename(fhirVersion.GetFhirVersionAsString()));
            return Directory.CreateDirectory(path); 
        }

        private async Task DownloadAndStore(FhirClientWrapper client, DirectoryInfo baseDir)
        {
            foreach (var resourceType in _arguments.Resources)
            {
                _logger.LogInformation($"Starting to download resources of type {resourceType}");
                await DownloadAndStoreResource(resourceType, client, baseDir);
                _logger.LogInformation($"Done downloading resources of type {resourceType}");
            }
        }

        private async Task DownloadAndStoreResource(string resourceType, FhirClientWrapper client, DirectoryInfo baseDir)
        {
            var resourceTypeDir = Directory.CreateDirectory(SafeDirectoryName(Path.Combine(baseDir.FullName, resourceType)));
            var result = await client.SearchAsync(resourceType);
            result = _arguments.KeepServerUrl && result != null ? UpdateBundleServerUrl(result, client.Endpoint) : result;
            await SerializeAndStore(result, resourceTypeDir);

            while(result != null)
            {
                result = await client.ContinueAsync(result);
                result = _arguments.KeepServerUrl && result != null ? UpdateBundleServerUrl(result, client.Endpoint) : result;
                await SerializeAndStore(result, resourceTypeDir);
            }
        }

        private BundleWrapper UpdateBundleServerUrl(BundleWrapper bundle, string endpoint)
        {
            return bundle.UpdateLinks(new Uri(endpoint));
        }

        private async Task SerializeAndStore(BundleWrapper result, DirectoryInfo baseDir)
        {
            if (result == null) return;

            var serializer = new SerializationWrapper(result.FhirVersion);
            var resources = result.GetResources();
            _logger.LogInformation($"  Fetched {resources.Count()} resources from server");
            foreach (var resource in resources)
            {
                string serialized = serializer.Serialize(resource, _arguments.MimeType);
                await Store(resource.Id, serialized, baseDir);
            }
        }

        private async Task Store(string id, string serialized, DirectoryInfo baseDir)
        {
            var filename = SafeFilename(string.Join(".", id, _arguments.MimeType.ToString().ToLower()));
            var path = Path.Combine(baseDir.FullName, filename);
            _logger.LogInformation($"    Storing resource {id} at {MakeRelativeToCurrentDirectory(path)}");
            await File.WriteAllTextAsync(path, serialized);
        }

        private string MakeRelativeToCurrentDirectory(string absolutePath)
        {
            var absolute = new Uri(absolutePath);
            var current = new Uri(Environment.CurrentDirectory);
            return current.MakeRelativeUri(absolute).ToString();
        }

        private string SafeDirectoryName(string directoryName)
        {
            foreach (var c in Path.GetInvalidPathChars())
            {
                directoryName = directoryName.Replace(c, '_');
            }

            return directoryName;
        }

        private string SafeFilename(string fileName)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }

            return fileName;
        }

        private void Validate(DownloadResourcesOperationOptions arguments)
        {
            arguments.Environment?.Validate(nameof(arguments.Environment));
        }
    }
}