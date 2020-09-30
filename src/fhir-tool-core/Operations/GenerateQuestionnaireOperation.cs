extern alias R4;

using R4::Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using Tasks = System.Threading.Tasks;
using CommandLine;
using FhirTool.Core.ArgumentHelpers;

namespace FhirTool.Core.Operations
{
    [Verb("generate", HelpText = "Generates questionnaire")]
    public class GenerateQuestionnaireOperationOptions
    {
        [Option('q', "questionnaire", HelpText = "questionnaire", Required = true)]
        public WithFile Questionnaire { get; set; }

        [Option('f', "format", HelpText = "mime-type", Required = true)]
        public string MimeType { get; set; }

        [Option("excel-sheet-version", HelpText = "excel sheet version ", Required = true)]
        public ExcelSheetVersion ExcelSheetVersion { get; set; }

        [Option("skip-validation", HelpText = "skips validation")]
        public bool SkipValidation { get; set; }

        [Option('s', "valueset", HelpText = "value set path")]
        public WithFile ValueSet { get; set; }
    }

    public class GenerateQuestionnaireOperation : Operation
    {
        private readonly GenerateQuestionnaireOperationOptions _arguments;
        private readonly QuestionnaireGenerator _generator;
        private readonly ILogger<GenerateQuestionnaireOperation> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public GenerateQuestionnaireOperation(GenerateQuestionnaireOperationOptions arguments, ILoggerFactory loggerFactory)
        {
            _arguments = arguments ?? throw new ArgumentOutOfRangeException(nameof(arguments));
            _loggerFactory = loggerFactory ?? throw new ArgumentOutOfRangeException(nameof(loggerFactory));

            _logger = loggerFactory.CreateLogger<GenerateQuestionnaireOperation>();

            ValidateInputArguments(arguments);

            _generator = new QuestionnaireGenerator(loggerFactory);
        }

        public override async Tasks.Task<OperationResultEnum> Execute()
        {
            Questionnaire questionnaire = _generator.GenerateQuestionnaireFromFlatFile(_arguments);
            if (questionnaire == null)
            {
                _issues.Add(new Issue
                {
                    Details = $"Failed to extract Questionnaire from flat file format\nLocation: '{_arguments.Questionnaire.Path}'.",
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
                var opts = new VerifyValidationItemsOptions { Questionnaire = new WithFhirFile(filename), MimeType = _arguments.MimeType };
                var verifyValidationOperation = new VerifyValidationItems(opts, _loggerFactory);
                await verifyValidationOperation.Execute();
                _issues.AddRange(verifyValidationOperation.Issues);
            }

            return _issues.Any(issue => issue.Severity == IssueSeverityEnum.Error)
                ? OperationResultEnum.Failed
                : OperationResultEnum.Succeeded;
        }

        private void ValidateInputArguments(GenerateQuestionnaireOperationOptions arguments)
        {
            arguments.Questionnaire.Validate(nameof(arguments.Questionnaire));
            arguments.ValueSet?.Validate(nameof(arguments.ValueSet));
        }
    }
}
