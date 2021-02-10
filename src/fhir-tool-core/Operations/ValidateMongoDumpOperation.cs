/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using CommandLine;
using FhirTool.Core.ArgumentHelpers;
using FhirTool.Core.FhirWrappers;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Tasks = System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace FhirTool.Core.Operations
{
    [Verb("validate-dump", HelpText = "validate mongo dumps")]
    public class ValidateMongoDumpOperationOptions
    {

        [Option("fhir-version", MetaValue = "fhirVersion", Required = true, HelpText = "fhir version")]
        public FhirVersion FhirVersion { get; set; }

        [Option('p', "permissive-parsing", Default = false, HelpText = "use permissive parsing")]
        public bool PermissiveParsing { get; set; }

        [Option('s', "skip", HelpText = "ids to skip")]
        public IList<string> SkipIds { get; set; } = new List<string>();

        [Option("show-json", HelpText = "show original json on error")]
        public bool ShowOriginalJson { get; set; }

        [Value(0, MetaName = "dumpfile", MetaValue = "dumpfile", HelpText = "dump file", Required = true)]
        public WithFile DumpFile { get; set; }
    }

    public class ValidateMongoDumpOperation : Operation
    {
        private readonly ValidateMongoDumpOperationOptions _arguments;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<ValidateMongoDumpOperation> _logger;

        public ValidateMongoDumpOperation(ValidateMongoDumpOperationOptions arguments, ILoggerFactory loggerFactory)
        {
            _arguments = arguments;
            _loggerFactory = loggerFactory;

            _logger = loggerFactory.CreateLogger<ValidateMongoDumpOperation>();

            Validate(arguments);
        }

        public override async Tasks.Task<OperationResultEnum> Execute()
        {
            var serializer = new SerializationWrapper(_arguments.FhirVersion);
            var lines = await File.ReadAllLinesAsync(_arguments.DumpFile.Path);
            var linenr = 0;

            var versionMap = new Dictionary<string, Data>();
            foreach (var entry in lines)
            {
                linenr++;
                try
                {
                    var json = JObject.Parse(entry);

                    var versionPath = json["_id"].ToString();
                    json.Property("_id").Remove();
                    var id = $"{json["resourceType"]}/{json["id"]}";

                    if(_arguments.SkipIds.Contains(id))
                    {
                        _logger.LogInformation($"Skipping {id} as requested");
                        continue;

                    }

                    RemoveUnwantedMongoDbElements(json);

                    var dataEntry = new Data
                    {
                        Id = id,
                        JObject = json,
                        Line = linenr,
                        RawContent = entry,
                        VersionPath = versionPath
                    };

                    if (versionMap.TryGetValue(id, out Data other))
                    {
                        var version = GetVersion(json);
                        var otherVersion = GetVersion(other.JObject);

                        if (version > otherVersion)
                        {
                            versionMap.Remove(id);
                            versionMap.Add(id, dataEntry);
                        }
                    }
                    else
                    {
                        versionMap.Add(id, dataEntry);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"line {linenr}: Not valid json{Environment.NewLine}{e.Message}");
                    if (_arguments.ShowOriginalJson)
                    {
                        _logger.LogInformation(entry);
                    }
                    continue;
                }
            }

            foreach(var key in versionMap.Keys)
            {
                var data = versionMap[key];
                try
                {
                    var content = data.JObject.ToString();
                    var resource = serializer.Parse(content, permissiveParsing: _arguments.PermissiveParsing);
                }
                catch(Exception e)
                {
                    _logger.LogError($"line {data.Line}: resource {key} is not a valid FHIR object (was {data.VersionPath}){Environment.NewLine}{e.Message}");
                    if (_arguments.ShowOriginalJson)
                    {
                        _logger.LogInformation(data.RawContent);
                    }
                    continue;
                }
            }

            return await Tasks.Task.FromResult(OperationResultEnum.Succeeded);
        }

        private void RemoveUnwantedMongoDbElements(JObject json)
        {
            RemoveUnwantedMongoDbTopLevelElements(json);
            RemoveUnwantedMongoDbNumberInts(json);
        }

        private void RemoveUnwantedMongoDbNumberInts(JObject json)
        {
            foreach(var p in json.Properties())
            {
                if (p.Value.Type == JTokenType.Object)
                {
                    if (IsJObjectWithNumberInt((JObject)p.Value))
                    {
                        json[p.Name] = ((JObject)p.Value)["$numberInt"];
                    }
                    else if(IsJObjectWithNumberDouble((JObject)p.Value))
                    {
                        json[p.Name] = ((JObject)p.Value)["$numberDouble"];
                    }
                    else if(IsJObjectWithNumberLong((JObject)p.Value))
                    {
                        json[p.Name] = ((JObject)p.Value)["$numberLong"];
                    }
                    else
                    {
                        RemoveUnwantedMongoDbNumberInts((JObject)p.Value);
                    }
                }
                else if(p.Value.Type == JTokenType.Array)
                {
                    RemoveUnwantedMongoDbNumberInts((JArray)p.Value);
                }
            }
        }

        private void RemoveUnwantedMongoDbNumberInts(JArray json)
        {
            foreach(var e in json)
            {
                if (e.Type == JTokenType.Object)
                {
                    RemoveUnwantedMongoDbNumberInts((JObject)e);
                }
                else if (e.Type == JTokenType.Array)
                {
                    RemoveUnwantedMongoDbNumberInts((JArray)e);
                }
            }
        }

        private bool IsJObjectWithNumberInt(JObject json)
        {
            return json.ContainsKey("$numberInt");
        }

        private bool IsJObjectWithNumberDouble(JObject json)
        {
            return json.ContainsKey("$numberDouble");
        }

        private bool IsJObjectWithNumberLong(JObject json)
        {
            return json.ContainsKey("$numberLong");
        }

        private void RemoveUnwantedMongoDbTopLevelElements(JObject json)
        {
            var toRemove = new List<string>();
            foreach (var p in json.Properties())
            {
                if (p.Name.StartsWith("@"))
                {
                    toRemove.Add(p.Name);
                }
            }

            foreach (var p in toRemove)
            {
                json.Property(p).Remove();
            }
        }

        private int GetVersion(JObject json)
        {
            var meta = (JObject)json["meta"];
            var versionId = meta["versionId"].ToString();
            return int.Parse(versionId);
        }

        private void Validate(ValidateMongoDumpOperationOptions arguments)
        {
            _arguments.DumpFile.Validate(nameof(_arguments.DumpFile));
        }
    }

    class Data
    {
        public string Id { get; set; }
        public JObject JObject { get; set; }
        public string VersionPath { get; set; }
        public string RawContent { get; set; }
        public int Line { get; set; }
    }
}
