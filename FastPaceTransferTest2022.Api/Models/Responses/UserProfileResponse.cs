using System.Collections.Generic;

namespace FastPaceTransferTest2022.Api.Models.Responses
{
    public class UserProfileResponse
    {
        public UserResponse User { get; set; }
        public WalletResponse Wallet { get; set; }
    }
}