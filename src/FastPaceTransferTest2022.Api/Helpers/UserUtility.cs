using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FastPaceTransferTest2022.Api.Models.Responses;
using Newtonsoft.Json;

namespace FastPaceTransferTest2022.Api.Helpers
{
    public static class UserHelper
    {
        public static UserResponse GetUserData(this ClaimsPrincipal claims)
        {
            var claimsIdentity = claims.Identities.FirstOrDefault(i => i.AuthenticationType == CommonConstants.AppAuthIdentity);
            var userData = claimsIdentity?.FindFirst(ClaimTypes.Thumbprint);

            if (userData is null)
            {
                return new UserResponse();
            }

            var user = JsonConvert.DeserializeObject<UserResponse>(userData.Value);
            return user;
        }
    }
}