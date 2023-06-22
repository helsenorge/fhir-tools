/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

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

        [Option('c', "content-type", HelpText = "Content-Type for attachment", Required = true)]
        public string ContentType { get; set; }

        [Option('o', "out", HelpText = "Save documentreference to path", Required = true)]
        public string OutPath { get; set; }

        [Option('f', "format", MetaValue = "xml/json", HelpText = "json or xml")]
        public FhirMimeType MimeType { get; set; }
    }
    public class GenerateDocumentReferenceOperation : Operation
    {
        private readonly GenerateDocumentReferenceOptions _arguments;
        private readonly ILogger<GenerateDocumentReferenceOperation> _logger;

        private readonly DocumentReferenceGenerator _documentReferenceGenerator;

        public GenerateDocumentReferenceOperation(GenerateDocumentReferenceOptions arguments, ILoggerFactory loggerFactory)
        {
            _arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            if (loggerFactory is null)
                throw new ArgumentNullException(nameof(loggerFactory));

            _logger = loggerFactory.CreateLogger<GenerateDocumentReferenceOperation>();

            _documentReferenceGenerator = new DocumentReferenceGenerator(loggerFactory);
        }
        public override async Task<OperationResultEnum> Execute()
        {
            DocumentReference documentReference = _documentReferenceGenerator.GenerateDocumentReference(_arguments);

            var resource = new ResourceWrapper(documentReference, FhirVersion.R4);
            var serializer = new SerializationWrapper(FhirVersion.R4);

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

            await File.WriteAllTextAsync(@$"{_arguments.OutPath}\documentreference-{_arguments.Id}.json", serialized);

            _logger.LogInformation($"Successfully generated 'documentreference-{_arguments.Id}.json' to location: '{_arguments.OutPath}'.");

            return _issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)
               ? OperationResultEnum.Failed
               : OperationResultEnum.Succeeded;
        }

    }
}
