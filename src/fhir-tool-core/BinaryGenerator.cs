extern alias R4;
using R4::Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using System.IO;

namespace FhirTool.Core.Operations
{
    internal class BinaryGenerator
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        public BinaryGenerator(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<BinaryGenerator>();
        }       

        public Binary GenerateBinary(GenerateBinaryOperationOptions arguments)
        {            
            _logger.LogInformation($"Loading file: '{arguments.File}'.");

            var bytes = File.ReadAllBytes(arguments.File.Path);
            
            return new Binary
            {
                Id = arguments?.Id,
                ContentType = arguments?.ContentType,
                SecurityContext = new ResourceReference
                {
                    Reference = arguments?.SecurityContext
                },
                Data = bytes
            };
            
        }
    }
}
