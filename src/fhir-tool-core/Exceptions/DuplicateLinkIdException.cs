/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using System;

namespace FhirTool.Core
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

        public string LinkId { get;  }
    }
}
