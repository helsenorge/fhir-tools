/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using System.Net.Http;

namespace FhirTool.Core.FhirWrappers
{
    public class FhirRequestMessage
    {
        private readonly HttpMethod _method;
        private readonly ResourceWrapper _resource;
        private readonly FhirMimeType _mimeType;
        private readonly string _mediaType;
        private readonly SerializationWrapper _serializer;
        private string _content = null;

        public FhirRequestMessage(HttpMethod method, ResourceWrapper resource, FhirMimeType mimeType = FhirMimeType.Json)
        {
            _method = method;
            _resource = resource;
            _mimeType = mimeType;
            _mediaType = mimeType == FhirMimeType.Json 
                ? "application/fhir+json"
                : "application/fhir+xml";
            _serializer = new SerializationWrapper(resource.FhirVersion);
        }

        public string GetContentAsString()
        {
            if (string.IsNullOrEmpty(_content))
                _content = _serializer.Serialize(_resource, _mimeType);
            return _content;
        }

        public HttpMethod Method => _method;

        public ResourceWrapper Resource => _resource;

        public FhirVersion FhirVersion => _resource.FhirVersion;

        public string MediaType => _mediaType;

        public FhirMimeType MimeType => _mimeType;
    }
}
