﻿/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FhirTool.Core.ArgumentHelpers
{
    public class WithFhirFile
    {
        public string Path { get; }

        public WithFhirFile(string path)
        {
            Path = path;
        }

        public void Validate(string paramName)
        {
            if (!(File.Exists(Path)))
            {
                throw new SemanticArgumentException($"argument must point to an actual file.", paramName);
            }

            if (!SerializationUtility.ProbeIsJsonOrXml(Path))
            {
                throw new SemanticArgumentException($"argument must refer to a file that contains either JSON or XML.", paramName);
            }
        }
    }
}
