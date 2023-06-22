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
    public class WithFileOrDirectory
    {
        public string Path { get; }

        public WithFileOrDirectory(string path)
        {
            Path = path;
        }

        public void Validate(string paramName)
        {
            if (!(Directory.Exists(Path) || File.Exists(Path)))
            {
                throw new SemanticArgumentException($"argument must point to a existing file or directory: {Path}.", paramName);
            }
        }

        public bool IsDirectory()
        {
            return Directory.Exists(Path);
        }

        public bool IsFile()
        {
            return File.Exists(Path);
        }
    }
}
