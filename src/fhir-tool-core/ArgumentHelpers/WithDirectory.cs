using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
