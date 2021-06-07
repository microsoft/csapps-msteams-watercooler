// <copyright file="IRoomLogoDataRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Repositories.RoomLogoUrlData
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for Room Logo Data Repository.
    /// </summary>
    public interface IRoomLogoDataRepository
    {
        /// <summary>
        /// Get List Of Logo Url for Rooms.
        /// </summary>
        /// <returns>The Blob Logo Data.</returns>
        public Task<List<BlobLogoData>> GetLogoUrlsAsync();
    }
}
