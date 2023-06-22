/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using CommandLine;
using JUST;
using FhirTool.Core.ArgumentHelpers;
using FhirTool.Core.FhirWrappers;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Xsl;
using Task = System.Threading.Tasks.Task;
using Tasks = System.Threading.Tasks;
using FhirTool.Core.Utils;
using System.Xml;
using System;

namespace FhirTool.Core.Operations
{
    [Verb("transform", HelpText = "transform")]
    public class TransformOperationOptions
    {
        [Option('o', "out", MetaValue = "path", Default = ".", HelpText = "output")]
        public string OutPath { get; set; }

        [Option('j', "just", MetaValue = "transform", HelpText = "Path to JUST transform", Group = "transform")]
        public WithFile JustTransform { get; set; }

        [Option('x', "xslt", MetaValue = "transform", HelpText ="Path to XSL transform", Group = "transform")]
        public WithFile XslTransform { get; set; }

        [Option("fhir-version", Required = true, HelpText = "which fhir version to assume")]
        public FhirVersion FhirVersion { get; set; }

        [Value(0, Required = true, MetaName = "files", MetaValue = "file or dir", HelpText = "files or directories of fhir files")]
        public IEnumerable<WithFileOrDirectory> SourcePath { get; set; }

        public bool OutputIsDirectory { get; set; } = false;
    }

    public class TransformOperation : Operation
    {
        private readonly TransformOperationOptions _arguments;
        private readonly ILogger<TransformOperation> _logger;

        private readonly JustTransform _justTransformer;
        private readonly XslTransform _xslTransformer;

        public TransformOperation(TransformOperationOptions arguments, ILoggerFactory loggerFactory)
        {
            _arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            if (loggerFactory is null)
                throw new ArgumentNullException(nameof(loggerFactory));

            _logger = loggerFactory.CreateLogger<TransformOperation>();

            if (_arguments.JustTransform != null)
            {
                _justTransformer = new JustTransform(_arguments.JustTransform.Path);
            }
            if(_arguments.XslTransform != null)
            {
                _xslTransformer = new XslTransform(_arguments.XslTransform.Path);
            }

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
                await TransformPath(path, _arguments.OutPath);
            }

            return await Task.FromResult(OperationResultEnum.Succeeded);
        }

        private async Task TransformPath(WithFileOrDirectory path, string outPath)
        {
            if(path.IsDirectory())
            {
                await TransformDirectory(path.Path, outPath);
            }
            else if(path.IsFile())
            {
                await TransformFile(path.Path, outPath);
            }
            else
            {
                _logger.LogWarning($"argument {path.Path} is neither a file nor a directory. Skipping.");
            }
        }

        private async Task TransformDirectory(string directory, string outPath)
        {
            var name = new DirectoryInfo(directory).Name;
            var newOutPath = Path.Combine(outPath, name);
            Directory.CreateDirectory(newOutPath);

            _logger.LogInformation($"Handling directory '{directory}' --> '{newOutPath}'");
            foreach(var fileItem in Directory.EnumerateFiles(directory))
            {
                await TransformFile(fileItem, newOutPath);
            }

            foreach(var directoryItem in Directory.EnumerateDirectories(directory))
            {
                await TransformDirectory(directoryItem, newOutPath);
            }
        }

        private async Task TransformFile(string file, string outPath)
        {
            var content = await File.ReadAllTextAsync(file);
            var currentMimeType = MimeTypeUtils.ProbeFhirMimeType(content);
            if(!currentMimeType.HasValue)
            {
                _logger.LogWarning("Can't determine mime-type of {0} -- Skipping", file);
                return;
            }

            string result = null;
            var newFile = _arguments.OutputIsDirectory ? Path.Combine(outPath, Path.GetFileName(file)) : outPath;
            _logger.LogInformation("Transform {0} --> {1}", file, newFile);
            if (_arguments.XslTransform != null)
            {
                result = TransformXslt(content, currentMimeType.Value);
            }
            else if(_arguments.JustTransform != null)
            {
                result = TransformJust(content, currentMimeType.Value);
            }

            result = GetContentAs(result, currentMimeType.Value);

            await File.WriteAllTextAsync(newFile, result);
        }

        private string TransformXslt(string content, FhirMimeType currentMimeType)
        {
            var xmlFile = GetContentAsXml(content, currentMimeType);
            return _xslTransformer.Transform(xmlFile);
        }

        private string TransformJust(string content, FhirMimeType currentMimeType)
        {
            var jsonFile = GetContentAsJson(content, currentMimeType);
            return _justTransformer.Transform(jsonFile);
        }

        private string GetContentAsJson(string content, FhirMimeType currentMimeType)
        {
            return currentMimeType == FhirMimeType.Json ? content : GetContentAs(content, FhirMimeType.Json);
        }

        private string GetContentAsXml(string content, FhirMimeType currentMimeType)
        {
            return currentMimeType == FhirMimeType.Xml ? content : GetContentAs(content, FhirMimeType.Xml);
        }

        private string GetContentAs(string content, FhirMimeType mimeType)
        {
            var serializer = new SerializationWrapper(_arguments.FhirVersion);
            var resource = serializer.Parse(content);
            return serializer.Serialize(resource, mimeType);
        }

        private void Validate(TransformOperationOptions arguments)
        {
            if (arguments.JustTransform != null)
            {
                arguments.JustTransform.Validate("just");
            }

            if(arguments.XslTransform != null)
            {
                arguments.XslTransform.Validate("xslt");
            }

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

    class JustTransform
    {
        private readonly string _transform;
        private readonly JsonTransformer _transformer;

        public JustTransform(string transform)
        {
            _transform = File.ReadAllText(transform);
            _transformer = new JsonTransformer();
        }

        public string Transform(string content)
        {
            return _transformer.Transform(_transform, content);
        }
    }

    class XslTransform
    {
        private readonly XslCompiledTransform _transformer;

        public XslTransform(string transformFile)
        {
            _transformer = new XslCompiledTransform();
            _transformer.Load(transformFile);
        }

        public string Transform(string content)
        {
            using (StringReader sReader = new StringReader(content))
            using (XmlReader xReader = XmlReader.Create(sReader))
            using (StringWriter sWriter = new StringWriter())
            using (XmlWriter xWriter = XmlWriter.Create(sWriter))
            {
                _transformer.Transform(xReader, xWriter);
                return sWriter.ToString();
            }
        }
    }
}
