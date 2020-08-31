using System;

namespace FhirTool.Core.ArgumentHelpers
{
    public class SemanticArgumentException : Exception
    {
        public string Parameter { get; }

        public SemanticArgumentException(string message, string parameter) : base(message)
        {
            Parameter = parameter;
        }
    }
}
