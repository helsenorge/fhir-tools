using FhirTool.Core.Operations;
using Hl7.Fhir.Utility;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FhirTool.Core
{
    [SuppressMessage("Usage", "CA2237:Mark ISerializable types with serializable", Justification = "<Pending>")]
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "<Pending>")]
    public class NotSupportedOperationException : Exception
    {
        public NotSupportedOperationException(OperationEnum operation)
            : base($"Operation is not supported. Operation: {operation.GetLiteral()}")
        {

        }
    }
}
