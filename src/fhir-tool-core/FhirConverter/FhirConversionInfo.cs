extern alias R3;
extern alias R4;

using FhirTool.Core;

namespace FhirTool.Conversion
{
    public class FhirConversionInfo
    {
        public FhirVersion FromVersion { get; set; }
        public FhirVersion ToVersion { get; set; }
    }
}
