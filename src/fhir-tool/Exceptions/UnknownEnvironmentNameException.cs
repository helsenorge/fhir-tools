using System;

namespace FhirTool
{
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
