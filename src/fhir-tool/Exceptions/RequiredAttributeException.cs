using System;

namespace FhirTool
{
    public class RequiredAttributeException : Exception
    {
        public RequiredAttributeException(string attribute)
            : base($"'{attribute}' is required and must be set.")
        {
            Attribute = attribute;
        }
        public RequiredAttributeException(string message, string attribute)
            : base(message)
        {
            Attribute = attribute;
        }

        public string Attribute { get; protected set; }
    }
}
