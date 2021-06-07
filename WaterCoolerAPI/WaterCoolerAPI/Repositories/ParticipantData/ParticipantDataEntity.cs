// <copyright file="ParticipantDataEntity.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Repositories.ParticipantData
{
    using System;
    using Microsoft.Azure.Cosmos.Table;

    /// <summary>
    /// Participant data entity class.
    /// This entity holds the information about a participant.
    /// </summary>
    public class ParticipantDataEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets the object id.
        /// </summary>
        public string CallId { get; set; }

        /// <summary>
        /// Gets or sets the object id.
        /// </summary>
        public string ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the log time.
        /// </summary>
        public DateTime? LogTime { get; set; }

        /// <summary>
        /// Gets or sets the Start Time Of Meeting for a User .
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the End time for a User.
        /// </summary>
        public DateTime? EndTime { get; set; }
    }
}
