using FhirTool.Configuration;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using FhirTool.Core.ArgumentHelpers;

namespace FhirTool
{
    internal class EnvironmentHelper
    {
        public static IEnumerable<FhirEnvironment> LoadEnvironments()
        {
            var section = (EnvironmentSection)ConfigurationManager.GetSection($"environmentSection");
            return section.Items.Items.Select(it => new FhirEnvironment
            {
                Name = it.Name,
                FhirBaseUrl = it.FhirBaseUrl,
                ProxyBaseUrl = it.ProxyBaseUrl
            });
        }
    }
}
