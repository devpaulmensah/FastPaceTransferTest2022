using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using FastPaceTransferTest2022.Api.Database;
using FastPaceTransferTest2022.Api.Database.Models;
using FastPaceTransferTest2022.Api.Helpers;
using FastPaceTransferTest2022.Api.Models.Filters;
using FastPaceTransferTest2022.Api.Models.Requests;
using FastPaceTransferTest2022.Api.Models.Responses;
using FastPaceTransferTest2022.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FastPaceTransferTest2022.Api.Services.Providers
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IWalletService _walletService;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public UserService(ILogger<UserService> logger,
            IWalletService walletService,
            ApplicationDbContext dbContext,
            IMapper mapper)
        {
            _logger = logger;
            _walletService = walletService;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<BaseResponse<UserResponse>> CreateUser(CreateUserRequest request)
        {
            try
            {
                var user = await _dbContext.Users.AsNoTracking()
                    .FirstOrDefaultAsync(u =>
                        u.EmailAddress.Equals(request.EmailAddress) ||
                        u.MobileNumber.Equals(request.MobileNumber));

                if (user != null)
                {
                    return new BaseResponse<UserResponse>
                    {
                        Code = (int) HttpStatusCode.Conflict,
                        Message = "User already created"
                    };
                }

                var newUser = _mapper.Map<User>(request);
                newUser.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

                await _dbContext.AddAsync(newUser);
                var response = await _dbContext.SaveChangesAsync();
                
                if (response < 1)
                {
                    _logger.LogError("An error occured saving user" +
                                     $"\nRequest: {JsonConvert.SerializeObject(request)}");

                    return CommonConstants.GetFailedDependencyResponse<UserResponse>();
                }

                await _walletService.CreateWallet(new WalletRequest {UserId = newUser.Id});
                var userResponse = _mapper.Map<UserResponse>(newUser);
                
                return new BaseResponse<UserResponse>
                {
                    Code = (int) HttpStatusCode.Created,
                    Message = "User created successfully",
                    Data = userResponse
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occured creating a user" +
                                    $"\nRequest => {JsonConvert.SerializeObject(request)}");

                return CommonConstants.GetInternalServerResponse<UserResponse>();
            }
        }

        public async Task<BaseResponse<UserResponse>> GetUser(string userId)
        {
            try
            {
                var user = await _dbContext.Users.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id.Equals(userId));

                if (user == null)
                {
                    return new BaseResponse<UserResponse>
                    {
                        Code = (int) HttpStatusCode.NotFound,
                        Message = "User not found"
                    };
                }

                var userResponse = _mapper.Map<UserResponse>(user);

                return new BaseResponse<UserResponse>
                {
                    Code = (int) HttpStatusCode.OK,
                    Message = "Retrieved successfully",
                    Data = userResponse
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occured getting user: {userId} details");

                return CommonConstants.GetInternalServerResponse<UserResponse>();
            }
        }

        public async Task<BaseResponse<PaginatedList<UserResponse>>> GetUsers(UserFilter filter)
        {
            try
            {
                filter.PageIndex = filter.PageIndex < 1 ? 1 : filter.PageIndex;
                filter.PageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

                var filterQueryableResponse = await FilterPurchaseOrder(filter);
                var filterResponse = await filterQueryableResponse.Paginate(filter.PageIndex, filter.PageSize);

                var userResponseList = filterResponse.Data.Select(u =>
                    _mapper.Map<UserResponse>(u)).ToList();
                
                return new BaseResponse<PaginatedList<UserResponse>>
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Retrieved successfully",
                    Data = new PaginatedList<UserResponse>
                    {
                        Data  = userResponseList,
                        PageIndex = filterResponse.PageIndex,
                        PageSize = filterResponse.PageSize,
                        TotalPages = filterResponse.TotalPages,
                        TotalRecords = filterResponse.TotalRecords
                    }
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occured getting purchase list of users" +
                                    $"\nFilters => {JsonConvert.SerializeObject(filter)}");

                return CommonConstants.GetInternalServerResponse<PaginatedList<UserResponse>>();
            }
        }
        
        private async Task<IQueryable<User>> FilterPurchaseOrder(UserFilter filter)
        {
            await Task.Delay(0);
            var filterQuery = _dbContext.Users.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(filter.FirstName))
            {
                filterQuery = filterQuery.Where(u => u.FirstName.ToLower().Contains(filter.FirstName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filter.LastName))
            {
                filterQuery = filterQuery.Where(u => u.LastName.ToLower().Contains(filter.LastName.ToLower()));
            }
            
            if (!string.IsNullOrEmpty(filter.MobileNumber))
            {
                filterQuery = filterQuery.Where(u => u.MobileNumber.Equals(filter.MobileNumber));
            }
            
            if (!string.IsNullOrEmpty(filter.EmailAddress))
            {
                filterQuery = filterQuery.Where(u => u.EmailAddress.Equals(filter.MobileNumber));
            }
            
            if (filter.StartDate.HasValue)
            {
                filterQuery = filterQuery.Where(u => u.CreatedAt >= filter.StartDate.Value);
            }
            
            if (filter.EndDate.HasValue)
            {
                filterQuery = filterQuery.Where(u => u.CreatedAt <= filter.StartDate.Value);
            }

            filterQuery = !string.IsNullOrEmpty(filter.SortOrder) && filter.SortOrder == "desc"
                ? filterQuery.OrderByDescending(u => u.CreatedAt)
                : filterQuery.OrderBy(u => u.CreatedAt);

            return filterQuery;
        }

        public async Task<BaseResponse<UserResponse>> UpdateUser(string userId, UserRequest request)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));

                if (user == null)
                {
                    return new BaseResponse<UserResponse>
                    {
                        Code = (int) HttpStatusCode.NotFound,
                        Message = "User not found"
                    };
                }

                _mapper.Map(request, user);

                _dbContext.Update(user);
                var updateResponse = await _dbContext.SaveChangesAsync();
                
                if (updateResponse < 1)
                {
                    _logger.LogError($"An error occured updating user: {userId} details" +
                                     $"\nRequest: {JsonConvert.SerializeObject(request)}");

                    return CommonConstants.GetFailedDependencyResponse<UserResponse>();
                }

                var userResponse = _mapper.Map<UserResponse>(user);

                return new BaseResponse<UserResponse>
                {
                    Code = (int) HttpStatusCode.OK,
                    Message = "Updated successfully",
                    Data = userResponse
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occured updating user: {userId} details" +
                                    $"\nRequest: {JsonConvert.SerializeObject(request)}");

                return CommonConstants.GetInternalServerResponse<UserResponse>();
            }
        }

        public async Task<BaseResponse<UserResponse>> DeleteUser(string userId)
        {
            try
            {
                var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Id.Equals(userId));

                if (user == null)
                {
                    return new BaseResponse<UserResponse>
                    {
                        Code = (int) HttpStatusCode.NotFound,
                        Message = "User not found"
                    };
                }

                _dbContext.Remove(user);
                var deleteResponse = await _dbContext.SaveChangesAsync();

                if (deleteResponse < 1)
                {
                    _logger.LogError($"An error occured deleting user: {userId} details");

                    return CommonConstants.GetFailedDependencyResponse<UserResponse>();
                }

                await _walletService.DeleteWalletByUserId(userId);
                
                return new BaseResponse<UserResponse>
                {
                    Code = (int) HttpStatusCode.OK,
                    Message = "User deleted successfully"
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occured deleting user: {userId} details");

                return CommonConstants.GetInternalServerResponse<UserResponse>();
            }
        }

        public async Task<BaseResponse<UserProfileResponse>> GetUserProfile(string userId)
        {
            try
            {
                var userResponse = await GetUser(userId);

                if (!200.Equals(userResponse.Code))
                {
                    return new BaseResponse<UserProfileResponse>
                    {
                        Code = (int) HttpStatusCode.NotFound,
                        Message = "User not found"
                    };
                }

                var walletResponse = await _walletService.GetUserWallet(userId);

                var userProfile = new UserProfileResponse
                {
                    User = userResponse.Data,
                    Wallet = 200.Equals(walletResponse.Code)
                        ? walletResponse.Data
                        : null
                };

                return new BaseResponse<UserProfileResponse>
                {
                    Code = (int) HttpStatusCode.OK,
                    Message = "Retrieved successfully",
                    Data = userProfile
                };
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occured getting user: {userId} profile");
                return CommonConstants.GetInternalServerResponse<UserProfileResponse>();
            }
        }
    }
}