using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using WaterCoolerAPI.Common;
using WaterCoolerAPI.Interface;
using WaterCoolerAPI.Models;

namespace WaterCoolerAPI.Helpers
{
    public class CreateRoomHelper
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CreateRoomHelper(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserPrincipalData> GetUserPrincipalAndId()
        {
            UserPrincipalData userPrincipal = new UserPrincipalData();
            var httpContext = this.httpContextAccessor.HttpContext;
            httpContext.Request.Headers.TryGetValue(Constants.Authorization, out StringValues assertion);
            var idToken = assertion.ToString().Split(" ")[1];
            if (idToken.Length > 0)
            {
                var handler = new JwtSecurityTokenHandler();
                if (handler.ReadToken(idToken) is JwtSecurityToken tokenS)
                {
                    userPrincipal.UserPrincipalName = tokenS.Claims.Where(a => a.Type.Equals(Constants.UpnKey)).Select(b => b).FirstOrDefault()?.Value;
                    userPrincipal.ObjectId = tokenS.Claims.Where(a => a.Type.Equals(Constants.OidKey)).Select(b => b).FirstOrDefault()?.Value;
                }
            }

            return userPrincipal;
        }

    }
}
