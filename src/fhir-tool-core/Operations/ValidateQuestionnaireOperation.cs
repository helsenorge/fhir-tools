using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Tasks = System.Threading.Tasks;

namespace FhirTool.Core.Operations
{
    internal class VerifyValidationItems : Operation
    {
        private readonly FhirToolArguments _arguments;
        private readonly ILogger<VerifyValidationItems> _logger;

        public VerifyValidationItems(FhirToolArguments arguments, ILogger<VerifyValidationItems> logger)
        {
            _arguments = arguments;
            _logger = logger;
        }

        public override async Tasks.Task<OperationResultEnum> Execute()
        {
            ValidateArguments(_arguments);
            if (_issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)) return OperationResultEnum.Failed;

            _logger.LogInformation($"Deserializing Fhir resource at '{_arguments.QuestionnairePath}'.");
            _logger.LogInformation($"Expecting format: '{_arguments.MimeType}'.");
            Questionnaire questionnaire = await SerializationUtility.DeserializeResource<Questionnaire>(_arguments.QuestionnairePath);
            if (questionnaire == null)
            {
                _issues.Add(new Issue
                {
                    Details = $"Failed to deserialize Questionnaire from file\nLocation: '{_arguments.QuestionnairePath}'.",
                    Severity = IssueSeverityEnum.Error,
                });
                return OperationResultEnum.Failed;
            }

            _issues.AddRange(VerifyItemValidation(questionnaire));

            return _issues.Any(issue => issue.Severity == IssueSeverityEnum.Error) 
                ? OperationResultEnum.Failed 
                : OperationResultEnum.Succeeded;
        }

        private void ValidateArguments(FhirToolArguments arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments.QuestionnairePath))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{arguments.Operation}' requires argument '{FhirToolArguments.QUESTIONNAIRE_ARG}' or '{FhirToolArguments.QUESTIONNAIRE_SHORT_ARG}'.",
                    Severity = IssueSeverityEnum.Error,
                });
            }
            else if (!SerializationUtility.ProbeIsJsonOrXml(arguments.QuestionnairePath))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{arguments.Operation}' requires that argument '{FhirToolArguments.QUESTIONNAIRE_ARG}' or '{FhirToolArguments.QUESTIONNAIRE_SHORT_ARG}' refers to a file that contains either JSON or XML.",
                    Severity = IssueSeverityEnum.Error,
                });
            }
        }

        private static IEnumerable<Issue> VerifyItemValidation(Questionnaire questionnaire)
        {
            var issues = new List<Issue>();
            foreach (Questionnaire.ItemComponent item in questionnaire.Item)
            {
                issues.AddRange(questionnaire.VerifyItemValidation(item));
            }

            return issues;
        }
    }
}
