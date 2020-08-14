using System;
using System.Diagnostics.CodeAnalysis;

namespace FhirTool.Conversion
{
    [SuppressMessage("Usage", "CA2237:Mark ISerializable types with serializable")]
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors")]
    internal class UnknownFhirTypeException : Exception
    {
        public UnknownFhirTypeException(Type type)
            : base($"The FHIR type '{type.FullName}' is unknown.")
        {
            Type = type;
        }

        public UnknownFhirTypeException(string message, Type type) : base(message)
        {
            Type = type;
        }

        public UnknownFhirTypeException(Type type, FhirPath path) 
            : base($"The FHIR type {type.FullName} is unknown. Found at {path.GetFullPath()}")
        {
            Type = type;
            Path = path;
        }

        public Type Type { get; }
        public FhirPath Path { get; }
    }
}