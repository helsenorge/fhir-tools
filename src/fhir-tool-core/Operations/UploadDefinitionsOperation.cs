using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tasks = System.Threading.Tasks;

namespace FhirTool.Core.Operations
{
    internal class UploadDefinitionsOperation : Operation
    {
        private readonly FhirToolArguments _arguments;
        private readonly ILogger<UploadDefinitionsOperation> _logger;

        public UploadDefinitionsOperation(FhirToolArguments arguments, ILogger<UploadDefinitionsOperation> logger)
        {
            _arguments = arguments;
            _logger = logger;
        }

        public override async Tasks.Task<OperationResultEnum> Execute()
        {
            ValidateArguments(_arguments);
            if (_issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)) return OperationResultEnum.Failed;

            IEnumerable<Resource> resources = null;
            

            _logger.LogInformation($"Uploading resources to endpoint: '{_arguments.FhirBaseUrl}'");

            FhirClient fhirClient = new FhirClient(_arguments.FhirBaseUrl);
            fhirClient.OnBeforeRequest += fhirClient_BeforeRequest;
            foreach (Resource resource in resources)
            {
                Resource resource2;
                if (resource is Questionnaire && !string.IsNullOrWhiteSpace(resource.Id))
                {
                    Questionnaire questionnaire = resource as Questionnaire;
                    questionnaire.Url = $"{_arguments.ProxyBaseUrl}Questionnaire/{resource.Id}";
                }
                if (string.IsNullOrWhiteSpace(resource.Id))
                {
                    _logger.LogInformation($"Creating new resource of type: '{resource.TypeName}'");
                    resource2 = fhirClient.Create(resource);
                }
                else
                {
                    _logger.LogInformation($"Updating resource with Id: '{resource.Id}' of type: '{resource.TypeName}'");
                    resource2 = fhirClient.Update(resource);
                }

                _logger.LogInformation($"Resource was assigned the Id: '{resource2.Id}'");
                _logger.LogInformation($"Resource can be accessed at: '{fhirClient.Endpoint.AbsoluteUri}{ResourceType.Questionnaire.GetLiteral()}/{resource2.Id}'");
            }

            _logger.LogInformation($"Successfully uploaded resources to endpoint: {_arguments.FhirBaseUrl}");

            return _issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)
                ? OperationResultEnum.Failed
                : OperationResultEnum.Succeeded;
        }

        private void fhirClient_BeforeRequest(object sender, BeforeRequestEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_arguments.Credentials))
            {
                e.RawRequest.Headers.Add(System.Net.HttpRequestHeader.Authorization, $"Basic {_arguments.Credentials.ToBase64()}");
            }
        }

        private void ValidateArguments(FhirToolArguments arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments.FhirBaseUrl))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{FhirToolArguments.UPLOAD_DEFINITIONS_OP}' requires argument '{FhirToolArguments.FHIRBASEURL_ARG}' or '{FhirToolArguments.ENVIRONMENT_ARG}'.",
                    Severity = IssueSeverityEnum.Error,
                });
            }
            if (string.IsNullOrWhiteSpace(arguments.SourcePath))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{FhirToolArguments.UPLOAD_DEFINITIONS_OP}' requires argument '{FhirToolArguments.SOURCE_ARG}'.",
                    Severity = IssueSeverityEnum.Error,
                });
            }
            if (!(Directory.Exists(arguments.SourcePath) || File.Exists(arguments.SourcePath)))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{FhirToolArguments.UPLOAD_DEFINITIONS_OP}' argument '{FhirToolArguments.SOURCE_ARG}' must point to a existing file or directory.",
                    Severity = IssueSeverityEnum.Error,
                });
            }
        }
    }
}
