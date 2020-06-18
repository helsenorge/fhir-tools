using System;
using System.Diagnostics.CodeAnalysis;

namespace FhirTool.Core
{
    [SuppressMessage("Usage", "CA2237:Mark ISerializable types with serializable", Justification = "<Pending>")]
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "<Pending>")]
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
