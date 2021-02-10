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
    public class RequiredArgumentException : Exception
    {
        public RequiredArgumentException(string argument)
            : base($"'{argument}' is required and must be set.")
        {
            Argument = argument;
        }
        public RequiredArgumentException(string message, string argument)
            : base(message)
        {
            Argument = argument;
        }

        public string Argument { get; protected set; }
    }
}
