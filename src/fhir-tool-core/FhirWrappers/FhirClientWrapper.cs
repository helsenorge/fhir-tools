﻿/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

extern alias R3;
extern alias R4;

using FhirTool.Core.Utils;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using R3Rest = R3::Hl7.Fhir.Rest;
using R4Rest = R4::Hl7.Fhir.Rest;

namespace FhirTool.Core.FhirWrappers
{
    internal class FhirClientWrapper
    {
        public FhirVersion FhirVersion { get; }
        public R3Rest.FhirClient R3Client { get; set; }
        public R4Rest.FhirClient R4Client { get; set; }
        public string LastBodyAsText { get; private set; }
        public string Endpoint { get; private set; }

        private ILogger _logger;
        private readonly string _accessToken;

        internal FhirClientWrapper(string endpoint, ILogger logger, FhirVersion? fhirVersion = null, string accessToken = null)
        {
            Endpoint = endpoint;
            _logger = logger;
            _accessToken = accessToken;

            if (fhirVersion == FhirVersion.None)
            {
                R3Client = new R3Rest.FhirClient(endpoint);
                var meta = R3Client.CapabilityStatement(R3Rest.SummaryType.Text);
                fhirVersion = FhirVersionUtils.MapStringToFhirVersion(meta.FhirVersion);
            }

            if(fhirVersion == null)
            {
                throw new Exception("Unable to determine FhirVersion used by server. Please specify fhir version");
            }

            FhirVersion = fhirVersion.Value;
            switch(FhirVersion) {
                case FhirVersion.R4:
                    R4Client = R4Client ?? new R4Rest.FhirClient(endpoint);
                    R4Client.OnBeforeRequest += R4Client_OnBeforeRequest;
                    R4Client.OnAfterResponse += R4Client_OnAfterRequest;
                    break;
                case FhirVersion.R3:
                    R3Client = R3Client ?? new R3Rest.FhirClient(endpoint);
                    R3Client.OnBeforeRequest += R3Client_OnBeforeRequest;
                    R3Client.OnAfterResponse += R3Client_OnAfterRequest;
                    break;
            }
        }

        private void R3Client_OnBeforeRequest(object sender, R3Rest.BeforeRequestEventArgs e)
        {
            _logger.LogInformation($"{e.RawRequest.Method} {e.RawRequest.Address}");

            if (!string.IsNullOrEmpty(_accessToken))
            {
                e.RawRequest.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {_accessToken}");
            }
        }

        private void R3Client_OnAfterRequest(object sender, R3Rest.AfterResponseEventArgs e)
        {
            if (e != null && e.Body != null)
            {
                LastBodyAsText = System.Text.Encoding.UTF8.GetString(e.Body);
            }
        }

        private void R4Client_OnBeforeRequest(object sender, R4Rest.BeforeRequestEventArgs e)
        {
            _logger.LogInformation($"{e.RawRequest.Method} {e.RawRequest.Address}");

            if (!string.IsNullOrEmpty(_accessToken))
            {
                e.RawRequest.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {_accessToken}");
            }
        }

        private void R4Client_OnAfterRequest(object sender, R4Rest.AfterResponseEventArgs e)
        {
            if (e != null && e.Body != null)
            {
                LastBodyAsText = System.Text.Encoding.UTF8.GetString(e.Body);
            }
        }

        internal async Task<BundleWrapper> SearchAsync(string resource)
        {
            switch (FhirVersion) {
                case FhirVersion.R3:
                    var resultR3 = await R3Client.SearchAsync(resource);
                    return resultR3 != null ? new BundleWrapper(resultR3) : null;
                case FhirVersion.R4:
                    var resultR4 = await R4Client.SearchAsync(resource);
                    return resultR4 != null ? new BundleWrapper(resultR4) : null;
                default:
                    return default;
            }
        }

        internal async Task<BundleWrapper> ContinueAsync(BundleWrapper bundleWrapper)
        {
            switch(FhirVersion)
            {
                case FhirVersion.R3:
                    var resultR3 = await R3Client.ContinueAsync(bundleWrapper.R3Bundle);
                    return resultR3 != null ? new BundleWrapper(resultR3) : null;
                case FhirVersion.R4:
                    var resultR4 = await R4Client.ContinueAsync(bundleWrapper.R4Bundle);
                    return resultR4 != null ? new BundleWrapper(resultR4) : null;
                default:
                    return default;
            }
        }

