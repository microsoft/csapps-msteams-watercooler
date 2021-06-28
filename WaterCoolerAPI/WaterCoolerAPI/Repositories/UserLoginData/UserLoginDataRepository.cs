using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterCoolerAPI.Helpers;
using WaterCoolerAPI.Models;

namespace WaterCoolerAPI.Repositories.UserLoginData
{
    public class UserLoginDataRepository : BaseRepository<UserLoginDataEntity>, IUserLoginDataRepository
    {

        private readonly TableRowKeyGenerator tableRowKeyGenerator;
        private readonly IOptions<RepositoryOptions> repositoryOptions;
        /// <summary>
        /// Initializes a new instance of the <see cref="UserLoginDataRepository"/> class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="repositoryOptions"></param>
        public UserLoginDataRepository(
            ILogger<UserLoginDataRepository> logger,
            TableRowKeyGenerator tableRowKeyGenerator,
            IOptions<RepositoryOptions> repositoryOptions)
            : base(
               logger,
               storageAccountConnectionString: repositoryOptions.Value.StorageAccountConnectionString,
               tableName: UserLoginDataTableNames.TableName,
               defaultPartitionKey: UserLoginDataTableNames.UserLoginDataPartition,
               ensureTableExists: repositoryOptions.Value.EnsureTableExists)
        {
            this.tableRowKeyGenerator = tableRowKeyGenerator;
            this.repositoryOptions = repositoryOptions;
        }

        public async Task<UserData> CheckFirstLoginAsync(UserPrincipalData userData)
        {
            UserData userLoginData = new UserData();
            userLoginData.TermsofUseText = this.repositoryOptions.Value.TermsofUseText;
            userLoginData.TermsofUseUrl = this.repositoryOptions.Value.TermsofUseUrl;
            try
            {
                var isExistPropertyFilter = this.GetIsExistPropertyFilter(userData.ObjectId);
                var user = await this.GetWithFilterAsync(isExistPropertyFilter);

                if (user.Any())
                {
                    user.First().LastLogin = DateTime.UtcNow;
                    await this.CreateOrUpdateAsync(user.First());
                    userLoginData.IsFirstLogin = false;
                }
                else
                {
                    UserLoginDataEntity userLogin = new UserLoginDataEntity();
                    userLogin.UserId = userData.ObjectId;
                    userLogin.UPN = userData.UserPrincipalName;
                    userLogin.LastLogin = DateTime.UtcNow;
                    userLogin.FirstLogin = DateTime.UtcNow;
                    userLogin.PartitionKey = UserLoginDataTableNames.UserLoginDataPartition;
                    userLogin.RowKey = this.tableRowKeyGenerator.CreateNewKeyOrderingOldestToMostRecent();
                    await this.CreateOrUpdateAsync(userLogin);
                    userLoginData.IsFirstLogin = true;
                }

                return userLoginData;
            }
            catch (Exception ex)
            {
                userLoginData.IsFirstLogin = false;
                return userLoginData;
            }
        }

        private string GetIsExistPropertyFilter(string userId)
        {
            return TableQuery.GenerateFilterCondition(
               "UserId", QueryComparisons.Equal, userId);
        }
    }
}
