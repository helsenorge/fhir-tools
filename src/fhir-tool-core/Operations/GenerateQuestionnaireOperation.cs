using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using Tasks = System.Threading.Tasks;

namespace FhirTool.Core.Operations
{
    internal class GenerateQuestionnaireOperation : Operation
    {
        private readonly string[] ExcelSheetVersions = new[] { "1", "2" };
        private readonly FhirToolArguments _arguments;
        private readonly QuestionnaireGenerator _generator;
        private readonly ILogger<GenerateQuestionnaireOperation> _logger;
        private readonly OperationFactory _operationFactory;

        public GenerateQuestionnaireOperation(FhirToolArguments arguments, QuestionnaireGenerator generator, ILogger<GenerateQuestionnaireOperation> logger, OperationFactory operationFactory)
        {
            _arguments = arguments ?? throw new ArgumentOutOfRangeException(nameof(arguments));
            _generator = generator ?? throw new ArgumentOutOfRangeException(nameof(generator));
            _logger = logger ?? throw new ArgumentOutOfRangeException(nameof(logger));
            _operationFactory = operationFactory ?? throw new ArgumentOutOfRangeException(nameof(operationFactory));
        }

        public override async Tasks.Task<OperationResultEnum> Execute()
        {
            ValidateInputArguments(_arguments);
            if (_issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)) return OperationResultEnum.Failed;

            Questionnaire questionnaire = _generator.GenerateQuestionnaireFromFlatFile(_arguments);
            if (questionnaire == null)
            {
                _issues.Add(new Issue
                {
                    Details = $"Failed to extract Questionnaire from flat file format\nLocation: '{_arguments.QuestionnairePath}'.",
                    Severity = IssueSeverityEnum.Error,
                });
            }

            string filename = $"{questionnaire.Name}-{ questionnaire.Language}-{questionnaire.Version}.{_arguments.MimeType}";
            IOUtility.GenerateLegalFilename(filename);
            
            questionnaire.SerializeResourceToDisk(filename, _arguments.MimeType);

            _logger.LogInformation($"Questionnaire written in '{_arguments.MimeType}' format to local disk: {filename}");
            _logger.LogInformation($"Questionnaire will be assigned the Id: {questionnaire.Id}");

            if(!_arguments.SkipValidation)
            {
                var verifyValidationOperation = _operationFactory.Create(OperationEnum.VerifyValidation, new FhirToolArguments { QuestionnairePath = filename, MimeType = _arguments.MimeType });
                await verifyValidationOperation.Execute();
                _issues.AddRange(verifyValidationOperation.Issues);
            }

            return _issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)
                ? OperationResultEnum.Failed
                : OperationResultEnum.Succeeded;
        }

        private void ValidateInputArguments(FhirToolArguments arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments.QuestionnairePath))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{FhirToolArguments.GENERATE_OP}' requires argument {FhirToolArguments.QUESTIONNAIRE_ARG} | {FhirToolArguments.QUESTIONNAIRE_SHORT_ARG} is required.",
                    Severity = IssueSeverityEnum.Error,
                });
            }
            else if (!File.Exists(arguments.QuestionnairePath))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{FhirToolArguments.GENERATE_OP}' File specified by argument {FhirToolArguments.QUESTIONNAIRE_ARG} | {FhirToolArguments.QUESTIONNAIRE_SHORT_ARG} was not found: '{arguments.QuestionnairePath}'",
                    Severity = IssueSeverityEnum.Error,
                });
            }

            if(string.IsNullOrWhiteSpace(arguments.MimeType))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{FhirToolArguments.GENERATE_OP}' requires argument {FhirToolArguments.MIMETYPE_ARG} | {FhirToolArguments.MIMETYPE_SHORT_ARG} is required.",
                    Severity = IssueSeverityEnum.Error,
                });
            }

            if(!ExcelSheetVersions.Contains(arguments.Version))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation {FhirToolArguments.GENERATE_OP} requires argument '{FhirToolArguments.VERSION_ARG}'. Argument is either missing or an unknown value was set.\nValue: '{arguments.Version}'",
                    Severity = IssueSeverityEnum.Error,
                });
            }
        }
    }
}
