/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FhirTool.Core
{
    internal class AuthToken
    {
        private readonly string _authority;
        private DiscoveryCache _discoveryCache;

        public AuthToken(string authority)
        {
            _authority = authority;
            _discoveryCache = new DiscoveryCache(_authority);
        }

        private async Task<DiscoveryDocumentResponse> GetDiscoveryDocument()
        {
            var disco = await _discoveryCache.GetAsync();
            if (disco.IsError) throw new Exception(disco.Error);
            
            return disco;
        }

        public async Task<TokenResponse> GetToken(string clientId, string clientSecret, string[] scopes = null)
        {
            var disco = await GetDiscoveryDocument();
            var client = new HttpClient();

            return await client.RequestTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = clientId,
                GrantType = "client_credentials",
                ClientSecret = clientSecret,
                Scope = scopes == null ? null : string.Join(' ', scopes),
            });
        }
    }
}
