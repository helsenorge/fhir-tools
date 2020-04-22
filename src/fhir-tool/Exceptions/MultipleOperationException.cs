using Hl7.Fhir.Utility;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FhirTool
{
    [SuppressMessage("Usage", "CA2237:Mark ISerializable types with serializable", Justification = "<Pending>")]
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "<Pending>")]
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
