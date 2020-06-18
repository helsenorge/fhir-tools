using System;

namespace FhirTool.Core
{
    public class FhirVersionNotSupportedException : Exception
    {
        public FhirVersionNotSupportedException(FhirVersion version)
            : base($"Fhir version '{version.GetFhirVersionAsString()}' is not supported.")
        {
            Version = version;
        }
        public FhirVersionNotSupportedException(FhirVersion version, string message) 
            : base(message)
        {
            Version = version;
        }

        public FhirVersionNotSupportedException(FhirVersion version, string message, Exception innerException) 
            : base(message, innerException)
        {
            Version = version;
        }

        public FhirVersion Version { get; }
    }
}
