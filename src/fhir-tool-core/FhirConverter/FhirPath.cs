/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using System;
using System.Collections.Generic;
using System.Linq;

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
