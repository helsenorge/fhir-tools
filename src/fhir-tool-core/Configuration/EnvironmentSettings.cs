namespace FhirTool.Core.Configuration
{
    public class EnvironmentSettings
    {
        public string Name { get; set; }
        public string ProxyBaseUrl { get; set; }
        public string FhirBaseUrl { get; set; }
        public string AuthorizationUrl { get; set; }
    }
}
