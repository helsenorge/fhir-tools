namespace FhirTool.Extensions
{
    internal static class StringExtensionMethods
    {
        internal static string UpperCaseFirstCharacter(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return $"{s.Substring(0, 1).ToUpper()}{s.Substring(1)}";
        }
    }
}
