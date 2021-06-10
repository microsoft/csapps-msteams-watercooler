// <copyright file="ParticipantData.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Meetings.ParticipantData
{
    /// <summary>
    /// Scored Email Address data entity class.
    /// This entity holds the information about an active participant's basic details.
    /// </summary>
    public class ParticipantData
    {
        /// <summary>
        /// Gets or sets the Call Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the Display Name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the Given Name.
        /// </summary>
        public string GivenName { get; set; }

        /// <summary>
        /// Gets or sets the Surname.
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Gets or sets the Job Title.
        /// </summary>
        public string JobTitle { get; set; }

        /// <summary>
        /// Gets or sets the Company Name.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the Department.
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Gets or sets the OfficeLocation.
        /// </summary>
        public string OfficeLocation { get; set; }

        /// <summary>
        /// Gets or sets the Profession.
        /// </summary>
        public string Profession { get; set; }

        /// <summary>
        /// Gets or sets the UserPrincipalName.
        /// </summary>
        public string UserPrincipalName { get; set; }

        /// <summary>
        /// Gets or sets the photo of user.
        /// </summary>
        public string DisplayPicture { get; set; }
    }
}
