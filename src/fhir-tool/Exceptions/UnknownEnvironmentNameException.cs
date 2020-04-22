using System;
using System.Diagnostics.CodeAnalysis;

namespace FhirTool
{
    [SuppressMessage("Usage", "CA2237:Mark ISerializable types with serializable", Justification = "<Pending>")]
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "<Pending>")]
    public class UnknownEnvironmentNameException : Exception
    {
        public UnknownEnvironmentNameException(string name)
            : base($"Environment '{name}' is unknown.")
        {
            Name = name;
        }
        public UnknownEnvironmentNameException(string message, string name)
            : base(message)
        {
            Name = name;
        }

        public string Name { get; protected set; }
    }
}
