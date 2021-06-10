// <copyright file="ParticipantsScheduler.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Graph;
    using WaterCoolerAPI.Interface;
    using WaterCoolerAPI.Repositories;
    using WaterCoolerAPI.Repositories.ParticipantData;
    using WaterCoolerAPI.Repositories.RoomData;

    /// <summary>
    /// Scheduler for getting participants.
    /// </summary>
    public class ParticipantsScheduler : IHostedService
    {
        private readonly IRoomDataRepository roomDataRepository;
        private readonly IParticipantDataRepository participantDataRepository;
        private readonly TableRowKeyGenerator tableRowKeyGenerator;
        private readonly ILogger<ParticipantsScheduler> logger;
        private readonly IGraph graph;
        private Timer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParticipantsScheduler" /> class.
        /// </summary>
        /// <param name="logger">ILogger instance.</param>
        /// <param name="roomDataRepository">IRoomDataRepository instance.</param>
        /// <param name="participantDataRepository">IParticipantDataRepository instance.</param>
        /// <param name="tableRowKeyGenerator">TableRowKeyGenerator instance.</param>
        /// <param name="graph">IGraph instance.</param>
        public ParticipantsScheduler(
            ILogger<ParticipantsScheduler> logger,
            IRoomDataRepository roomDataRepository,
            IParticipantDataRepository participantDataRepository,
            TableRowKeyGenerator tableRowKeyGenerator,
            IGraph graph)
        {
            this.roomDataRepository = roomDataRepository;
            this.logger = logger;
            this.participantDataRepository = participantDataRepository;
            this.tableRowKeyGenerator = tableRowKeyGenerator;
            this.graph = graph;
        }

        /// <summary>
        /// Starts service.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token data as input.</param>
        /// <returns>Completed task.</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.timer = new Timer(this.RunJob, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(10));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops service.
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Completed task.</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Update the DB with unique Participant Record.
        /// </summary>
        /// <param name="participantUserIdList">List of Participant User Id as input.</param>
        /// <param name="callId">Room Caller Id data as input.</param>
        public async void AddUpdateParticipantData(List<string> participantUserIdList, string callId)
        {
            if (callId != null)
            {
                var participantDataEntityList = await this.participantDataRepository.GetParticipantAsync(callId);
                List<ParticipantDataEntity> updatedParticipantDataEntityList = new List<ParticipantDataEntity>();

                if (participantUserIdList != null && participantUserIdList.Any())
                {
                    // Find common participants from Graph and DB call, where Participant EndTime is not null. This is for a scenario where user has rejoined the call.
                    List<ParticipantDataEntity> commonParticipantDataEntityList = participantDataEntityList.Where(participantDataEntity => participantDataEntity.EndTime == null).ToList();
                    List<string> commonParticipantUserIdList = commonParticipantDataEntityList.Select(participantDataEntity => participantDataEntity.ObjectId).ToList();

                    // Find new participants to be added in DB
                    List<string> newParticipantUserIdList = participantUserIdList.Except(commonParticipantUserIdList).ToList();

                    foreach (var newParticipantUserId in newParticipantUserIdList)
                    {
                        var newParticipant = new ParticipantDataEntity
                        {
                            PartitionKey = ParticipantDataTableNames.ParticipantDataPartition,
                            RowKey = this.tableRowKeyGenerator.CreateNewKeyOrderingOldestToMostRecent(),
                            CallId = callId,
                            ObjectId = newParticipantUserId,
                            StartTime = DateTime.UtcNow,
                            LogTime = DateTime.UtcNow,
                        };

                        updatedParticipantDataEntityList.Add(newParticipant);
                    }

                    // Find inactive partcipants to be updated in DB
                    List<string> droppedParticipantIdList = commonParticipantUserIdList.Except(participantUserIdList).ToList();
                    foreach (var droppedParticipantId in droppedParticipantIdList)
                    {
                        ParticipantDataEntity dropedParticipant = commonParticipantDataEntityList.Where(participantDataEntity => participantDataEntity.ObjectId == droppedParticipantId).FirstOrDefault();
                        dropedParticipant.EndTime = DateTime.UtcNow;
                        updatedParticipantDataEntityList.Add(dropedParticipant);
                    }
                }
                else if (participantDataEntityList != null && participantDataEntityList.Any())
                {
                    // Updating Participant EndTime for:
                    // 1. Room is not Active.
                    // 2. Room is available with only Bot.
                    foreach (var participantDataEntity in participantDataEntityList)
                    {
                        if (participantDataEntity.EndTime == null)
                        {
                            participantDataEntity.EndTime = DateTime.UtcNow;
                            updatedParticipantDataEntityList.Add(participantDataEntity);
                        }
                    }
                }

                if (updatedParticipantDataEntityList.Any())
                {
                    await this.participantDataRepository.BatchInsertOrMergeAsync(updatedParticipantDataEntityList);
                }
            }
        }

        /// <summary>
        /// Runs job.
        /// </summary>
        /// <param name="state">state information.</param>
        private async void RunJob(object state)
        {
            try
            {
                var graphserviceClient = await this.graph.GetGraphServiceClient();

                var activeRooms = await this.roomDataRepository.GetActiveRoomsAsync();

                foreach (var room in activeRooms)
                {
                    try
                    {
                        var participants = await graphserviceClient.Communications.Calls[room.CallId.ToString()].Participants
                            .Request()
                            .GetAsync();
                        var attendees = new List<ParticipantDataEntity>();
                        if (DateTime.UtcNow > room.EndDateTime && participants.CurrentPage.Count == 1 && participants.CurrentPage[0].Info.Identity.User == null)
                        {
                            await graphserviceClient.Communications.Calls[room.CallId.ToString()].Request().DeleteAsync();
                            room.IsActive = false;
                            await this.roomDataRepository.CreateOrUpdateAsync(room);

                            // If room is available with only Bot Participant, all the participant has to be updated with EndTime.
                            this.AddUpdateParticipantData(null, room.CallId.ToString());
                        }
                        else
                        {
                            // List to capture participantId for DB update.
                            List<string> activeParticipantUserIdList = new List<string>();
                            foreach (var participant in participants.CurrentPage)
                            {
                                if (participant.Info.Identity.User != null)
                                {
                                    activeParticipantUserIdList.Add(participant.Info.Identity.User.Id);
                                }
                            }

                            this.AddUpdateParticipantData(activeParticipantUserIdList, room.CallId.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogError(ex, ex.Message);

                        if (((ServiceException)ex).StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            room.IsActive = false;
                            await this.roomDataRepository.CreateOrUpdateAsync(room);

                            // If Room is not active, all the participant has to be updated with EndTime.
                            this.AddUpdateParticipantData(null, room.CallId.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
            }
        }
    }
}
