using System;

namespace FhirTool
{
    public class MissingRequirementException : Exception
    {
        public MissingRequirementException(string message)
            : base(message)
        {

        }
    }
}
