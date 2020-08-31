using CommandLine;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Tasks = System.Threading.Tasks;

namespace FhirTool.Core.Operations
{
    [Verb("convert", HelpText = "convert")]
    public class ConvertOperationOptions
    {
        [Option('S', "source", Required = true, HelpText = "source path")]
        public string SourcePath { get; set; }

        [Option("convert-from", Required = true, HelpText = "convert from format")]
        public string FromFhirVersion { get; set; }

        [Option("convert-to", Required = true, HelpText = "convert to format")]
        public string ToFhirVersion { get; set; }
    }

    public class ConvertOperation : Operation
    {
        private readonly ConvertOperationOptions _arguments;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<ConvertOperation> _logger;

        public ConvertOperation(ConvertOperationOptions arguments, ILoggerFactory loggerFactory)
        {
            _arguments = arguments;
            _loggerFactory = loggerFactory;

            _logger = loggerFactory.CreateLogger<ConvertOperation>();
        }

        public override async Tasks.Task<OperationResultEnum> Execute()
        {
            return await Tasks.Task.FromResult(OperationResultEnum.Succeeded);
        }
    }
}
