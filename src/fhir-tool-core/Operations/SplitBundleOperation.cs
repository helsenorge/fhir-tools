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
    [Verb("split-bundle", HelpText = "split bundle")]
    public class SplitBundleOperationOptions
    {
        [Option('S', "source", Required = true, HelpText = "source path")]
        public WithFile Source { get; set; }

        [Option('o', "out", HelpText = "out path")]
        public string OutPath { get; set; }

        [Option('f', "format", Default = Constants.DEFAULT_MIMETYPE, HelpText = "mime type")]
        public string MimeType { get; set; }
    }

    public class SplitBundleOperation : Operation
    {
        private SplitBundleOperationOptions _arguments;
        private ILoggerFactory _loggerFactory;
        private ILogger<SplitBundleOperation> _logger;

        public SplitBundleOperation(SplitBundleOperationOptions arguments, ILoggerFactory loggerFactory)
        {
            _arguments = arguments;
            _loggerFactory = loggerFactory;

            _logger = loggerFactory.CreateLogger<SplitBundleOperation>();

            ValidateArguments(arguments);
        }

        public override async Tasks.Task<OperationResultEnum> Execute()
        {
            var outPath = !string.IsNullOrWhiteSpace(_arguments.OutPath) && Directory.Exists(_arguments.OutPath) ? _arguments.OutPath : @".\";
            var resources = SerializationUtility.ImportFile(_arguments.Source.Path);
            foreach (Resource resource in resources)
            {
                var path = Path.Combine(outPath, $"{resource.ResourceType.ToString().ToLower()}-{(string.IsNullOrWhiteSpace(resource.Id) ? Guid.NewGuid().ToString("N").Substring(0, 5) : resource.Id)}.{_arguments.MimeType}");
                _logger.LogInformation($"Writing Resource in '{_arguments.MimeType}' format to local disk: '{path}'");
                resource.SerializeResourceToDisk(path, _arguments.MimeType);
            }

            return await Tasks.Task.FromResult(_issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)
                ? OperationResultEnum.Failed
                : OperationResultEnum.Succeeded);
        }

        private void ValidateArguments(SplitBundleOperationOptions arguments)
        {
            arguments.Source.Validate(nameof(arguments.Source));
        }
    }
}