/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using CommandLine;
using FhirTool.Core.ArgumentHelpers;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FhirTool.Core.FhirWrappers;
using System;
using System.Text;

namespace FhirTool.Core.Operations
{
    [Verb("convert-format", HelpText = "Converts from one file format to another. I.e Json to Xml or vice versa.")]
    public class ConvertFormatOperationOptions
    {
        [Option('s', "source", HelpText = "The path ot the file being transformed.", Required = true)]
        public string Source { get; set; }

        [Option('t', "to", Required = true, HelpText = "The file format to convert to. Allowed values are json or xml.")]
        public string ToFormat { get; set; }

        [Option('o', "out", Default = ".", HelpText = "The path where the file should be stored.")]
        public string OutPath { get; set; }

        [Option('v', "fhir-version", Required = false, HelpText = "which fhir version to assume", Default = FhirVersion.R4)]
        public FhirVersion FhirVersion { get; set; }

        internal FhirMimeType? ToMimeType { get; set; }
        internal FhirMimeType? FromMimeType { get; set; }

        internal string SourceContent { get; set; }
    }

    public class ConvertFormatOperation : Operation
    {
        private readonly ConvertFormatOperationOptions _arguments;
        private readonly ILogger<ConvertFormatOperation> _logger;

        public ConvertFormatOperation(ConvertFormatOperationOptions arguments, ILogger<ConvertFormatOperation> logger)
        {
            _arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            _logger = logger;

            Validate(_arguments);
        }

        public override async Task<OperationResultEnum> Execute()
        {
            _logger.LogInformation($"Starting conversion of source file: '{_arguments.Source}' from format: '{_arguments.FromMimeType}' to format '{_arguments.ToMimeType}'.");

            var serializer = new SerializationWrapper(_arguments.FhirVersion);
            var resource = serializer.Parse(_arguments.SourceContent, _arguments.FromMimeType, true);
            var outContent = serializer.Serialize(resource, _arguments.ToMimeType.GetValueOrDefault());
            var fileName = Path.GetFileName(_arguments.Source);
            var outPath = GetOutPath(_arguments.OutPath, fileName, _arguments.ToMimeType.GetValueOrDefault());
            if (!string.IsNullOrWhiteSpace(outContent))
            {
                using var stream = File.Open(outPath, FileMode.OpenOrCreate, FileAccess.Write);
                await stream.WriteAsync(Encoding.UTF8.GetBytes(outContent));

                _logger.LogInformation($"Converted file stored at: '{outPath}'.");
            }

            return await Task.FromResult(OperationResultEnum.Succeeded);
        }

        private string GetOutPath(string outPath, string fileName, FhirMimeType mimeType)
        {
            string path = outPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fileNameWithOutExtension = Path.GetFileNameWithoutExtension(fileName);
            path = Path.Combine(path, $"{fileNameWithOutExtension}.{GetFileExtension(mimeType)}");

            return path;
        }

        private string GetFileExtension(FhirMimeType? mimeType)
        {
            return mimeType switch
            {
                FhirMimeType.Json => "json",
                FhirMimeType.Xml => "xml",
                _ => throw new UnknownMimeTypeException(mimeType.GetLiteral()),
            };
        }

        private FhirMimeType? GetResourceFormat(string data)
        {
            var format = default(FhirMimeType);
            if (SerializationUtil.ProbeIsJson(data))
            {
                format = FhirMimeType.Json;
            }
            else if(SerializationUtil.ProbeIsXml(data))
            {
                format = FhirMimeType.Xml;
            }

            return format;
        }

        private void Validate(ConvertFormatOperationOptions arguments)
        {
            if(!File.Exists(arguments.Source))
            {
                throw new SemanticArgumentException("The 'source' argument must reference an existing file.", nameof(arguments.Source));
            }

            arguments.SourceContent = File.ReadAllText(arguments.Source);
            arguments.FromMimeType = GetResourceFormat(arguments.SourceContent);
            if (arguments.FromMimeType == default)
            {
                throw new SemanticArgumentException("Unknown Format. The 'source' argument must point to a valid FHIR Resource.", nameof(arguments.Source));
            }

            var toFormat = arguments.ToFormat.ToLowerInvariant();
            if (!new[] { "json", "xml" }.Contains(toFormat))
            {
                throw new SemanticArgumentException("Unknown format. Valid values for 'to' argument is 'json' or 'xml'.", nameof(arguments.ToFormat));
            }
            arguments.ToMimeType = Enum.Parse<FhirMimeType>(toFormat, ignoreCase: true);

            var fromFormat = arguments.FromMimeType.GetLiteral().ToLowerInvariant();
            if (toFormat == fromFormat)
            {
                throw new SemanticArgumentException("The 'to' format must differ from contents of the 'source' format.", nameof(arguments.ToFormat));
            }

            if(!(arguments.FhirVersion == FhirVersion.R3 || arguments.FhirVersion == FhirVersion.R4))
            {
                throw new SemanticArgumentException("The 'fhir-version' parameter only accepts the arguments 'R3' or 'R4'.", nameof(arguments.FhirVersion));
            }
        }
    }
}
