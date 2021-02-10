/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using System;

namespace FhirTool.Conversion
{
    public class FhirConversionException : Exception
    {
        public FhirConversionException(string message) : base(message)
        {
        }

        public FhirConversionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
