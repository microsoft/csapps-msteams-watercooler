// <copyright file="IRoomDataRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Repositories.RoomData
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for Room Data Repository.
    /// </summary>
    public interface IRoomDataRepository : IRepository<RoomDataEntity>
    {
        /// <summary>
        /// Get active room data entities.
        /// </summary>
        /// <returns>The Active data entities.</returns>
        public Task<IEnumerable<RoomDataEntity>> GetActiveRoomsAsync();
    }
}
