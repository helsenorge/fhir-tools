using CommandLine;
using FhirTool.Conversion;
using FhirTool.Core.ArgumentHelpers;
using FhirTool.Core.FhirWrappers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

using Tasks = System.Threading.Tasks;

namespace FhirTool.Core.Operations
{
    [Verb("convert", HelpText = "convert")]
    public class ConvertOperationOptions
    {
        [Option("from", MetaValue = "FhirVersion", Required = true, HelpText = "convert from fhir format")]
        public FhirVersion FromFhirVersion { get; set; }

        [Option("to", MetaValue = "FhirVersion", Required = true, HelpText = "convert to fhir format")]
        public FhirVersion ToFhirVersion { get; set; }

        [Option('o', "out", MetaValue = "path", Default = ".", HelpText = "output")]
        public string OutPath { get; set; }

        [Value(0, Required = true, MetaName = "files", MetaValue = "file or dir", HelpText = "files or directories of fhir files")]
        public IEnumerable<WithFileOrDirectory> SourcePath { get; set; }

        public bool OutputIsDirectory { get; set; } = false;
    }

    public class ConvertOperation : Operation
    {
        private readonly ConvertOperationOptions _arguments;
        private readonly ILoggerFactory _loggerFactory;
        private readonly FhirConverterWrapper _converter;
        private readonly ILogger<ConvertOperation> _logger;

        public ConvertOperation(ConvertOperationOptions arguments, ILoggerFactory loggerFactory)
        {
            _arguments = arguments;
            _loggerFactory = loggerFactory;
            _converter = new FhirConverterWrapper(arguments.ToFhirVersion, arguments.FromFhirVersion);

            _logger = loggerFactory.CreateLogger<ConvertOperation>();

            Validate(arguments);
        }

        public override async Tasks.Task<OperationResultEnum> Execute()
        {
            if(_arguments.OutputIsDirectory)
            {
                Directory.CreateDirectory(_arguments.OutPath);
            }

            foreach (var path in _arguments.SourcePath)
            {
                await ConvertPath(path, _arguments.OutPath);
            }

            return await Task.FromResult(OperationResultEnum.Succeeded);
        }

        private async Task ConvertPath(WithFileOrDirectory path, string outPath)
        {
            if(path.IsDirectory())
            {
                await ConvertDirectory(path.Path, outPath);
            }
            else if(path.IsFile())
            {
                await ConvertFile(path.Path, outPath);
            }
            else
            {
                _logger.LogWarning($"argument {path.Path} is neither a file nor a directory. Skipping.");
            }
        }

        private async Task ConvertDirectory(string directory, string outPath)
        {
            var name = new DirectoryInfo(directory).Name;
            var newOutPath = Path.Combine(outPath, name);
            Directory.CreateDirectory(newOutPath);

            _logger.LogInformation($"Handling directory '{directory}' --> '{newOutPath}'");
            foreach(var fileItem in Directory.EnumerateFiles(directory))
            {
                await ConvertFile(fileItem, newOutPath);
            }

            foreach(var directoryItem in Directory.EnumerateDirectories(directory))
            {
                await ConvertDirectory(directoryItem, newOutPath);
            }
        }

        private async Task ConvertFile(string file, string outPath)
        {
            var newFile = _arguments.OutputIsDirectory ? Path.Combine(outPath, Path.GetFileName(file)) : outPath;
            _logger.LogInformation($"  Converting '{file}' --> {newFile}");

            var content = await File.ReadAllTextAsync(file);
            var newContent = _converter.Convert(content);

            await File.WriteAllTextAsync(newFile, newContent);
        }

        private void Validate(ConvertOperationOptions arguments)
        {
            foreach(var path in arguments.SourcePath)
            {
                path.Validate("files");
            }

            if(arguments.SourcePath.Count() > 1 || arguments.SourcePath.First().IsDirectory())
            {
                arguments.OutputIsDirectory = true;
                if(File.Exists(arguments.OutPath))
                {
                    throw new SemanticArgumentException($"output must point to a directory when converting multiple files", nameof(arguments.OutPath));
                }
            }
        }
    }
}
