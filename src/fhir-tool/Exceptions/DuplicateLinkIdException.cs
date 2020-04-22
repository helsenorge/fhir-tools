using System;
using System.Diagnostics.CodeAnalysis;

namespace FhirTool
{
    [SuppressMessage("Usage", "CA2237:Mark ISerializable types with serializable", Justification = "<Pending>")]
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "<Pending>")]
    public class DuplicateLinkIdException : Exception
    {
        public DuplicateLinkIdException(string linkId) : base($"Duplicate LinkId detected. LinkId: {linkId}.")
        {
            LinkId = linkId;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public DuplicateLinkIdException(string message, string linkId) : base(message)
        {
            LinkId = linkId;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public DuplicateLinkIdException(string message, string linkId, Exception inner) : base(message, inner)
        {
            LinkId = linkId;
        }

        public string LinkId { get;  }
    }
}
