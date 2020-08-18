using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Tasks = System.Threading.Tasks;

namespace FhirTool.Core.Operations
{
    internal class ConvertOperation : Operation
    {
        private readonly FhirToolArguments _arguments;
        private readonly ILogger<ConvertOperation> _logger;

        public ConvertOperation(FhirToolArguments arguments, ILogger<ConvertOperation> logger)
        {
            _arguments = arguments;
            _logger = logger;
        }

        public override async Tasks.Task<OperationResultEnum> Execute()
        {
            ValidateInputArgument(_arguments);
            if (_issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)) return OperationResultEnum.Failed;

            return OperationResultEnum.Succeeded;
        }

        private void ValidateInputArgument(FhirToolArguments arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments.SourcePath))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{arguments.Operation}' requires argument '{FhirToolArguments.SOURCE_ARG}' or '{FhirToolArguments.SOURCE_SHORT_ARG}'.",
                    Severity = IssueSeverityEnum.Error,
                });
            }
            if (string.IsNullOrWhiteSpace(arguments.FromFhirVersion))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{arguments.Operation}' requires argument '{FhirToolArguments.CONVERT_FROM_ARG}' or '{FhirToolArguments.CONVERT_FROM_SHORT_ARG}'.",
                    Severity = IssueSeverityEnum.Error,
                });
            }
            if (string.IsNullOrWhiteSpace(arguments.ToFhirVersion))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{arguments.Operation}' requires argument '{FhirToolArguments.CONVERT_TO_ARG}' or '{FhirToolArguments.CONVERT_TO_SHORT_ARG}'.",
                    Severity = IssueSeverityEnum.Error,
                });
            }
        }
    }
}
