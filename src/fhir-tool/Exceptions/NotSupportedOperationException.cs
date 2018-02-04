using Hl7.Fhir.Utility;
using System;

namespace FhirTool
{
    public class NotSupportedOperationException : Exception
    {
        public NotSupportedOperationException(OperationEnum operation)
            : base($"Operation is not supported. Operation: {operation.GetLiteral()}")
        {

        }
    }
}
