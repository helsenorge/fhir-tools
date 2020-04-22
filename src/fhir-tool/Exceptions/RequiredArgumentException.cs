using System;
using System.Diagnostics.CodeAnalysis;

namespace FhirTool
{
    [SuppressMessage("Usage", "CA2237:Mark ISerializable types with serializable", Justification = "<Pending>")]
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "<Pending>")]
    public class RequiredArgumentException : Exception
    {
        public RequiredArgumentException(string argument)
            : base($"'{argument}' is required and must be set.")
        {
            Argument = argument;
        }
        public RequiredArgumentException(string message, string argument)
            : base(message)
        {
            Argument = argument;
        }

        public string Argument { get; protected set; }
    }
}
