using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using Tasks = System.Threading.Tasks;

namespace FhirTool.Core.Operations
{
    internal class BundleResourcesOperation : Operation
    {
        private FhirToolArguments _arguments;
        private ILogger<BundleResourcesOperation> _logger;

        public BundleResourcesOperation(FhirToolArguments arguments, ILogger<BundleResourcesOperation> logger)
        {
            _arguments = arguments;
            _logger = logger;
        }

        public override async Tasks.Task<OperationResultEnum> Execute()
        {
            ValidateArguments(_arguments);
            if (_issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)) return OperationResultEnum.Failed;

            Bundle bundle;
            Uri uri = new Uri(_arguments.SourcePath);
            if (uri.IsHttpScheme())
            {
                throw new NotImplementedException("argument '--source | -S' do not currently support a HTTP scheme.");
            }
            else
            {
                bundle = SerializationUtility.ImportPath(_arguments.SourcePath).ToBundle();
            }

            var path = $"bundle-{Guid.NewGuid().ToString("N").Substring(0, 5)}.{_arguments.MimeType}";
            if (!string.IsNullOrWhiteSpace(_arguments.OutPath) && Directory.Exists(_arguments.OutPath))
                path = Path.Combine(_arguments.OutPath, path);

            _logger.LogInformation($"Writing Bundle in '{_arguments.MimeType}' format to local disk: {path}");
            bundle.SerializeResourceToDisk(path, _arguments.MimeType);

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
                    Details = $"Operation '{FhirToolArguments.BUNDLE_OP}' requires argument '{FhirToolArguments.SOURCE_ARG}'.",
                    Severity = IssueSeverityEnum.Error,
                });
            }
            else if (!(Directory.Exists(_arguments.SourcePath)))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{FhirToolArguments.BUNDLE_OP}' argument '{FhirToolArguments.SOURCE_ARG}' must point to an actual directory.",
                    Severity = IssueSeverityEnum.Error,
                });
            }
            // Set default format/MIME type
            if (string.IsNullOrWhiteSpace(arguments.MimeType)) arguments.MimeType = Constants.DEFAULT_MIMETYPE;
        }
    }
}