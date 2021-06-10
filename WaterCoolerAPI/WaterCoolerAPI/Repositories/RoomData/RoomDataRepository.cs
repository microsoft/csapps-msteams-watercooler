// <copyright file="RoomDataRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Repositories.RoomData
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Table;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Repository of the room data stored in the table storage.
    /// </summary>
    public class RoomDataRepository : BaseRepository<RoomDataEntity>, IRoomDataRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomDataRepository"/> class.
        /// </summary>
        /// <param name="logger">The logging service.</param>
        /// <param name="repositoryOptions">Options used to create the repository.</param>
        public RoomDataRepository(
         ILogger<RoomDataRepository> logger,
         IOptions<RepositoryOptions> repositoryOptions)
         : base(
               logger,
               storageAccountConnectionString: repositoryOptions.Value.StorageAccountConnectionString,
               tableName: RoomDataTableNames.TableName,
               defaultPartitionKey: RoomDataTableNames.RoomDataPartition,
               ensureTableExists: repositoryOptions.Value.EnsureTableExists)
        {
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<RoomDataEntity>> GetActiveRoomsAsync()
        {
            var isActivePropertyFilter = this.GetIsActivePropertyFilter();
            var activeRoomDataEntities = await this.GetWithFilterAsync(isActivePropertyFilter);
            return activeRoomDataEntities;
        }

        private string GetIsActivePropertyFilter()
        {
            return TableQuery.GenerateFilterConditionForBool(
               "IsActive", QueryComparisons.Equal, true);
        }
    }
}
