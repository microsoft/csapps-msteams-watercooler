namespace WaterCoolerAPI.Repositories.UserLoginData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using WaterCoolerAPI.Helpers;
    using WaterCoolerAPI.Models;

    /// <summary>
    /// Interface for UserLoginData Repository.
    /// </summary>
    public interface IUserLoginDataRepository : IRepository<UserLoginDataEntity>
    {
        /// <summary>
        /// Get Participant data entities.
        /// </summary>
        /// <param name="userId"> Caller Id for room. </param>
        /// <returns>boolean.</returns>
        public Task<UserData> CheckFirstLoginAsync(UserPrincipalData userData);
    }
}
