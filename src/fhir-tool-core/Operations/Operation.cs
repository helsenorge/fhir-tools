/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using FhirTool.Core.ArgumentHelpers;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FhirTool.Core.Operations
{
    public abstract class Operation : IOperation
    {
        protected readonly List<Issue> _issues = new List<Issue>();

        public virtual IEnumerable<Issue> Issues => _issues;

        public abstract Task<OperationResultEnum> Execute();

        protected async Task<TokenResponse> GetToken(string authority, string credentials, string fallbackAuthority)
        {
            if(string.IsNullOrEmpty(authority) && string.IsNullOrEmpty(fallbackAuthority))
            {
                throw new ArgumentException($"Argument '{nameof(authority)}' or '{nameof(fallbackAuthority)}' must be set.");
            }
            
            return await GetToken(string.IsNullOrEmpty(authority) ? fallbackAuthority : authority, credentials);
        }

        protected async Task<TokenResponse> GetToken(string authority, string credentials)
        {
            if (string.IsNullOrEmpty(authority)) throw new ArgumentNullException(nameof(authority));
            if (string.IsNullOrEmpty(credentials)) throw new ArgumentNullException(nameof(credentials));

            var creds = credentials.Split(':');
            if (creds.Length != 2) return null;

            var clientId = creds[0];
            var secret = creds[1];
            AuthToken authToken = new AuthToken(authority);

            return await authToken.GetToken(clientId, secret);
        }

        protected void ValidateResourceType(string resourceType)
        {
            var validResourceTypes = new List<string>
            {
                "Endpoint",
                "Questionnaire",
                "DocumentReference",
                "Binary",
            };
            if (!validResourceTypes.Contains(resourceType))
            {
                var known = string.Join(", ", validResourceTypes.AsEnumerable());
                throw new SemanticArgumentException($"Resource Type {resourceType} is not a known Resource Type. Known Resource Types are {known}", "ResourceType");
            }
        }
    }
}
