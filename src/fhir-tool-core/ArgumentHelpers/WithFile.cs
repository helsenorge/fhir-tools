/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using System.IO;

namespace FhirTool.Core.ArgumentHelpers
{
    public class WithFile
    {
        public string Path { get; }

        public WithFile(string path)
        {
            Path = path;
        }

        internal void Validate(string paramName)
        {
            if (!File.Exists(Path))
            {
                throw new SemanticArgumentException($"argument must point to an existing file.", paramName);
            }

        }
    }
}
