using System.Collections.Generic;

namespace FhirTool.Core.Operations
{
    public class ConvertOperation : IOperation<object>
    {
        private readonly IList<Issue> _issues = new List<Issue>();

        public IEnumerable<Issue> Issues => _issues;

        public Result<object> Execute(FhirToolArguments arguments)
        {
            if (string.IsNullOrEmpty(arguments.SourcePath))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{arguments.Operation}' requires argument '{FhirToolArguments.SOURCE_ARG}' or '{FhirToolArguments.SOURCE_SHORT_ARG}'.",
                    Severity = IssueSeverityEnum.Error
                });
                return null;
            }
            if (string.IsNullOrEmpty(arguments.FromFhirVersion))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{arguments.Operation}' requires argument '{FhirToolArguments.CONVERT_FROM_ARG}' or '{FhirToolArguments.CONVERT_FROM_SHORT_ARG}'.",
                    Severity = IssueSeverityEnum.Error
                });
                return null;
            }
            if (string.IsNullOrEmpty(arguments.ToFhirVersion))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{arguments.Operation}' requires argument '{FhirToolArguments.CONVERT_TO_ARG}' or '{FhirToolArguments.CONVERT_TO_SHORT_ARG}'.",
                    Severity = IssueSeverityEnum.Error
                });
                return null;
            }

            return null;
        }
    }
}
