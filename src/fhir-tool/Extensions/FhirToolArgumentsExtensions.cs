using EnsureThat;
using System;
using System.IO;
using System.Net.Http;

namespace FhirTool.Extensions
{
    internal static class FhirToolArgumentsExtensions
    {
        public static bool Validate(this FhirToolArguments arguments)
        {
            bool validated = true;
            
            if (!string.IsNullOrEmpty(arguments.ValueSetPath))
            {
                if (!File.Exists(arguments.ValueSetPath))
                {
                    Logger.ErrorWriteLineToOutput($"File not found: '{arguments.ValueSetPath}'");
                    validated = false;
                }
            }
            if (arguments.Operation == OperationEnum.Generate && string.IsNullOrEmpty(arguments.Version))
            {
                Logger.ErrorWriteLineToOutput($"Argument {FhirToolArguments.VERSION_ARG} | {FhirToolArguments.VERSION_SHORT_ARG} is required.");
                validated = false;
            }
            if (!string.IsNullOrEmpty(arguments.FhirBaseUrl))
            {
                HttpResponseMessage responseMessage = arguments.ResolveUrl(new Uri(arguments.FhirBaseUrl));
                if (arguments.ResolveUrl && !responseMessage.IsSuccessStatusCode)
                {
                    if (responseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Logger.ErrorWriteLineToOutput($"Could not resolve url: {arguments.FhirBaseUrl}");
                        validated = false;
                    }
                    else
                    {
                        Logger.WarnWriteLineToOutput($"The status Code: '{responseMessage.StatusCode}' was returned when running the --resolve-url operation.");
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
                if (!string.IsNullOrEmpty(arguments.Credentials))
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", arguments.Credentials.ToBase64());
                response = client.SendAsync(request).GetAwaiter().GetResult();
            }
            return response;
        }
    }
}
