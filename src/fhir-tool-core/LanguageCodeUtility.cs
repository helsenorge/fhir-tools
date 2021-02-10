/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

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
