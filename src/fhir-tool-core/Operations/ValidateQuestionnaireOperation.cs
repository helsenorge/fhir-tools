/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

extern alias R4;

using R4::Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Tasks = System.Threading.Tasks;
using CommandLine;
using FhirTool.Core.ArgumentHelpers;
using System;

namespace FhirTool.Core.Operations
{
    [Verb("verify-validation", HelpText = "verify validation")]
    public class VerifyValidationItemsOptions
    {
        [Option('q', "questionnaire", Required = true, HelpText = "questionnaire path")]
        public WithFhirFile Questionnaire { get; set; }

        [Option('f', "format", HelpText = "mime type")]
        public string MimeType { get; set; }
    }

    public class VerifyValidationItems : Operation
    {
        private readonly VerifyValidationItemsOptions _arguments;
        private readonly ILogger<VerifyValidationItems> _logger;

        public VerifyValidationItems(VerifyValidationItemsOptions arguments, ILoggerFactory loggerFactory)
        {
            _arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            if (loggerFactory is null)
                throw new ArgumentNullException(nameof(loggerFactory));

            _logger = loggerFactory.CreateLogger<VerifyValidationItems>();

            ValidateArguments(arguments);
        }

        public override async Tasks.Task<OperationResultEnum> Execute()
        {
            _logger.LogInformation($"Deserializing Fhir resource at '{_arguments.Questionnaire.Path}'.");
            _logger.LogInformation($"Expecting format: '{_arguments.MimeType}'.");
            Questionnaire questionnaire = await SerializationUtility.DeserializeResource<Questionnaire>(_arguments.Questionnaire.Path);
            if (questionnaire == null)
            {
                _issues.Add(new Issue
                {
                    Details = $"Failed to deserialize Questionnaire from file\nLocation: '{_arguments.Questionnaire.Path}'.",
                    Severity = IssueSeverityEnum.Error,
                });
                return OperationResultEnum.Failed;
            }

            _issues.AddRange(VerifyItemValidation(questionnaire));

            return _issues.Any(issue => issue.Severity == IssueSeverityEnum.Error) 
                ? OperationResultEnum.Failed 
                : OperationResultEnum.Succeeded;
        }

        private void ValidateArguments(VerifyValidationItemsOptions arguments)
        {
            arguments.Questionnaire.Validate(nameof(arguments.Questionnaire));
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
