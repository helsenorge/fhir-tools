/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

extern alias R4;
using R4::Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using System.IO;
using Hl7.Fhir.Model;

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
            _logger.LogInformation($"Loading file: '{arguments.Path}'.");

            var bytes = File.ReadAllBytes(arguments.Path.Path);
            
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
