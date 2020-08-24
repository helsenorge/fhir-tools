namespace FhirTool.Core
{
    internal static class LanguageCodeUtility
    {
        public static string GetLanguageCode(string languageAndCountryCode)
        {
            string languageCode = languageAndCountryCode;
            int index = languageCode.IndexOf("-");
            if (index > 0)
            {
                languageCode = languageCode.Substring(0, index);
            }

            return languageCode;
        }
    }
}
