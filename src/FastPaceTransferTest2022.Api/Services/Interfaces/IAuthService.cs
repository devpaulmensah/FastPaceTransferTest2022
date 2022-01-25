using System.Threading.Tasks;
using FastPaceTransferTest2022.Api.Models;
using FastPaceTransferTest2022.Api.Models.Requests;
using FastPaceTransferTest2022.Api.Models.Responses;

namespace FastPaceTransferTest2022.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<BaseResponse<LoginResponse>> Login(LoginRequest request);
        GenerateTokenResponse GenerateToken(UserResponse user);
    }
}