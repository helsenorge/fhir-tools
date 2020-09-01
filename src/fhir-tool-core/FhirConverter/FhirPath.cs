using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FhirTool.Conversion
{
    public class FhirPath
    {
        private Stack<string> Path { get; } = new Stack<string>();

        public string GetFullPath()
        {
            return string.Join(".", Path.Reverse());
        }

        public void Push(string pathElem)
        {
            Path.Push(pathElem);
        }

        public void Pop()
        {
            Path.Pop();
        }

        public override string ToString()
        {
            return GetFullPath();
        }
    }
}
