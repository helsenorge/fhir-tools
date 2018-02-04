using System;

namespace FhirTool
{
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

        public string LinkId { get; protected set; }
    }
}
