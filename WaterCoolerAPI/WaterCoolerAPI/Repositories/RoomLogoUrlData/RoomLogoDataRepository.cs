// <copyright file="RoomLogoDataRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Repositories.TeamsLogoUrlData
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using WaterCoolerAPI.Repositories.RoomLogoUrlData;

    /// <summary>
    /// Repository of the room data stored in the table storage.
    /// </summary>
    public class RoomLogoDataRepository : IRoomLogoDataRepository
    {
        private BlobContainerClient blobContainerClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomLogoDataRepository"/> class.
        /// </summary>
        /// <param name="logger">The logging service.</param>
        /// <param name="repositoryOptions">Options used to create the repository.</param>
        public RoomLogoDataRepository(
            ILogger<RoomLogoDataRepository> logger,
            IOptions<RepositoryOptions> repositoryOptions)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(repositoryOptions.Value.StorageAccountConnectionString);
            this.blobContainerClient = blobServiceClient.GetBlobContainerClient(repositoryOptions.Value.BlobContainerName);
        }

        /// <inheritdoc/>
        public async Task<List<BlobLogoData>> GetLogoUrlsAsync()
        {
            var blobLogoDataList = new List<BlobLogoData>();
            if (this.blobContainerClient != null)
            {
                string containerUrl = this.blobContainerClient.Uri.ToString();
                var response = this.blobContainerClient.GetBlobsAsync();
                await foreach (BlobItem item in response)
                {
                    BlobLogoData blobLogoData = new BlobLogoData()
                    {
                        Name = item.Name,
                        Url = $"{containerUrl}/{item.Name}",
                    };
                    blobLogoDataList.Add(blobLogoData);
                }
            }

            return blobLogoDataList;
        }
    }
}
