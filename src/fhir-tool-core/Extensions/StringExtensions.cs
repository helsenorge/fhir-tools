/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using System;
using System.Text;

namespace FhirTool.Core
{
    public static class StringExtensions
    {
        public static string UpperCaseFirstCharacter(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;
            return $"{s.Substring(0, 1).ToUpper()}{s.Substring(1)}";
        }

        public static string ToBase64(this string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            return Convert.ToBase64String(bytes);
        }

        public static string AppendCharToEndOfStringIfMissing(this string s, char c)
        {
            if (s.LastIndexOf(c) != s.Length - 1)
            {
                s += c;
            }
            return s;
        }
    }
}
