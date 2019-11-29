using System;

namespace FhirTool
{
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
