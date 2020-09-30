using System.Collections.Generic;

namespace FhirTool.Core
{
    internal static class FhirConstants
    {
        public const string VERSION_R2 = "1.0";
        public const string VERSION_R3 = "3.0";
        public const string VERSION_R4 = "4.0";
        public const string VERSION_R5 = "4.4";

        public static Dictionary<FhirVersion, string> KnownFhirVersions = new Dictionary<FhirVersion, string>
        {
            { FhirVersion.None, string.Empty },
            { FhirVersion.R2, VERSION_R2 },
            { FhirVersion.R3, VERSION_R3 },
            { FhirVersion.R4, VERSION_R4 },
            { FhirVersion.R5, VERSION_R5 },
        };
    }
}
