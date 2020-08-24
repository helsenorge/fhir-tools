using System;
using System.Runtime.Serialization;

namespace FhirTool.Core.Operations
{
    [Serializable]
    internal class UnknownOperationException : Exception
    {
        public UnknownOperationException(OperationEnum operation)
            : this($"Operation: '{operation}' is unknown.", operation)
        {
        }

        public UnknownOperationException(string message, OperationEnum operation) : base(message)
        {
            Operation = operation;
        }

        public UnknownOperationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnknownOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public OperationEnum Operation { get; }
    }
}