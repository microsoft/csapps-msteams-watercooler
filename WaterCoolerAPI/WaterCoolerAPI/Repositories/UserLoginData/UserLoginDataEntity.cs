namespace WaterCoolerAPI.Repositories.UserLoginData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Table;

    /// <summary>
    /// UserLoginData entity class.
    /// This entity holds the information about a user's login info.
    /// </summary>
    public class UserLoginDataEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets the User id.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the last login time for a User .
        /// </summary>
        public DateTime LastLogin { get; set; }

        /// <summary>
        /// Gets or sets the first login time for a User .
        /// </summary>
        public DateTime FirstLogin { get; set; }

        /// <summary>
        /// Gets or sets the User principal name for a User .
        /// </summary>
        public string UPN { get; set; }
    }
}
