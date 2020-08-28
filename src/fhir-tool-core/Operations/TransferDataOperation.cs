extern alias R3;

using R3::Hl7.Fhir.Model;
using R3::Hl7.Fhir.Rest;
using R3::Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Tasks = System.Threading.Tasks;

namespace FhirTool.Core.Operations
{
    internal class TransferDataOperation : Operation
    {
        private FhirToolArguments _arguments;
        private ILogger<TransferDataOperation> _logger;

        public TransferDataOperation(FhirToolArguments arguments, ILogger<TransferDataOperation> logger)
        {
            this._arguments = arguments;
            _logger = logger;
        }

        public override async Tasks.Task<OperationResultEnum> Execute()
        {
            ValidateArguments(_arguments);
            if (_issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)) return OperationResultEnum.Failed;

            FhirJsonSerializer serializer = new FhirJsonSerializer();
            FhirClient sourceClient = new FhirClient(_arguments.SourceFhirBaseUrl)
            {
                ParserSettings = new ParserSettings
                {
                    PermissiveParsing = false,
                }
            };
            HttpClient destClient = new HttpClient()
            {
                BaseAddress = new Uri(_arguments.DestinationFhirBaseUrl),
            };

            string relativeUrl = $"{_arguments.ResourceType.GetLiteral()}";
            if (_arguments.SearchCount > 0)
                relativeUrl += $"?_count={_arguments.SearchCount}";

            Bundle sourceBundle = await sourceClient.GetAsync(relativeUrl) as Bundle;
            foreach (Bundle.EntryComponent entry in sourceBundle.Entry)
            {
                Resource resource = entry.Resource;
                string resourceType = resource.ResourceType.GetLiteral();

                if (resource is Questionnaire questionnaire)
                {
                    // This part gets rid of some legacy
                    // TODO: Remove when we have gotten rid of the legacy
                    if (string.IsNullOrWhiteSpace(questionnaire.ApprovalDate)) questionnaire.ApprovalDate = null;
                    if (string.IsNullOrWhiteSpace(questionnaire.LastReviewDate)) questionnaire.LastReviewDate = null;
                    if (questionnaire.Copyright != null && string.IsNullOrWhiteSpace(questionnaire.Copyright.Value)) questionnaire.Copyright = null;

                    // Update known properties and extensions with urls that points to the old source instance.
                    // TODO: The lines referring FhirBaseUrl is legacy and can be removed in a future version.
                    questionnaire.Url = questionnaire.Url.Replace(_arguments.SourceProxyBaseUrl, string.Empty);
                    questionnaire.Url = questionnaire.Url.Replace(_arguments.SourceFhirBaseUrl, string.Empty);

                    IEnumerable<Extension> extensions = questionnaire.GetExtensions(Constants.EndPointUri);
                    foreach (Extension extension in extensions)
                    {
                        if (extension.Value is ResourceReference v)
                        {
                            v.Reference = v.Reference.Replace(_arguments.SourceProxyBaseUrl, string.Empty);
                            v.Reference = v.Reference.Replace(_arguments.SourceFhirBaseUrl, string.Empty);
                        }
                    }

                    extensions = questionnaire.GetExtensions(Constants.OptionReferenceUri);
                    foreach (Extension extension in extensions)
                    {
                        if (extension.Value is ResourceReference v)
                        {
                            v.Reference = v.Reference.Replace(_arguments.SourceProxyBaseUrl, string.Empty);
                            v.Reference = v.Reference.Replace(_arguments.SourceFhirBaseUrl, string.Empty);
                        }
                    }
                }

                _logger.LogInformation($"Preparing to write resource of type '{resourceType}' to '{_arguments.DestinationFhirBaseUrl}'");
                HttpContent content = new StringContent(serializer.SerializeToString(resource));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/fhir+json");
                HttpResponseMessage response;
                if (string.IsNullOrWhiteSpace(resource.Id))
                    response = await destClient.PostAsync($"{resource.ResourceType.GetLiteral()}", content);
                else
                    response = await destClient.PutAsync($"{resource.ResourceType.GetLiteral()}/{resource.Id}", content);

                _logger.LogInformation($"{response.StatusCode} - {response.RequestMessage.Method} {response.RequestMessage.RequestUri}");
            }

            return _issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)
            ? OperationResultEnum.Failed
            : OperationResultEnum.Succeeded;
        }

        private void ValidateArguments(FhirToolArguments arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments.SourceEnvironment))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{FhirToolArguments.UPLOAD_DEFINITIONS_OP}' requires argument '{FhirToolArguments.ENVIRONMENT_SOURCE_ARG}' or '{FhirToolArguments.ENVIRONMENT_SOURCE_SHORT_ARG}'",
                    Severity = IssueSeverityEnum.Error,
                });
            }
            if (string.IsNullOrWhiteSpace(arguments.DestinationEnvironment))
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{FhirToolArguments.UPLOAD_DEFINITIONS_OP}' requires argument '{FhirToolArguments.ENVIRONMENT_DESTINATION_ARG}' or '{FhirToolArguments.ENVIRONMENT_DESTINATION_SHORT_ARG}'",
                    Severity = IssueSeverityEnum.Error,
                });

            }
            if (!arguments.ResourceType.HasValue)
            {
                _issues.Add(new Issue
                {
                    Details = $"Operation '{FhirToolArguments.UPLOAD_DEFINITIONS_OP}' requires argument '{FhirToolArguments.RESOURCETYPE_ARG}' or '{FhirToolArguments.RESOURCETYPE_SHORT_ARG}'",
                    Severity = IssueSeverityEnum.Error,
                });
            }
        }
    }
}