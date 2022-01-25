using System.Threading.Tasks;
using FastPaceTransferTest2022.Api.Models.Requests;
using FastPaceTransferTest2022.Api.Models.Responses;

namespace FastPaceTransferTest2022.Api.Services.Interfaces
{
    public interface IOtpService
    {
        Task<BaseResponse<SendOtpResponse>> SendOtpAsync(OtpRequest request);
        Task<BaseResponse<LoginResponse>> VerifyOtp(VerifyOtpRequest request);
    }
}