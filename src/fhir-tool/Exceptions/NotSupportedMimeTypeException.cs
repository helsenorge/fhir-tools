using System;

namespace FhirTool
{
    public class NotSupportedMimeTypeException : Exception
    {
        public NotSupportedMimeTypeException(string mimeType)
            : base($"'{mimeType}' is not a supported MIME Type.")
        {
            MimeType = mimeType;
        }

        public string MimeType { get; protected set; }
    }
}
