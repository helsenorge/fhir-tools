using EnsureThat;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

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

        public HttpResponseMessage ResolveUrl(Uri uri, string credentials)
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
                if (!string.IsNullOrWhiteSpace(credentials))
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials.ToBase64());
                response = client.SendAsync(request).GetAwaiter().GetResult();
            }
            return response;
        }
    }
}
