using System.Threading.Tasks;
using FastPaceTransferTest2022.Api.Models.Filters;
using FastPaceTransferTest2022.Api.Models.Requests;
using FastPaceTransferTest2022.Api.Models.Responses;

namespace FastPaceTransferTest2022.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<BaseResponse<UserResponse>> CreateUser(CreateUserRequest request);
        Task<BaseResponse<UserResponse>> GetUser(string userId);
        Task<BaseResponse<PaginatedList<UserResponse>>> GetUsers(UserFilter filter);
        Task<BaseResponse<UserResponse>> UpdateUser(string userId, UserRequest request);
        Task<BaseResponse<UserResponse>> DeleteUser(string userId);
        Task<BaseResponse<UserProfileResponse>> GetUserProfile(string userId);
    }
}