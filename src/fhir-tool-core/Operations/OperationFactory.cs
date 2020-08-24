using Microsoft.Extensions.Logging;
using System;

namespace FhirTool.Core.Operations
{
    public class OperationFactory
    {
        private readonly FhirToolArguments _arguments;
        private readonly ILoggerFactory _loggerFactory = new LoggerFactory();

        public OperationFactory(FhirToolArguments arguments, ILoggerFactory loggerFactory)
        {
            _arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public IOperation Create(OperationEnum operation)
        {
            return Create(operation, _arguments);
        }

        internal IOperation Create(OperationEnum operation, FhirToolArguments arguments)
        {
            return operation switch
            {
                OperationEnum.GenerateResource => 
                    new GenerateQuestionnaireOperation(
                        arguments: arguments,
                        generator: new QuestionnaireGenerator(
                            proxyBaseUrl: arguments.ProxyBaseUrl,
                            logger: _loggerFactory.CreateLogger<QuestionnaireGenerator>()),
                        logger: _loggerFactory.CreateLogger<GenerateQuestionnaireOperation>(),
                        operationFactory: this
                    ),
                OperationEnum.UploadResource => 
                    new UploadResourceOperation(
                        arguments: arguments,
                        logger: _loggerFactory.CreateLogger<UploadResourceOperation>()
                    ),
                OperationEnum.UploadFhirDefinitions => 
                    new UploadDefinitionsOperation(
                        arguments: arguments,
                        logger: _loggerFactory.CreateLogger<UploadDefinitionsOperation>()
                    ),
                OperationEnum.BundleResources => 
                    new BundleResourcesOperation(
                        arguments: arguments,
                        logger: _loggerFactory.CreateLogger<BundleResourcesOperation>()
                    ),
                OperationEnum.SplitBundle => 
                    new SplitBundleOperation(
                        arguments: arguments,
                        logger: _loggerFactory.CreateLogger<SplitBundleOperation>()
                    ),
                OperationEnum.TransferData => 
                    new TransferDataOperation(
                        arguments: arguments,
                        logger: _loggerFactory.CreateLogger<TransferDataOperation>()
                    ),
                OperationEnum.VerifyValidation => 
                    new VerifyValidationItems(
                        arguments: arguments,
                        logger: _loggerFactory.CreateLogger<VerifyValidationItems>()
                    ),
                OperationEnum.Convert => 
                    new ConvertOperation(
                        arguments: arguments,
                        logger: _loggerFactory.CreateLogger<ConvertOperation>()
                    ),
                _ => throw new UnknownOperationException(operation),
            };
        }
    }
}
