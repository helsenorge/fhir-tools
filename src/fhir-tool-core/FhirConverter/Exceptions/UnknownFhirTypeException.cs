/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using System;

namespace FhirTool.Conversion
{
    internal class UnknownFhirTypeException : Exception
    {
        public UnknownFhirTypeException(Type type)
            : base($"The FHIR type '{type.FullName}' is unknown.")
        {
            Type = type;
        }

        public UnknownFhirTypeException(string message, Type type) : base(message)
        {
            Type = type;
        }

        public UnknownFhirTypeException(Type type, FhirPath path) 
            : base($"The FHIR type {type.FullName} is unknown. Found at {path.GetFullPath()}")
        {
            Type = type;
            Path = path;
        }

        public Type Type { get; }
        public FhirPath Path { get; }
    }
}