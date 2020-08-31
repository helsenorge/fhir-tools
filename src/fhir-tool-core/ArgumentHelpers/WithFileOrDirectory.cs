using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
                throw new SemanticArgumentException($"argument must point to a existing file or directory.", paramName);
            }
        }
    }
}
