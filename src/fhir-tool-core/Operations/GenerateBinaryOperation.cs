extern alias R4;
using CommandLine;
using FhirTool.Core.ArgumentHelpers;
using R4::Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using FhirTool.Core.FhirWrappers;
using System.IO;
using System.Linq;

namespace FhirTool.Core.Operations
{
    [Verb("generate-binary", HelpText = "Generates binary file")]
    public class GenerateBinaryOperationOptions
    {       
        [Option('i', "id", HelpText = "Binary id", Required = true)]
        public string Id { get; set; }

        [Option('c', "content-type", HelpText = "Content-Type", Required = true)]
        public string ContentType { get; set; }

        [Option('s', "securityContext", HelpText = "Security context", Required = true)]
        public string SecurityContext { get; set; }

        [Option('p', "path", HelpText = "path to file", Required = true)]
        public WithFile Path { get; set; }       

        [Option('f', "format", MetaValue = "xml/json", HelpText = "json or xml")]
        public FhirMimeType MimeType { get; set; }

        [Option('o', "out", HelpText = "Save binary to path", Required = true)]
        public string OutPath { get; set; }
    }
    public class GenerateBinaryOperation : Operation
    {
        private readonly GenerateBinaryOperationOptions _arguments;
        private readonly ILogger<GenerateBinaryOperation> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly BinaryGenerator _generator;
        public GenerateBinaryOperation(GenerateBinaryOperationOptions arguments, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentOutOfRangeException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<GenerateBinaryOperation>();
            _arguments = arguments;

            _generator = new BinaryGenerator(loggerFactory);
        }
        public override async Task<OperationResultEnum> Execute()
        {
            Validate(_arguments);

            Binary binary = _generator.GenerateBinary(_arguments);

            var resource = new ResourceWrapper(binary);

            var serializer = new SerializationWrapper(FhirVersion.R4);

            var serialized = serializer.Serialize(resource, _arguments.MimeType);

            if (serialized == null)
            {
                _issues.Add(new Issue
                {
                    Details = $"Failed to serialize binary from file\nLocation: '{_arguments.Path}'.",
                    Severity = IssueSeverityEnum.Error
                });

                return OperationResultEnum.Failed;
            }

            await File.WriteAllTextAsync(@$"{_arguments.OutPath}\binary-{_arguments.Id}.json", serialized);

            _logger.LogInformation($"Successfully generated 'binary-{_arguments.Id}.json' to location: '{_arguments.OutPath}'.");

            return _issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)
                ? OperationResultEnum.Failed
                : OperationResultEnum.Succeeded;
        }

        private void Validate(GenerateBinaryOperationOptions arguments)
        {
            arguments.Path.Validate(nameof(arguments.Path));
        }
    }
}