        public async Task<BundleWrapper> TransactionAsync(BundleWrapper bundleWrapper)
        {
            switch(FhirVersion)
            {
                case FhirVersion.R3:
                    var resultR3 = await R3Client.TransactionAsync(bundleWrapper.R3Bundle);
                    return resultR3 != null ? new BundleWrapper(resultR3) : null;
                case FhirVersion.R4:
                    var resultR4 = await R4Client.TransactionAsync(bundleWrapper.R4Bundle);
                    return resultR4 != null ? new BundleWrapper(resultR4) : null;
                default:
                    return default;
            }
        }

        public async Task<ResourceWrapper> CreateAsync(ResourceWrapper resourceWrapper)
        {
            // Using the FhirClient for Binary resources is causing a 
            // 415 Unsupported Media Type on our API Management Proxy in Azure
            // The reason seems to be that the FhirClient converts the Binary 
            // xml/json resource to its actual binary representation
            if (resourceWrapper.ResourceType == ResourceTypeWrapper.Binary)
            {
                return await CreateUsingHttpClientAsync(resourceWrapper);
            }

            switch (FhirVersion)
            {
                case FhirVersion.R3:
                    var resultR3 = await R3Client.CreateAsync(resourceWrapper.R3Resource);
                    return resultR3 != null ? new ResourceWrapper(resultR3) : null;
                case FhirVersion.R4:
                    var resultR4 = await R4Client.CreateAsync(resourceWrapper.R4Resource);
                    return resultR4 != null ? new ResourceWrapper(resultR4) : null;
                default:
                    return default;
            }
        }

        public async Task<ResourceWrapper> UpdateAsync(ResourceWrapper resourceWrapper)
        {
            // Using the FhirClient for Binary resources is causing a 
            // 415 Unsupported Media Type on our API Management Proxy in Azure
            // The reason seems to be that the FhirClient converts the Binary 
            // xml/json resource to its actual binary representation
            if (resourceWrapper.ResourceType == ResourceTypeWrapper.Binary)
            {
                return await UpdateUsingHttpClientAsync(resourceWrapper);
            }

            switch (FhirVersion)
            {
                case FhirVersion.R3:
                    var resultR3 = await R3Client.UpdateAsync(resourceWrapper.R3Resource);
                    return resultR3 != null ? new ResourceWrapper(resultR3) : null;
                case FhirVersion.R4:
                    var resultR4 = await R4Client.UpdateAsync(resourceWrapper.R4Resource);
                    return resultR4 != null ? new ResourceWrapper(resultR4) : null;
                default:
                    return default;
            }
        }

        private async Task<ResourceWrapper> CreateUsingHttpClientAsync(ResourceWrapper resourceWrapper)
        {
            var serializer = new SerializationWrapper(FhirVersion);
            var request = new FhirRequestMessage(HttpMethod.Post, resourceWrapper);
            var response = await CreateOrUpdateAsync(request);
            return serializer.Parse(response.Content, response.MimeType);
        }

        private async Task<ResourceWrapper> UpdateUsingHttpClientAsync(ResourceWrapper resourceWrapper)
        {
            var serializer = new SerializationWrapper(FhirVersion);
            var request = new FhirRequestMessage(HttpMethod.Put, resourceWrapper);
            var response = await CreateOrUpdateAsync(request);
            return serializer.Parse(response.Content, response.MimeType);
        }

        private async Task<FhirResponseMessage> CreateOrUpdateAsync(FhirRequestMessage request)
        {
             var relativeUrl = request.Resource.ResourceType.GetLiteral();
            if (request.Method == HttpMethod.Put)
            {
                relativeUrl += $"/{request.Resource.Id}";
            }
            // Build REST url
            var endpoint = $"{Endpoint}{relativeUrl}";

            // Build Request Message
            var requestMessage = new HttpRequestMessage(request.Method, endpoint);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(request.MediaType));
            requestMessage.Content = new StringContent(request.GetContentAsString(), Encoding.UTF8, request.MediaType);

            // Initialzie HttpClient and Send the request
            var client = new HttpClient();
            using var response = await client.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP Request failed with status code: '{response.StatusCode}' and reason: '{response.ReasonPhrase}'");
            }
            
            return new FhirResponseMessage((int)response.StatusCode, response.Content.Headers.ContentType.MediaType, await response.Content.ReadAsStringAsync());
        }
    }
}
