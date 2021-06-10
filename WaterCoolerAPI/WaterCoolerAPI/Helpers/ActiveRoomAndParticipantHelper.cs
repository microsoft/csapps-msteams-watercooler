// <copyright file="ActiveRoomAndParticipantHelper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
namespace WaterCoolerAPI.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using WaterCoolerAPI.Interface;
    using WaterCoolerAPI.Meetings.ParticipantData;
    using WaterCoolerAPI.Repositories.RoomData;

    /// <summary>
    /// Active Room And Participant helper class.
    /// </summary>
    public class ActiveRoomAndParticipantHelper : IActiveRoomAndParticipant
    {
        private readonly ILogger<ActiveRoomAndParticipantHelper> logger;
        private readonly IRoomDataRepository roomDataRepository;
        private readonly IGraph graph;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveRoomAndParticipantHelper"/> class.
        /// </summary>
        /// <param name="roomDataRepository">IRoomDataRepository instance.</param>
        /// <param name="logger">ILogger instance.</param>
        /// <param name="graph">IGraph instance.</param>
        public ActiveRoomAndParticipantHelper(
            IRoomDataRepository roomDataRepository,
            ILogger<ActiveRoomAndParticipantHelper> logger,
            IGraph graph)
        {
            this.roomDataRepository = roomDataRepository;
            this.logger = logger;
            this.graph = graph;
        }

        /// <summary>
        /// Get token using client credentials flow.
        /// </summary>
        /// <returns>Active Room Data along with User details.</returns>
        public async Task<List<ActiveRoomData>> ActiveParticipantListAsync()
        {
            List<ActiveRoomData> activeRoomList = new List<ActiveRoomData>();
            try
            {
                var activeRooms = await this.roomDataRepository.GetActiveRoomsAsync();
                if (activeRooms != null)
                {
                    var sortedActiveResult = activeRooms.OrderByDescending(c => c.StartDateTime);
                    var graphServiceClient = await this.graph.GetGraphServiceClient();
                    if (sortedActiveResult != null)
                    {
                        foreach (var room in sortedActiveResult)
                        {
                            ActiveRoomData activeRoomData = new ActiveRoomData();
                            activeRoomData.Name = room.Name;
                            activeRoomData.CallId = room.CallId;
                            activeRoomData.Description = room.Description;
                            activeRoomData.StartDateTime = room.StartDateTime;
                            activeRoomData.EndDateTime = room.EndDateTime;
                            activeRoomData.IsActive = room.IsActive;
                            activeRoomData.LogoUrl = room.LogoUrl;
                            activeRoomData.MeetingUrl = room.MeetingUrl;
                            activeRoomData.UserPrincipleName = room.UserPrincipleName;
                            activeRoomData.ObjectId = room.ObjectId;
                            if (!string.IsNullOrEmpty(room.CallId))
                            {
                                var participants = await this.graph.GetParticipants(graphServiceClient, room.CallId);
                                if (participants != null)
                                {
                                    var activeParticipants = new List<ParticipantData>();

                                    foreach (var participant in participants)
                                    {
                                        if (participant != null && participant.Info != null && participant.Info.Identity != null && participant.Info.Identity.User != null)
                                        {
                                            var activeParticipantData = new ParticipantData();
                                            activeParticipantData.Id = participant.Id;
                                            activeParticipantData.DisplayName = participant.Info.Identity.User.DisplayName;

                                            var userId = participant.Info.Identity.User.Id;
                                            if (!string.IsNullOrEmpty(userId))
                                            {
                                                var userDetails = await this.graph.GetUserDetailsAsync(graphServiceClient, userId);
                                                activeParticipantData.DisplayName = userDetails.DisplayName;
                                                activeParticipantData.GivenName = userDetails.GivenName;
                                                activeParticipantData.JobTitle = userDetails.JobTitle;
                                                activeParticipantData.OfficeLocation = userDetails.OfficeLocation;
                                                activeParticipantData.UserPrincipalName = userDetails.UserPrincipalName;
                                                activeParticipantData.DisplayPicture = await this.graph.GetPhotoAsync(graphServiceClient, userId);
                                                activeParticipants.Add(activeParticipantData);
                                            }
                                        }
                                    }

                                    activeRoomData.UserList = activeParticipants;
                                }
                            }

                            if (activeRoomData != null && activeRoomData.UserList != null && activeRoomData.UserList.Any())
                            {
                                activeRoomList.Add(activeRoomData);
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
            }

            return activeRoomList;
        }
    }
}
