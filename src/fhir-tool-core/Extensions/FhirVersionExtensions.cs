using System.Linq;

namespace FhirTool.Core
{
    public static class FhirVersionExtensions
    {
        public static string GetFhirVersionAsString(this FhirVersion version)
        {
            return FhirConstants.KnownFhirVersions[version];
        }
    }
}
