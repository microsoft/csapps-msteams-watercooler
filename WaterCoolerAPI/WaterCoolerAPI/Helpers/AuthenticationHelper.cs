// <copyright file="AuthenticationHelper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using Newtonsoft.Json;
    using WaterCoolerAPI.Common;

    /// <summary>
    /// Authentication helper class.
    /// </summary>
    public static class AuthenticationHelper
    {
        /// <summary>
        /// Retrieve Valid Audiences.
        /// </summary>
        /// <param name="configuration">IConfiguration instance.</param>
        /// <returns>Valid Audiences.</returns>
        public static IEnumerable<string> GetValidAudiences(IConfiguration configuration)
        {
            var clientId = configuration[Constants.ClientIdConfigurationSettingsKey];
            var applicationIdUri = configuration[Constants.ApplicationIdURIConfigurationSettingsKey];
            var validAudiences = new List<string> { clientId, applicationIdUri.ToLower() };
            return validAudiences;
        }

        /// <summary>
        /// Retrieve Valid Issuers.
        /// </summary>
        /// <param name="configuration">IConfiguration instance.</param>
        /// <returns>Valid Issuers.</returns>
        public static IEnumerable<string> GetValidIssuers(IConfiguration configuration)
        {
            var tenantId = configuration[Constants.TenantIdConfigurationSettingsKey];

            var validIssuers = GetSettings(configuration);

            validIssuers = validIssuers.Select(validIssuer => validIssuer.Replace(Constants.TenantIdReplaceString, tenantId));

            return validIssuers;
        }

        /// <summary>
        /// Audience Validator.
        /// </summary>
        /// <param name="tokenAudiences">Token audiences.</param>
        /// <param name="securityToken">Security token.</param>
        /// <param name="validationParameters">Validation parameters.</param>
        /// <returns>Audience validator status.</returns>
        public static bool AudienceValidator(
            IEnumerable<string> tokenAudiences,
            SecurityToken securityToken,
            TokenValidationParameters validationParameters)
        {
            if (tokenAudiences == null || tokenAudiences.Count() == 0)
            {
                throw new ApplicationException(Constants.NoAudienceDefinedInToken);
            }

            var validAudiences = validationParameters.ValidAudiences;
            if (validAudiences == null || validAudiences.Count() == 0)
            {
                throw new ApplicationException(Constants.NoValidAudiencesDefinedInValidationParameters);
            }

            foreach (var tokenAudience in tokenAudiences)
            {
                if (validAudiences.Any(validAudience => validAudience.Equals(tokenAudience, StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get token using client credentials flow.
        /// </summary>
        /// <param name="configuration">IConfiguration instance.</param>
        /// <param name="clientFactory">IHttpClientFactory instance.</param>
        /// <returns>App access token.</returns>
        public static async Task<string> GetAccessTokenAsync(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            var body = $"grant_type=client_credentials&client_id={configuration[Constants.ClientIdConfigurationSettingsKey]}@{configuration[Constants.TenantIdConfigurationSettingsKey]}&client_secret={configuration[Constants.ClientSecretConfigurationSettingsKey]}&scope={Constants.GraphDefaultScope}";
            try
            {
                var client = clientFactory.CreateClient(Constants.GraphWebClient);
                string responseBody;
                using (var request = new HttpRequestMessage(HttpMethod.Post, configuration[Constants.AzureInstanceConfigurationSettingsKey] + configuration[Constants.TenantIdConfigurationSettingsKey] + configuration[Constants.AzureAuthUtrlConfigurationSettingsKey]))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ApplicationJsonMediaTypeHeader));
                    request.Content = new StringContent(body, Encoding.UTF8, Constants.ApplicationXWwwFormUrlencodedMediaTypeHeader);
                    using HttpResponseMessage response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        responseBody = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        responseBody = await response.Content.ReadAsStringAsync();
                        throw new Exception(responseBody);
                    }
                }

                string accessToken = JsonConvert.DeserializeObject<dynamic>(responseBody).access_token;
                return accessToken;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get token using client credentials flow.
        /// </summary>
        /// <param name="configuration">IConfiguration instance.</param>
        /// <param name="clientFactory">IHttpClientFactory instance.</param>
        /// <param name="idToken">Id token.</param>
        /// <returns>App access token on behalf of user.</returns>
        public static async Task<string> GetAccessTokenOnBehalfUserAsync(IConfiguration configuration, IHttpClientFactory clientFactory, string idToken)
        {
            var body = $"assertion={idToken}&requested_token_use=on_behalf_of&grant_type=urn:ietf:params:oauth:grant-type:jwt-bearer&client_id={configuration[Constants.ClientIdConfigurationSettingsKey]}@{configuration[Constants.TenantIdConfigurationSettingsKey]}&client_secret={configuration[Constants.ClientSecretConfigurationSettingsKey]}&scope={Constants.GraphDefaultScope}";
            try
            {
                var client = clientFactory.CreateClient(Constants.GraphWebClient);
                string responseBody;
                using var request = new HttpRequestMessage(HttpMethod.Post, configuration[Constants.AzureInstanceConfigurationSettingsKey] + configuration[Constants.TenantIdConfigurationSettingsKey] + configuration[Constants.AzureAuthUtrlConfigurationSettingsKey]);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ApplicationJsonMediaTypeHeader));
                request.Content = new StringContent(body, Encoding.UTF8, Constants.ApplicationXWwwFormUrlencodedMediaTypeHeader);
                using HttpResponseMessage response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    responseBody = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    responseBody = await response.Content.ReadAsStringAsync();
                    throw new Exception(responseBody);
                }

                var accessToken = JsonConvert.DeserializeObject<dynamic>(responseBody).access_token;
                return accessToken;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Retrieve Settings.
        /// </summary>
        /// <param name="configuration">IConfiguration instance.</param>
        /// <returns>Configuration settings for valid issuers.</returns>
        private static IEnumerable<string> GetSettings(IConfiguration configuration)
        {
            var configurationSettingsValue = configuration[Constants.ValidIssuersConfigurationSettingsKey];
            var settings = configurationSettingsValue
                ?.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                ?.Select(p => p.Trim());
            if (settings == null)
            {
                throw new ApplicationException($"{Constants.ValidIssuersConfigurationSettingsKey} {Constants.ValidIssuersConfigurationSettingsException}");
            }

            return settings;
        }
    }
}
