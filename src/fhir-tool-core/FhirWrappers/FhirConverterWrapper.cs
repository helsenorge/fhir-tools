using FhirTool.Conversion;
using Hl7.Fhir.Model;

namespace FhirTool.Core.FhirWrappers
{
    public class FhirConverterWrapper
    {
        private FhirConverter _converter;

        public SerializationWrapper ToSerializer { get; }
        public SerializationWrapper FromSerializer { get; }

        public FhirConverterWrapper(FhirVersion to, FhirVersion from)
        {
            _converter = new FhirConverter(to, from);

            ToSerializer = new SerializationWrapper(to);
            FromSerializer = new SerializationWrapper(from);
        }

        public string Convert(string content)
        {
            var baseFromObject = FromSerializer.Parse(content);
            var baseToObject = _converter.Convert<Base, Base>(baseFromObject.ToBase());
            return ToSerializer.Serialize(baseToObject);
        }
    }
}
