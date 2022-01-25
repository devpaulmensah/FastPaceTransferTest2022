using System.Collections.Generic;
using System.Threading.Tasks;
using FastPaceTransferTest2022.Api.Models.Requests;
using FastPaceTransferTest2022.Api.Models.Responses;

namespace FastPaceTransferTest2022.Api.Services.Interfaces
{
    public interface IWalletService
    {
        Task<BaseResponse<WalletResponse>> CreateWallet(WalletRequest request);
        Task<BaseResponse<WalletResponse>> GetWallet(string walletId);
        Task<BaseResponse<WalletResponse>> GetUserWallet(string userId);
        Task<BaseResponse<WalletResponse>> UpdateWallet(string walletId, WalletRequest request);
        Task<BaseResponse<WalletResponse>> DeleteWalletByUserId(string walletId);
    }
}