using Hl7.Fhir.Utility;
using System;

namespace FhirTool
{
    public class MultipleOperationException : Exception
    {
        public MultipleOperationException(OperationEnum operation)
            : base($"Multiple operations are not allowed. Second operation detected: {operation.GetLiteral()}")
        {
            Operation = operation;
        }

        public OperationEnum Operation { get; protected set; }
    }
}
