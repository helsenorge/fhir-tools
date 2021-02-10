/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using System;

namespace FhirTool.Core
{
    public class FhirVersionNotSupportedException : Exception
    {
        public FhirVersionNotSupportedException(FhirVersion version)
            : base($"Fhir version '{version.GetFhirVersionAsString()}' is not supported.")
        {
            Version = version;
        }
        public FhirVersionNotSupportedException(FhirVersion version, string message) 
            : base(message)
        {
            Version = version;
        }

        public FhirVersionNotSupportedException(FhirVersion version, string message, Exception innerException) 
            : base(message, innerException)
        {
            Version = version;
        }

        public FhirVersion Version { get; }
    }
}
