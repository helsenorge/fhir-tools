using CommandLine;
using FhirTool.Core.ArgumentHelpers;
using FhirTool.Core.Extensions;
using FhirTool.Core.FhirWrappers;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tasks = System.Threading.Tasks;

namespace FhirTool.Core.Operations
{
    [Verb("upload-transaction", HelpText = "uploads files and directoriees using transaction bundles")]
    public class UploadTransactionOperationOptions
    {
        [Option('u', "fhir-base-url", Group = "url", Required = true, HelpText = "fhir server url")]
        public WithFhirBaseUrl FhirBaseUrl { get; set; } = new WithFhirBaseUrl();

        [Option('e', "environment", Group = "url", Required = true, HelpText = "fhir server from environment")]
        public WithEnvironment Environment { get; set; }

        [Option('c', "credentials", HelpText = "credentials")]
        public string Credentials { get; set; }

        [Option('r', "resolve-url", HelpText = "try to resolve url")]
        public bool ResolveUrl { get; set; }

        [Option('f', "format", MetaValue = "xml/json", HelpText = "json or xml")]
        public FhirMimeType? MimeType { get; set; }

        [Option("fhir-version", MetaValue = "fhirVersion", HelpText = "fhir version")]
        public FhirVersion? FhirVersion { get; set; }

        [Option('b', "batch-size", Default = 20, MetaValue = "size", HelpText = "number of resources to upload in a transaction")]
        public int BatchSize { get; set; }

        [Value(0, MetaName = "files", MetaValue = "file or dir", HelpText = "list of files/directories to upload", Required = true)]
        public IEnumerable<WithFileOrDirectory> SourceFiles { get; set; }
    }

    public class UploadTransactionOperation : Operation
    {
        private readonly UploadTransactionOperationOptions _arguments;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<UploadTransactionOperation> _logger;

        public UploadTransactionOperation(UploadTransactionOperationOptions arguments, ILoggerFactory loggerFactory)
        {
            _arguments = arguments;
            _loggerFactory = loggerFactory;

            _logger = loggerFactory.CreateLogger<UploadTransactionOperation>();

            arguments.FhirBaseUrl.Uri ??= arguments.Environment?.FhirBaseUrl;

            Validate(arguments);
        }


        public override async Tasks.Task<OperationResultEnum> Execute()
        {
            var client = new FhirClientWrapper(_arguments.FhirBaseUrl.Uri, _logger, _arguments.FhirVersion);
            var batches = FindFiles().Batch(_arguments.BatchSize);
            foreach (var batch in batches)
            {
                var bundle = await CreateBundle(batch, client.FhirVersion);
                bundle.Type = BundleTypeWrapper.Transaction;

                try
                {
                    _logger.LogInformation($"Uploading batch with {batch.Count()} entries");
                    await client.TransactionAsync(bundle);
                }
                catch(Exception e)
                {
                    _logger.LogError($"Failed to upload batch: {e.Message}");
                    _logger.LogError($"Response was: {client.LastBodyAsText}");
                    var bundleAsStr = new SerializationWrapper(client.FhirVersion).Serialize(bundle.ToBase());
                    await File.WriteAllTextAsync("debug-on-exception", bundleAsStr);
                }
            }

            return await Tasks.Task.FromResult(OperationResultEnum.Succeeded);
        }

        private async Tasks.Task<BundleWrapper> CreateBundle(IEnumerable<string> batch, FhirVersion fhirVersion)
        {
            var serializer = new SerializationWrapper(fhirVersion);
            var bundle = new BundleWrapper(fhirVersion);
            foreach(var file in batch)
            {
                ResourceWrapper resource = null;
                try
                {
                    var content = await File.ReadAllTextAsync(file);
                    resource = serializer.Parse(content, _arguments.MimeType);
                }
                catch(Exception e)
                {
                    _logger.LogError($"Could not read and parse {file}. Skipping: {e.Message}");
                }
                var entry = bundle.AddResourceEntry(resource, null);
                var request = new RequestComponentWrapper(fhirVersion);
                request.Method = HTTPVerbWrapper.PUT;
                request.Url = $"/{resource.ResourceType}/{resource.Id}";
                entry.Request = request;
                _logger.LogInformation($"  Added {request.Url} to bundle");
            }

            return bundle;
        }

        private IEnumerable<string> FindFiles()
        {
            foreach (var path in _arguments.SourceFiles)
            {
                if (path.IsFile())
                {
                    yield return path.Path;
                }
                else if (path.IsDirectory())
                {
                    var files = FindFilesInDirectory(new WithDirectory(path.Path));
                    foreach(var file in files)
                    {
                        yield return file;
                    }
                }
                else
                {
                    _logger.LogWarning($"Don't know how to handle {path.Path} -- Skipping");
                }
            }
        }

        private IEnumerable<string> FindFilesInDirectory(WithDirectory path)
        {
            foreach (var file in Directory.EnumerateFiles(path.Path))
            {
                yield return file;
            }

            foreach (var dir in Directory.EnumerateDirectories(path.Path))
            {
                var files = FindFilesInDirectory(new WithDirectory(dir));
                foreach (var file in files)
                {
                    yield return file;
                }
            }
        }

        private void Validate(UploadTransactionOperationOptions arguments)
        {
            arguments.FhirBaseUrl?.Validate(nameof(arguments.FhirBaseUrl), arguments.ResolveUrl, arguments.Credentials);
            arguments.Environment?.Validate(nameof(arguments.Environment));

            foreach (var item in arguments.SourceFiles)
            {
                item.Validate("files");
            }
        }
    }
}
