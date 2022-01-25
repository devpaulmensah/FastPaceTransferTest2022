using AutoMapper;
using FastPaceTransferTest2022.Api.Database.Models;
using FastPaceTransferTest2022.Api.Models.Requests;
using FastPaceTransferTest2022.Api.Models.Responses;

namespace FastPaceTransferTest2022.Api.Mapping
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<UserRequest, User>().ReverseMap();
            CreateMap<CreateUserRequest, User>().ReverseMap();
            CreateMap<UserResponse, User>().ReverseMap();
            CreateMap<WalletResponse, Wallet>().ReverseMap();
            CreateMap<WalletRequest, Wallet>().ReverseMap();
        }
    }
}