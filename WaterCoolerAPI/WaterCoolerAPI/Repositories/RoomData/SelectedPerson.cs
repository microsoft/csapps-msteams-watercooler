// <copyright file="SelectedPerson.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Repositories.RoomData
{
    /// <summary>
    /// Selected person model.
    /// </summary>
    public class SelectedPerson
    {
        /// <summary>
        /// Gets or sets id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets given name.
        /// </summary>
        public string GivenName { get; set; }

        /// <summary>
        /// Gets or sets sur name.
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Gets or sets job title.
        /// </summary>
        public string JobTitle { get; set; }

        /// <summary>
        /// Gets or sets user principal name.
        /// </summary>
        public string UserPrincipalName { get; set; }

        /// <summary>
        /// Gets or sets IM address.
        /// </summary>
        public string ImAddress { get; set; }
    }
}
