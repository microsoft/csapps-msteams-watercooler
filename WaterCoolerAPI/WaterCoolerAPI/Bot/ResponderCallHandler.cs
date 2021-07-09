// <copyright file="ResponderCallHandler.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Bot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Graph;
    using Microsoft.Graph.Communications.Calls;
    using Microsoft.Graph.Communications.Common.Telemetry;
    using Microsoft.Graph.Communications.Resources;
    using WaterCoolerAPI.Data;
    using WaterCoolerAPI.IncidentManagement.IncidentStatus;

    /// <summary>
    /// The responder call handler class.
    /// </summary>
    public class ResponderCallHandler : CallHandler
    {
        private string responderId;

        private IncidentStatusData statusData;

        private int promptTimes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponderCallHandler"/> class.
        /// </summary>
        /// <param name="bot">The bot.</param>
        /// <param name="call">The call.</param>
        /// <param name="responderId">The responder id.</param>
        /// <param name="statusData">The incident status data.</param>
        public ResponderCallHandler(Bot bot, ICall call, string responderId, IncidentStatusData statusData)
            : base(bot, call)
        {
            this.responderId = responderId;
            this.statusData = statusData;

            this.statusData?.UpdateResponderNotificationCallId(this.responderId, call.Id, call.ScenarioId);
        }

        /// <inheritdoc/>
        protected override void CallOnUpdated(ICall sender, ResourceEventArgs<Call> args)
        {
            this.statusData?.UpdateResponderNotificationStatus(this.responderId, sender.Resource.State);

            if (sender.Resource.State == CallState.Established)
            {
                var currentPromptTimes = Interlocked.Increment(ref this.promptTimes);

                if (currentPromptTimes == 1)
                {
                    this.PlayPromptAndTransferToMeeting();
                }
            }
        }

        /// <summary>
        /// Play prompt and transfer to the incident meeting.
        /// </summary>
        private void PlayPromptAndTransferToMeeting()
        {
            Task.Run(async () =>
            {
                await this.PlayTransferingPromptAsync().ConfigureAwait(false);
                this.TransferToIncidentMeeting();
            });
        }

        /// <summary>
        /// Play the transfering prompt.
        /// </summary>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        private async Task PlayTransferingPromptAsync()
        {
            try
            {
                await this.Call.PlayPromptAsync(new List<MediaPrompt> { this.Bot.MediaMap[Bot.TransferingPromptName] }).ConfigureAwait(false);
                this.GraphLogger.Info(Common.Constants.StartedPlayingTransferingPrompt);
            }
            catch (Exception ex)
            {
                this.GraphLogger.Error(ex, Common.Constants.FailedToPlayTransferingPrompt);
                throw;
            }
        }

        /// <summary>
        /// add current responder to incident meeting as participant.
        /// </summary>
        private void TransferToIncidentMeeting()
        {
            Task.Run(async () =>
            {
                try
                {
                    var incidentMeetingCallId = this.statusData?.BotMeetingCallId;
                    var responderStatusData = this.statusData?.GetResponder(this.responderId);

                    if (incidentMeetingCallId != null && responderStatusData != null)
                    {
                        var addParticipantRequestData = new AddParticipantRequestData()
                        {
                            ObjectId = responderStatusData.ObjectId,
                            ReplacesCallId = responderStatusData.NotificationCallId,
                        };

                        await this.Bot.AddParticipantAsync(incidentMeetingCallId, addParticipantRequestData).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    this.GraphLogger.Error(ex, Common.Constants.FailedToTransferToIncidentMeeting);
                }
            });
        }
    }
}
