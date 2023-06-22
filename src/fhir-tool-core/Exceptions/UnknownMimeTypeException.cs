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