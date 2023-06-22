/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using System.IO;

namespace FhirTool.Core.ArgumentHelpers
{
    public class WithDirectory
    {
        public string Path { get; }

        public WithDirectory(string path)
        {
            Path = path;
        }

        public void Validate(string paramName)
        {
            if(Path != null && !(Directory.Exists(Path)))
            {
                throw new SemanticArgumentException($"argument must point to an actual directory.", paramName);
            }
        }
    }
}
