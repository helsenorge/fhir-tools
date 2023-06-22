/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using System.Collections.Generic;

namespace FhirTool.Core
{
    public static class KeyExtensions
    {
        public static IEnumerable<string> GetSegments(this IKey key)
        {
            if (key.Base != null) yield return key.Base;
            if (key.TypeName != null) yield return key.TypeName;
            if (key.ResourceId != null) yield return key.ResourceId;
            if (key.VersionId != null)
            {
                yield return "_history";
                yield return key.VersionId;
            }
        }

        public static string ToUriString(this IKey key)
        {
            var segments = key.GetSegments();
            return string.Join("/", segments);
        }
    }
}
