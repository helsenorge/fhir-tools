using System.Linq;

namespace FhirTool.Core.Utils
{
    public static class FhirVersionUtils
    {
        public static FhirVersion? MapStringToFhirVersion(string version)
        {
            var majorNumber = version.Split('.').FirstOrDefault();
            switch (majorNumber)
            {
                case "2":
                    return FhirVersion.R2;
                case "3":
                    return FhirVersion.R3;
                case "4":
                    return FhirVersion.R4;
                case "5":
                    return FhirVersion.R5;
                default:
                    return default;
            }
        }
    }
}
