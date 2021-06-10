// <copyright file="ParticipantData.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Meetings.ParticipantData
{
    /// <summary>
    /// Scored Email Address data entity class.
    /// This entity holds the information about an active participant's basic details.
    /// </summary>
    public class ParticipantData
    {
        /// <summary>
        /// Gets or sets the Display Name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the UserId or ObjectId.
        /// </summary>
        public string UserId { get; set; }
    }
}
