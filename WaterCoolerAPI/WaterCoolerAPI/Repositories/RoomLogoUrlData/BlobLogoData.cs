// <copyright file="BlobLogoData.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Repositories.RoomLogoUrlData
{
    using System;

    /// <summary>
    ///  Blob Logo Data Type data entity class.
    /// This entity holds the information about an Logo Images.
    /// </summary>
    public class BlobLogoData
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        public string Url { get; set; }
    }
}
