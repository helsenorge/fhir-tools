using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FhirTool.Core.FhirWrappers;

namespace FhirTool.Core.Operations
{
    internal class DownloadResourcesOperation : Operation
    {
        private readonly FhirToolArguments _arguments;
        private readonly ILogger _logger;

        public DownloadResourcesOperation(FhirToolArguments arguments, ILogger logger)
        {
            _arguments = arguments;
            _logger = logger;
        }

        public override async Task<OperationResultEnum> Execute()
        {
            ValidateArguments(_arguments);
            if (_issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)) return OperationResultEnum.Failed;

            var client = new FhirClientWrapper(_arguments.FhirBaseUrl, _logger, _arguments.FhirVersion);
            
            var baseDir = CreateBaseDirectory(new Uri(_arguments.FhirBaseUrl), client.FhirVersion);
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
            result = _arguments.KeepServerUrl && result != null ? UpdateBundleServerUrl(result) : result;
            SerializeAndStore(result, resourceTypeDir);

            while(result != null)
            {
                result = await client.ContinueAsync(result);
                result = _arguments.KeepServerUrl && result != null ? UpdateBundleServerUrl(result) : result;
                SerializeAndStore(result, resourceTypeDir);
            }
        }

        private BundleWrapper UpdateBundleServerUrl(BundleWrapper bundle)
        {
            return bundle.UpdateLinks(new Uri(_arguments.FhirBaseUrl));
        }

        private void SerializeAndStore(BundleWrapper result, DirectoryInfo baseDir)
        {
            if (result == null) return;
            var resources = result.GetResources();
            _logger.LogInformation($"  Fetched {resources.Count()} resources from server");
            foreach (var resource in resources)
            {
                string serialized = resource.Serialize();
                Store(resource.Id, serialized, baseDir);
            }
        }

        private void Store(string id, string serialized, DirectoryInfo baseDir)
        {
            var filename = SafeFilename(string.Join(".", id, "json"));
            var path = Path.Combine(baseDir.FullName, filename);
            _logger.LogInformation($"    Storing resource {id} at {MakeRelativeToCurrentDirectory(path)}");
            File.WriteAllText(path, serialized);
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

        private void ValidateArguments(FhirToolArguments arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments.FhirBaseUrl))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{FhirToolArguments.DOWNLOAD_OP}' requires argument '{FhirToolArguments.FHIRBASEURL_ARG}' or '{FhirToolArguments.ENVIRONMENT_ARG}'.",
                    Severity = IssueSeverityEnum.Error,
                });
            }
            if (arguments.Resources == null || !arguments.Resources.Any())
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{FhirToolArguments.DOWNLOAD_OP}' requires argument '{FhirToolArguments.DOWNLOAD_RESOURCES_ARG}'.",
                    Severity = IssueSeverityEnum.Error,
                });
            }
        }
    }
}