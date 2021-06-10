// <copyright file="AzureBlobStorageHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using WaterCoolerAPI.Interface;
    using WaterCoolerAPI.Repositories;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// AzureBlobStorageHelper helper class.
    /// </summary>
    public class AzureBlobStorageHelper : IAzureBlobHelper
    {
        private readonly string connectionString;
        private readonly string blobContainer;
        private BlobContainerClient blobContainerClient;
        private readonly CloudStorageAccount storageAccount;
        private CloudBlobClient cloudBlobClient;
        private IOptions<RepositoryOptions> repositoryOptions;
        private CloudBlobContainer cloudBlobContainer;
        private readonly ILogger<AzureBlobStorageHelper> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobStorageHelper"/> class.
        /// </summary>
        /// <param name="repositoryOptions"></param>
        /// <param name="logger"></param>
        public AzureBlobStorageHelper(IOptions<RepositoryOptions> repositoryOptions, ILogger<AzureBlobStorageHelper> logger)
        {
            this.logger = logger;
            this.repositoryOptions = repositoryOptions;
            BlobServiceClient blobServiceClient = new BlobServiceClient(repositoryOptions.Value.StorageAccountConnectionString);
            this.blobContainerClient = blobServiceClient.GetBlobContainerClient(repositoryOptions.Value.BlobContainerName);
            this.blobContainer = repositoryOptions.Value.BlobContainerName;
            this.connectionString = repositoryOptions.Value.StorageAccountConnectionString;
        }

        /// <inheritdoc/>
        public async Task<bool> CheckOrCreateBlobContainer()
        {
            CloudStorageAccount storageAccount;
            try
            {
                if (CloudStorageAccount.TryParse(this.connectionString, out storageAccount))
                {
                    this.cloudBlobClient = storageAccount.CreateCloudBlobClient();
                    this.cloudBlobContainer = this.cloudBlobClient.GetContainerReference(this.repositoryOptions.Value.BlobContainerName);
                    if (!await this.cloudBlobContainer.ExistsAsync())
                    {
                        await this.cloudBlobContainer.CreateAsync();
                        BlobContainerPermissions permissions = new BlobContainerPermissions
                        {
                            PublicAccess = BlobContainerPublicAccessType.Blob,
                        };
                        await this.cloudBlobContainer.SetPermissionsAsync(permissions);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> BlobUpload()
        {
            try
            {
                string localPath = Path.Combine(Environment.CurrentDirectory, @"Icons\");
                List<string> sourceFile = Directory.GetFiles(localPath).ToList();

                // Get a reference to the blob address, then upload the file to the blob.
                // Use the value of localFileName for the blob name.

                foreach (string file in sourceFile)
                {
                    string filename = Path.GetFileName(file);
                    CloudBlockBlob cloudBlockBlob = this.cloudBlobContainer.GetBlockBlobReference(filename);
                    cloudBlockBlob.Properties.ContentType = "image/png";
                    await cloudBlockBlob.UploadFromFileAsync(@"Icons\" + filename);
                }

                return true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return false;
            }

        }
    }
}
