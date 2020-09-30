using FhirTool.Core.FhirWrappers;
using Hl7.Fhir.Utility;

namespace FhirTool.Core.Utils
{
    public static class MimeTypeUtils
    {
        public static FhirMimeType? ProbeFhirMimeType(string content)
        {
            if (SerializationUtil.ProbeIsJson(content))
            {
                return FhirMimeType.Json;
            }
            if (SerializationUtil.ProbeIsXml(content))
            {
                return FhirMimeType.Xml;
            }

            return null;
        }
    }
}
