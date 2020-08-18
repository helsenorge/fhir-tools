using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using System.Linq;
using Tasks = System.Threading.Tasks;

namespace FhirTool.Core.Operations
{
    internal class UploadResourceOperation : Operation
    {
        private readonly FhirToolArguments _arguments;
        private readonly ILogger<UploadResourceOperation> _logger;

        public UploadResourceOperation(FhirToolArguments arguments, ILogger<UploadResourceOperation> logger)
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

            _logger.LogInformation($"Uploading Questionnaire to endpoint: '{_arguments.FhirBaseUrl}'");
            // Set a relative url before posting to the server
            if (!string.IsNullOrWhiteSpace(questionnaire.Id))
            {
                questionnaire.Url = $"{ResourceType.Questionnaire.GetLiteral()}/{questionnaire.Id}";
            }

            // Initialize a FhirClient and POST or PUT Questionnaire to server.
            FhirClient fhirClient = new FhirClient(_arguments.FhirBaseUrl);
            if (string.IsNullOrWhiteSpace(questionnaire.Id))
                questionnaire = await fhirClient.CreateAsync(questionnaire);
            else
                questionnaire = await fhirClient.UpdateAsync(questionnaire);

            _logger.LogInformation($"Successfully uploaded Questionnaire to endpoint: '{_arguments.FhirBaseUrl}'.");
            _logger.LogInformation($"Questionnaire was assigned the Id: '{questionnaire.Id}'");
            _logger.LogInformation($"Questionnaire can be accessed at: '{fhirClient.Endpoint.AbsoluteUri}{ResourceType.Questionnaire.GetLiteral()}/{questionnaire.Id}'");

            return _issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)
                ? OperationResultEnum.Failed
                : OperationResultEnum.Succeeded;
        }

        private void ValidateArguments(FhirToolArguments arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments.FhirBaseUrl))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{FhirToolArguments.UPLOAD_OP}' requires argument '{FhirToolArguments.FHIRBASEURL_ARG}' or '{FhirToolArguments.ENVIRONMENT_ARG}'.",
                    Severity = IssueSeverityEnum.Error,
                });
            }
            if (string.IsNullOrWhiteSpace(arguments.QuestionnairePath))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{FhirToolArguments.UPLOAD_OP}' requires argument '{FhirToolArguments.QUESTIONNAIRE_ARG}'.",
                    Severity = IssueSeverityEnum.Error,
                });
            }
        }
    }
}