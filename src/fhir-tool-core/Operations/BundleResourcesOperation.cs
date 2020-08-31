extern alias R3;

using R3::Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using Tasks = System.Threading.Tasks;
using CommandLine;
using FhirTool.Core.ArgumentHelpers;

namespace FhirTool.Core.Operations
{
    [Verb("bundle", HelpText = "bundle resources")]
    public class BundleResourcesOperationOptions
    {
        [Option('S', "source", Required = true, HelpText = "source path")]
        public WithDirectory Source { get; set; }

        [Option('f', "format", Default = Constants.DEFAULT_MIMETYPE, HelpText = "mime type")]
        public string MimeType { get; set; }

        [Option('o', "out", HelpText = "output path")]
        public string OutPath { get; set; }
    }

    public class BundleResourcesOperation : Operation
    {
        private BundleResourcesOperationOptions _arguments;
        private ILoggerFactory _loggerFactory;
        private ILogger<BundleResourcesOperation> _logger;

        public BundleResourcesOperation(BundleResourcesOperationOptions arguments, ILoggerFactory loggerFactory)
        {
            _arguments = arguments;
            _loggerFactory = loggerFactory;

            _logger = loggerFactory.CreateLogger<BundleResourcesOperation>();

            ValidateArguments(arguments);
        }

        public override async Tasks.Task<OperationResultEnum> Execute()
        {
            Bundle bundle;
            Uri uri = new Uri(_arguments.Source.Path);
            if (uri.IsHttpScheme())
            {
                throw new NotImplementedException("argument '--source | -S' do not currently support a HTTP scheme.");
            }
            else
            {
                bundle = SerializationUtility.ImportPath(_arguments.Source.Path).ToBundle();
            }

            var path = $"bundle-{Guid.NewGuid().ToString("N").Substring(0, 5)}.{_arguments.MimeType}";
            if (!string.IsNullOrWhiteSpace(_arguments.OutPath) && Directory.Exists(_arguments.OutPath))
                path = Path.Combine(_arguments.OutPath, path);

            _logger.LogInformation($"Writing Bundle in '{_arguments.MimeType}' format to local disk: {path}");
            bundle.SerializeResourceToDisk(path, _arguments.MimeType);

            return await Tasks.Task.FromResult(_issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)
                ? OperationResultEnum.Failed
                : OperationResultEnum.Succeeded);
        }

        private void ValidateArguments(BundleResourcesOperationOptions arguments)
        {
            arguments.Source.Validate(nameof(arguments.Source));
        }
    }
}