using System;

namespace FhirTool.Conversion
{
    public class FhirConversionException : Exception
    {
        public FhirConversionException(string message) : base(message)
        {
        }

        public FhirConversionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
