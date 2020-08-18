using EnsureThat;
using FhirTool.Core;
using FhirTool.Core.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;

namespace FhirTool
{
    internal static class FhirToolArgumentsExtensions
    {
        internal static ILogger Logger;

        public static bool Validate(this FhirToolArguments arguments)
        {
            bool validated = true;
            
            if (!string.IsNullOrWhiteSpace(arguments.ValueSetPath))
            {
                if (!File.Exists(arguments.ValueSetPath))
                {
                    Logger.LogError($"File not found: '{arguments.ValueSetPath}'");
                    validated = false;
                }
            }
            if (arguments.Operation == OperationEnum.GenerateResource && string.IsNullOrWhiteSpace(arguments.Version))
            {
                Logger.LogError($"Argument {FhirToolArguments.VERSION_ARG} | {FhirToolArguments.VERSION_SHORT_ARG} is required.");
                validated = false;
            }
            if (!string.IsNullOrWhiteSpace(arguments.FhirBaseUrl))
            {
                HttpResponseMessage responseMessage = arguments.ResolveUrl(new Uri(arguments.FhirBaseUrl));
                if (arguments.ResolveUrl && !responseMessage.IsSuccessStatusCode)
                {
                    if (responseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Logger.LogError($"Could not resolve url: {arguments.FhirBaseUrl}");
                        validated = false;
                    }
                    else
                    {
                        Logger.LogWarning($"The status Code: '{responseMessage.StatusCode}' was returned when running the --resolve-url operation.");
                    }
                }
            }

            return validated;
        }

        public static HttpResponseMessage ResolveUrl(this FhirToolArguments arguments, Uri uri)
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
                if (!string.IsNullOrWhiteSpace(arguments.Credentials))
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", arguments.Credentials.ToBase64());
                response = client.SendAsync(request).GetAwaiter().GetResult();
            }
            return response;
        }
    }
}
