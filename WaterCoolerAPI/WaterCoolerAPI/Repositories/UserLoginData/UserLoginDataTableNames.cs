namespace WaterCoolerAPI.Repositories.UserLoginData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// UserLoginData table names.
    /// </summary>
    public class UserLoginDataTableNames
    {
        /// <summary>
        /// Table name for the user login table.
        /// </summary>
        public static readonly string TableName = "UserLoginData";

        /// <summary>
        /// user login data partition key name.
        /// </summary>
        public static readonly string UserLoginDataPartition = "UserLoginData";
    }
}
