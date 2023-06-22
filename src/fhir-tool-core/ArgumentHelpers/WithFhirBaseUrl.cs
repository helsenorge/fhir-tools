/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using EnsureThat;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace FhirTool.Core.ArgumentHelpers
{
    public class WithFhirBaseUrl
    {
        public string Uri { get; set;  }

        public WithFhirBaseUrl() { }

        public WithFhirBaseUrl(string uri)
        {
            Uri = uri;
        }

        public void Validate(string paramName, bool resolveUrl, string credentials)
        {
            HttpResponseMessage responseMessage = ResolveUrl(new Uri(Uri), credentials);
            if (resolveUrl && !responseMessage.IsSuccessStatusCode)
            {
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new SemanticArgumentException($"Unable to resolve url (value: {Uri}).", paramName);
                }
            }
        }

        public HttpResponseMessage ResolveUrl(Uri uri, string token)
        {
            EnsureArg.IsNotNull(uri, nameof(uri));

            HttpResponseMessage response = null;
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Head,
                RequestUri = new Uri($"{uri.Scheme}://{uri.Host}")
            };

            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrWhiteSpace(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = client.SendAsync(request).GetAwaiter().GetResult();
            }
            return response;
        }
    }
}
