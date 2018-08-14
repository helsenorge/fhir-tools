using System;
using System.IO;
using System.Net.Http;

namespace FhirTool.Extensions
{
    public static class FhirToolArgumentsExtensionMethods
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
                if (arguments.ResolveUrl && !arguments.ResolveUrl(new Uri(arguments.FhirBaseUrl)).IsSuccessStatusCode)
                {
                    Logger.ErrorWriteLineToOutput($"Could not resolve url: {arguments.FhirBaseUrl}");
                    validated = false;
                }
            }

            return validated;
        }

        public static HttpResponseMessage ResolveUrl(this FhirToolArguments arguments, Uri uri)
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Head,
                RequestUri = new Uri($"{uri.Scheme}://{uri.Host}")
            };
            using (HttpClient client = new HttpClient())
                return client.SendAsync(request).GetAwaiter().GetResult();
        }
    }
}
