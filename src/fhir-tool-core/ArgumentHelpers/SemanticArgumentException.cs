/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using System;

namespace FhirTool.Core.ArgumentHelpers
{
    public class SemanticArgumentException : Exception
    {
        public string Parameter { get; }

        public SemanticArgumentException(string message, string parameter) : base(message)
        {
            Parameter = parameter;
        }
    }
}
