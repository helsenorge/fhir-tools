using System;
using System.Diagnostics.CodeAnalysis;

namespace FhirTool.Core
{
    [SuppressMessage("Usage", "CA2237:Mark ISerializable types with serializable", Justification = "<Pending>")]
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "<Pending>")]
    public class MissingRequirementException : Exception
    {
        public MissingRequirementException(string message)
            : base(message)
        {

        }
    }
}
