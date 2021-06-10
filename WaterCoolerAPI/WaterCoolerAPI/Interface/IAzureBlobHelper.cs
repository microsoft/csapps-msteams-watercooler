// <copyright file="IAzureBlobHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Interface
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for AzureBlobStorageHelper.
    /// </summary>
    ///
    public interface IAzureBlobHelper
    {
        /// <summary>
        /// Check azure blob storage
        /// /// </summary>
        /// <returns>bool.</returns>
        Task<bool> CheckOrCreateBlobContainer();

        /// <summary>
        /// file upload
        /// /// </summary>
        /// <returns>bool.</returns>
        Task<bool> BlobUpload();
    }
}
