// <copyright file="ParticipantDataRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Repositories.ParticipantData
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Table;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Repository of the room data stored in the table storage.
    /// </summary>
    public class ParticipantDataRepository : BaseRepository<ParticipantDataEntity>, IParticipantDataRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParticipantDataRepository"/> class.
        /// </summary>
        /// <param name="logger">The logging service.</param>
        /// <param name="repositoryOptions">Options used to create the repository.</param>
        public ParticipantDataRepository(
         ILogger<ParticipantDataRepository> logger,
         IOptions<RepositoryOptions> repositoryOptions)
         : base(
               logger,
               storageAccountConnectionString: repositoryOptions.Value.StorageAccountConnectionString,
               tableName: ParticipantDataTableNames.TableName,
               defaultPartitionKey: ParticipantDataTableNames.ParticipantDataPartition,
               ensureTableExists: repositoryOptions.Value.EnsureTableExists)
        {
        }

        /// <summary>
        /// Get Particpinat List from Database using Call Id.
        /// </summary>
        /// <returns> Return Active Participant Data Entities.</returns>
        /// <param name="callId">IGraph instance.</param>
        public async Task<IEnumerable<ParticipantDataEntity>> GetParticipantAsync(string callId)
        {
            var isActivePropertyFilter = this.GetIsActivePropertyFilter(callId);
            var activeParticipantDataEntities = await this.GetWithFilterAsync(isActivePropertyFilter);
            return activeParticipantDataEntities;
        }

        /// <summary>
        /// Get Active Particpinat List from Database using Call Id.
        /// </summary>
        private string GetIsActivePropertyFilter(string callId)
        {
            return TableQuery.GenerateFilterCondition(
               "CallId", QueryComparisons.Equal, callId);
        }
    }
}
