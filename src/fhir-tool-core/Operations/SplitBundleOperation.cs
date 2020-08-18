using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using Tasks = System.Threading.Tasks;

namespace FhirTool.Core.Operations
{
    internal class SplitBundleOperation : Operation
    {
        private FhirToolArguments _arguments;
        private ILogger<SplitBundleOperation> _logger;

        public SplitBundleOperation(FhirToolArguments arguments, ILogger<SplitBundleOperation> logger)
        {
            _arguments = arguments;
            _logger = logger;
        }

        public override async Tasks.Task<OperationResultEnum> Execute()
        {
            ValidateArguments(_arguments);
            if (_issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)) return OperationResultEnum.Failed;

            var outPath = !string.IsNullOrWhiteSpace(_arguments.OutPath) && Directory.Exists(_arguments.OutPath) ? _arguments.OutPath : @".\";
            var resources = SerializationUtility.ImportFile(_arguments.SourcePath);
            foreach (Resource resource in resources)
            {
                var path = Path.Combine(outPath, $"{resource.ResourceType.ToString().ToLower()}-{(string.IsNullOrWhiteSpace(resource.Id) ? Guid.NewGuid().ToString("N").Substring(0, 5) : resource.Id)}.{_arguments.MimeType}");
                _logger.LogInformation($"Writing Resource in '{_arguments.MimeType}' format to local disk: '{path}'");
                resource.SerializeResourceToDisk(path, _arguments.MimeType);
            }

            return _issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)
                ? OperationResultEnum.Failed
                : OperationResultEnum.Succeeded;
        }

        private void ValidateArguments(FhirToolArguments arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments.SourcePath))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{FhirToolArguments.SPLIT_BUNDLE_OP}' requires argument '{FhirToolArguments.SOURCE_ARG}'.",
                    Severity = IssueSeverityEnum.Error,
                });
            }
            else if (!File.Exists(_arguments.SourcePath))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{FhirToolArguments.SPLIT_BUNDLE_OP}' argument '{FhirToolArguments.SOURCE_ARG}' must point to an actual file.",
                    Severity = IssueSeverityEnum.Error,
                });
            }
            // Set default format/MIME type
            if (string.IsNullOrWhiteSpace(arguments.MimeType)) arguments.MimeType = Constants.DEFAULT_MIMETYPE;
        }
    }
}