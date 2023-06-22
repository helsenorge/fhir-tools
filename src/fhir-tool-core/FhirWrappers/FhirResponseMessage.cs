/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

namespace FhirTool.Core.FhirWrappers
{
    public class FhirResponseMessage
    {
        private readonly int _statusCode;
        private readonly string _content;
        private readonly string _mediaType;
        private FhirMimeType? _mimeType;

        public FhirResponseMessage(int statusCode, string mediaType, string content)
        {
            _statusCode = statusCode;
            _content = content;
            _mediaType = mediaType;
            switch (mediaType)
            {
                case "application/json":
                case "application/fhir+json":
                case "application/json+fhir":
                    _mimeType = FhirMimeType.Json;
                    break;
                case "application/xml":
                case "application/fhir+xml":
                case "application/xml+fhir":
                    _mimeType = FhirMimeType.Xml;
                    break;
                //default:
                //    throw new UnknownMimeTypeException(mediaType);
            }
        }

        public int StatusCode => _statusCode;
        
        public string Content => _content;

        public string MediaType => _mediaType;

        public FhirMimeType? MimeType => _mimeType;
    }
}
