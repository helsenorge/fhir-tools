using System;
using System.Diagnostics.CodeAnalysis;

namespace FhirTool.Core
{
    [SuppressMessage("Usage", "CA2237:Mark ISerializable types with serializable", Justification = "<Pending>")]
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "<Pending>")]
    internal class UnknownMimeTypeException : Exception
    {
        public UnknownMimeTypeException(string mimeType)
            : this(mimeType, $"Mime Type: '{mimeType}' is unknown.")
        {
        }

        public UnknownMimeTypeException(string mimeType, string message) : base(message)
        {
            MimeType = mimeType;
        }

        public string MimeType { get; }
    }
}