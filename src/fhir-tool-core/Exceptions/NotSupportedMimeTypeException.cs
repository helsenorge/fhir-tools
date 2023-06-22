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
