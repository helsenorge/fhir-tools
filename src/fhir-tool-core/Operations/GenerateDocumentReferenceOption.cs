extern alias R4;
using CommandLine;
using FhirTool.Core.FhirWrappers;
using R4::Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace FhirTool.Core.Operations
{
    [Verb("generate-documentreference", HelpText = "Generate documentreference file")]
    public class GenerateDocumentReferenceOptions 
    {
        [Option('i', "id", HelpText = "Documentreference id", Required = true)]
        public string Id { get; set; }

        [Option('c', "contentType", HelpText = "contentType for attachment", Required = true)]
        public string ContentType { get; set; }

        [Option('p', HelpText = "Save documentreference to path", Required = true)]
        public string SaveToPath { get; set; }

        [Option('m', "format", MetaValue = "xml/json", HelpText = "json or xml")]
        public FhirMimeType MimeType { get; set; }

        [Option("fhir-version", MetaValue = "fhirVersion", HelpText = "fhir version")]
        public FhirVersion FhirVersion { get; set; }


    }
    public class GenerateDocumentReferenceOption : Operation
    {
        private readonly GenerateDocumentReferenceOptions _arguments;
        private readonly ILogger<GenerateDocumentReferenceOption> _logger;
        private readonly ILoggerFactory _loggerFactory;

        private readonly DocumentReferenceGenerator _documentReferenceGenerator;

        public GenerateDocumentReferenceOption(GenerateDocumentReferenceOptions arguments, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentOutOfRangeException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<GenerateDocumentReferenceOption>();
            _arguments = arguments;

            _documentReferenceGenerator = new DocumentReferenceGenerator(loggerFactory);
        }
        public override async Task<OperationResultEnum> Execute()
        {
            DocumentReference documentReference = _documentReferenceGenerator.GenerateDocumentReference(_arguments);

            var resource = new ResourceWrapper(documentReference);
            var serializer = new SerializationWrapper(_arguments.FhirVersion);

            var serialized = serializer.Serialize(resource, _arguments.MimeType);

            if (serialized == null)
            {
                _issues.Add(new Issue
                {
                    Details = $"Failed to serialize documentreference for Id: '{_arguments.Id}'.",
                    Severity = IssueSeverityEnum.Error
                });

                return OperationResultEnum.Failed;
            }

            await File.WriteAllTextAsync(@$"{_arguments.SaveToPath}\documentreference-{_arguments.Id}.json", serialized);

            _logger.LogInformation($"Successfully generated 'documentreference-{_arguments.Id}.json' to location: '{_arguments.SaveToPath}'.");

            return _issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)
               ? OperationResultEnum.Failed
               : OperationResultEnum.Succeeded;
        }

    }
}
