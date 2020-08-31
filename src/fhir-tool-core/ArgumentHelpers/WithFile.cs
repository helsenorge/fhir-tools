using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
