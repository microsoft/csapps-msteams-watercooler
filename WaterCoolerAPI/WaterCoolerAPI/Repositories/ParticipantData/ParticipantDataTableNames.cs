// <copyright file="ParticipantDataTableNames.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Repositories.ParticipantData
{
    /// <summary>
    /// Participant data table names.
    /// </summary>
    public class ParticipantDataTableNames
    {
        /// <summary>
        /// Table name for the room data table.
        /// </summary>
        public static readonly string TableName = "ParticipantData";

        /// <summary>
        /// Room data partition key name.
        /// </summary>
        public static readonly string ParticipantDataPartition = "ParticipantData";
    }
}
