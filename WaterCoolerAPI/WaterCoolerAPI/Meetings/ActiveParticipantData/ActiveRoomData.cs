// <copyright file="ActiveRoomData.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
namespace WaterCoolerAPI.Meetings.ParticipantData
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Active Room Data Type data entity class.
    /// This entity holds the information about an Active Room Data.
    /// </summary>
    public class ActiveRoomData
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the logo url.
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// Gets or sets the start date time.
        /// </summary>
        public DateTimeOffset? StartDateTime { get; set; }

        /// <summary>
        /// Gets or sets the end date time.
        /// </summary>
        public DateTimeOffset? EndDateTime { get; set; }

        /// <summary>
        /// Gets or sets the meeting url.
        /// </summary>
        public string MeetingUrl { get; set; }

        /// <summary>
        /// Gets or sets the user principle name.
        /// </summary>
        public string UserPrincipleName { get; set; }

        /// <summary>
        /// Gets or sets the object id.
        /// </summary>
        public string ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the Call Id.
        /// </summary>
        public string CallId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the room is active or not.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a list of Active User Data.
        /// </summary>
        public List<ParticipantData> UserList { get; set; }
    }
}
