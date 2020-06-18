using FhirTool.Core;
using System.Collections.Generic;

namespace FhirTool.Operations
{
    internal class ConvertOperation : IOperation
    {
        private readonly IList<Issue> _issues = new List<Issue>();
        public ConvertOperation()
        {
        }

        public IEnumerable<Issue> Issues => _issues;

        public void Execute(FhirToolArguments arguments)
        {
            if (string.IsNullOrEmpty(arguments.SourcePath))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{arguments.Operation}' requires argument '{FhirToolArguments.SOURCE_ARG}' or '{FhirToolArguments.SOURCE_SHORT_ARG}'.",
                    Severity = IssueSeverityEnum.Error
                });
                return;
            }
            if (string.IsNullOrEmpty(arguments.FromFhirVersion))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{arguments.Operation}' requires argument '{FhirToolArguments.CONVERT_FROM_ARG}' or '{FhirToolArguments.CONVERT_FROM_SHORT_ARG}'.",
                    Severity = IssueSeverityEnum.Error
                });
                return;
            }
            if (string.IsNullOrEmpty(arguments.ToFhirVersion))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{arguments.Operation}' requires argument '{FhirToolArguments.CONVERT_TO_ARG}' or '{FhirToolArguments.CONVERT_TO_SHORT_ARG}'.",
                    Severity = IssueSeverityEnum.Error
                });
                return;
            }

            
        }
    }
}
