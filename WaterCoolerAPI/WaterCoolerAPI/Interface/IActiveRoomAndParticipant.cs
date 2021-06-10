// <copyright file="IActiveRoomAndParticipant.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using WaterCoolerAPI.Meetings.ParticipantData;

    /// <summary>
    /// Interface for ActiveRoomAndParticipant.
    /// </summary>
    public interface IActiveRoomAndParticipant
    {
        /// <summary>
        /// Get token using client credentials flow.
        /// </summary>
        /// <returns>Active Room Data along with User details.</returns>
        Task<List<ActiveRoomData>> ActiveParticipantListAsync();
    }
}
