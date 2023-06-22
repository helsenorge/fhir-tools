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
    public class UnknownEnvironmentNameException : Exception
    {
        public UnknownEnvironmentNameException(string name)
            : base($"Environment '{name}' is unknown.")
        {
            Name = name;
        }
        public UnknownEnvironmentNameException(string message, string name)
            : base(message)
        {
            Name = name;
        }

        public string Name { get; protected set; }
    }
}
