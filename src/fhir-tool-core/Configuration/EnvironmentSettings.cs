/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

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
