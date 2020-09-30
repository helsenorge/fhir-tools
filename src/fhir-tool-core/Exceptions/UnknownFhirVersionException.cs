using System;
using System.Diagnostics.CodeAnalysis;

namespace FhirTool.Core
{ 
    [SuppressMessage("Usage", "CA2237:Mark ISerializable types with serializable", Justification = "<Pending>")]
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "<Pending>")]
    public class UnknownFhirMonikerException : Exception
    {
        public UnknownFhirMonikerException(string moniker)
            : this($"'{moniker}' is not a known FHIR moniker", moniker)
        {
        }

        public UnknownFhirMonikerException(string message, string moniker) : base(message)
        {
            Moniker = moniker;
        }

        public string Moniker { get; }
    }
}