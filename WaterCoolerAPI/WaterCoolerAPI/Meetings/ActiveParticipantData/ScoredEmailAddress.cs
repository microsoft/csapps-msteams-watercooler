// <copyright file="ScoredEmailAddress.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Meetings.ParticipantData
{
    /// <summary>
    /// Scored Email Address data entity class.
    /// This entity holds the information about an active participant's Scored Email Address.
    /// </summary>
    public class ScoredEmailAddress
    {
        /// <summary>
        /// Gets or sets the Address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the Relevance Score.
        /// </summary>
        public string RelevanceScore { get; set; }

        /// <summary>
        /// Gets or sets the Selection Likelihood.
        /// </summary>
        public string SelectionLikelihood { get; set; }
    }
}
