// <copyright file="Phone.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Meetings.ParticipantData
{
    /// <summary>
    /// Phone data entity class.
    /// This entity holds the information about an active participant's Phone details.
    /// </summary>
    public class Phone
    {
        /// <summary>
        /// Gets or sets the Phone Type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the Phone Number.
        /// </summary>
        public string Number { get; set; }
    }
}
