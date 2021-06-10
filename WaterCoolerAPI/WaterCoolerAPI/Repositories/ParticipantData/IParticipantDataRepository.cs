// <copyright file="IParticipantDataRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Repositories.ParticipantData
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for Participant Data Repository.
    /// </summary>
    public interface IParticipantDataRepository : IRepository<ParticipantDataEntity>
    {
        /// <summary>
        /// Get Participant data entities.
        /// </summary>
        /// <param name="callId"> Caller Id for room. </param>
        /// <returns>The Participant data entities.</returns>
        public Task<IEnumerable<ParticipantDataEntity>> GetParticipantAsync(string callId);
    }
}
