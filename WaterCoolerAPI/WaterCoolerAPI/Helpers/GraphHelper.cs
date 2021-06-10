// <copyright file="GraphHelper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Graph;
    using WaterCoolerAPI.Interface;
    using WaterCoolerAPI.Meetings.ParticipantData;
    using WaterCoolerAPI.Repositories.RoomData;

    /// <summary>
    /// Graph helper class.
    /// </summary>
    public class GraphHelper : IGraph
    {
        private readonly ILogger<GraphHelper> logger;
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphHelper"/> class.
        /// </summary>
        /// <param name="logger">ILogger instance.</param>
        /// <param name="configuration">IConfiguration instance.</param>
        /// <param name="httpClientFactory">IHttpClientFactory instance.</param>
        public GraphHelper(ILogger<GraphHelper> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
        }

        /// <inheritdoc/>
        public async Task<OnlineMeeting> CreateOnlineMeeting(GraphServiceClient graphServiceClient, RoomDataEntity roomDataEntity)
        {
            try
            {
                var onlineMeeting = new OnlineMeeting
                {
                    StartDateTime = DateTimeOffset.Parse(DateTimeOffset.UtcNow.ToString("o")),
                    EndDateTime = DateTimeOffset.Parse(DateTimeOffset.UtcNow.AddMinutes(30).ToString("o")),
                    Subject = roomDataEntity.Name,
                };

                var onlineMeetingResponse = await graphServiceClient.Users[this.configuration[Common.Constants.UserIdConfigurationSettingsKey]].OnlineMeetings
                    .Request()
                    .AddAsync(onlineMeeting);
                return onlineMeetingResponse;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<string> GetApplicationAccessToken()
        {
            return await AuthenticationHelper.GetAccessTokenAsync(this.configuration, this.httpClientFactory);
        }

        /// <summary>
        /// Get graph service client information.
        /// </summary>
        /// <returns>Graph service client.</returns>
        public async Task<GraphServiceClient> GetGraphServiceClient()
        {
            try
            {
                string token = await AuthenticationHelper.GetAccessTokenAsync(this.configuration, this.httpClientFactory);
                return new GraphServiceClient(
                new DelegateAuthenticationProvider(
                       (requestMessage) =>
                       {
                           requestMessage.Headers.Authorization = new AuthenticationHeaderValue(Common.Constants.Schema, token);

                           return Task.FromResult(0);
                       }));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Participant>> GetParticipants(GraphServiceClient graphServiceClient, string callId)
        {
            try
            {
                var participants = await graphServiceClient.Communications.Calls[callId].Participants.Request().GetAsync();
                return participants;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return null;
            }
        }
    }
}
