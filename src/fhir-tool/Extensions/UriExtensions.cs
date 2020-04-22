using System;

namespace FhirTool.Extensions
{
    internal static class UriExtensions
    {
        public static bool IsHttpScheme(this Uri uri)
        {
            if (uri != null)
            {
                if (uri.IsAbsoluteUri)
                {
                    return (uri.Scheme == Uri.UriSchemeHttp) || (uri.Scheme == Uri.UriSchemeHttps);
                }

            }
            return false;
        }
    }
}
