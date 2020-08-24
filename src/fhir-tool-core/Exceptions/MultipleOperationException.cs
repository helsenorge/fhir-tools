using FhirTool.Core.Operations;
using Hl7.Fhir.Utility;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FhirTool.Core
{
    [SuppressMessage("Usage", "CA2237:Mark ISerializable types with serializable")]
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors")]
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
