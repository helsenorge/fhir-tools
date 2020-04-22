using System;
using System.Text;

namespace FhirTool.Extensions
{
    internal static class StringExtensions
    {
        internal static string UpperCaseFirstCharacter(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return $"{s.Substring(0, 1).ToUpper()}{s.Substring(1)}";
        }

        internal static string ToBase64(this string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            return Convert.ToBase64String(bytes);
        }
    }
}
