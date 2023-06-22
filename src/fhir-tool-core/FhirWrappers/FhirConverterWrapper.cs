/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

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

        public string Convert(string content, FhirMimeType fhirMimeType)
        {
            var baseFromObject = FromSerializer.Parse(content);
            var baseToObject = _converter.Convert<Base, Base>(baseFromObject.Resource);
            return ToSerializer.Serialize(baseToObject, fhirMimeType);
        }
    }
}
