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
    public class UnknownFhirMonikerException : Exception
    {
        public UnknownFhirMonikerException(string moniker)
            : this($"'{moniker}' is not a known FHIR moniker", moniker)
        {
        }

        public UnknownFhirMonikerException(string message, string moniker) : base(message)
        {
            Moniker = moniker;
        }

        public string Moniker { get; }
    }
}