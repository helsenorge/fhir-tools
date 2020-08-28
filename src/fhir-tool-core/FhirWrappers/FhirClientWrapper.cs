extern alias R3;
extern alias R4;

using FhirTool.Core.Utils;
using Microsoft.Extensions.Logging;
using R3Rest = R3::Hl7.Fhir.Rest;
using R4Rest = R4::Hl7.Fhir.Rest;

namespace FhirTool.Core.FhirWrappers
{
    internal class FhirClientWrapper
    {
        public FhirVersion FhirVersion { get; }
        private R3Rest.FhirClient R3Client;
        private R4Rest.FhirClient R4Client;

        private ILogger _logger;

        internal FhirClientWrapper(string endpoint, ILogger logger, FhirVersion? fhirVersion = null)
        {
            _logger = logger;

            if(fhirVersion == null)
            {
                R3Client = new R3Rest.FhirClient(endpoint);
                var meta = R3Client.CapabilityStatement(R3Rest.SummaryType.Text);
                fhirVersion = FhirVersionUtils.MapStringToFhirVersion(meta.FhirVersion);
            }

            FhirVersion = fhirVersion.Value;
            switch(FhirVersion) {
                case FhirVersion.R4:
                    R4Client = R4Client ?? new R4Rest.FhirClient(endpoint);
                    R4Client.OnBeforeRequest += R4Client_OnBeforeRequest;
                    break;
                case FhirVersion.R3:
                    R3Client = R3Client ?? new R3Rest.FhirClient(endpoint);
                    R3Client.OnBeforeRequest += R3Client_OnBeforeRequest;
                    break;
            }
        }

        private void R3Client_OnBeforeRequest(object sender, R3Rest.BeforeRequestEventArgs e)
        {
            _logger.LogInformation($"{e.RawRequest.Method} {e.RawRequest.Address}");
        }

        private void R4Client_OnBeforeRequest(object sender, R4Rest.BeforeRequestEventArgs e)
        {
            _logger.LogInformation($"{e.RawRequest.Method} {e.RawRequest.Address}");
        }

        internal BundleWrapper Search(string resource)
        {
            switch (FhirVersion) {
                case FhirVersion.R3:
                    var resultR3 = R3Client.Search(resource);
                    return resultR3 != null ? new BundleWrapper(resultR3) : null;
                case FhirVersion.R4:
                    var resultR4 = R4Client.Search(resource);
                    return resultR4 != null ? new BundleWrapper(resultR4) : null;
                default:
                    return default;
            }
        }

        internal BundleWrapper Continue(BundleWrapper bundleWrapper)
        {
            switch(FhirVersion)
            {
                case FhirVersion.R3:
                    var resultR3 = R3Client.Continue(bundleWrapper.R3Bundle);
                    return resultR3 != null ? new BundleWrapper(resultR3) : null;
                case FhirVersion.R4:
                    var resultR4 = R4Client.Continue(bundleWrapper.R4Bundle);
                    return resultR4 != null ? new BundleWrapper(resultR4) : null;
                default:
                    return default;
            }
        }
    }
}
