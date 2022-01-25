using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using FastPaceTransferTest2022.Api.Database;
using FastPaceTransferTest2022.Api.Database.Models;
using FastPaceTransferTest2022.Api.Helpers;
using FastPaceTransferTest2022.Api.Models.Requests;
using FastPaceTransferTest2022.Api.Models.Responses;
using FastPaceTransferTest2022.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FastPaceTransferTest2022.Api.Services.Providers
{
    public class WalletService : IWalletService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<WalletService> _logger;
        private readonly IMapper _mapper;

        public WalletService(ApplicationDbContext dbContext,
            ILogger<WalletService> logger,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
        }
        
        public async Task<BaseResponse<WalletResponse>> CreateWallet(WalletRequest request)
        {
            try
            {
                var user = await _dbContext.Users.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id.Equals(request.UserId));
                
                if (user == null)
                {
                    return new BaseResponse<WalletResponse>
                    {
                        Code = (int) HttpStatusCode.BadRequest,
                        Message = $"No user found with id: {request.UserId}"
                    };
                }
                
                var wallet = await _dbContext.Wallets.AsNoTracking()
                    .FirstOrDefaultAsync(w => w.UserId.Equals(request.UserId));

                if (wallet != null)
                {
                    return new BaseResponse<WalletResponse>
                    {
                        Code = (int) HttpStatusCode.Conflict,
                        Message = "Wallet already created"
                    };
                }

                var newWallet = _mapper.Map<Wallet>(request);

                await _dbContext.AddAsync(newWallet);
                var response = await _dbContext.SaveChangesAsync();

                if (response < 1)
                {
                    _logger.LogError("An error occured saving wallet" +
                                     $"\nRequest: {JsonConvert.SerializeObject(request)}");

                    return CommonConstants.GetFailedDependencyResponse<WalletResponse>();
                }

                var walletResponse = _mapper.Map<WalletResponse>(newWallet);

                return new BaseResponse<WalletResponse>
                {
                    Code = (int) HttpStatusCode.Created,
                    Message = "Wallet created successfully",
                    Data = walletResponse
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occured creating a wallet" +
                                    $"\nRequest => {JsonConvert.SerializeObject(request)}");

                return CommonConstants.GetInternalServerResponse<WalletResponse>();
            }
        }
        
        public async Task<BaseResponse<WalletResponse>> GetWallet(string walletId)
        {
            try
            {
                var wallet = await _dbContext.Wallets.AsNoTracking()
                    .FirstOrDefaultAsync(w => w.Id.Equals(walletId));

                if (wallet == null)
                {
                    return new BaseResponse<WalletResponse>
                    {
                        Code = (int) HttpStatusCode.NotFound,
                        Message = "Wallet not found"
                    };
                }

                var walletResponse = _mapper.Map<WalletResponse>(wallet);
                
                return new BaseResponse<WalletResponse>
                {
                    Code = (int) HttpStatusCode.OK,
                    Message = "Retrieved successfully",
                    Data = walletResponse
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occured getting wallet with id: {walletId}");

                return CommonConstants.GetInternalServerResponse<WalletResponse>();
            }
        }
        
        public async Task<BaseResponse<WalletResponse>> GetUserWallet(string userId)
        {
            try
            {
                var wallet = await _dbContext.Wallets.AsNoTracking()
                    .FirstOrDefaultAsync(w => w.UserId.Equals(userId));

                var walletResponse = _mapper.Map<WalletResponse>(wallet);
                
                return new BaseResponse<WalletResponse>
                {
                    Code = (int) HttpStatusCode.OK,
                    Message = "Retrieved successfully",
                    Data = walletResponse
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occured getting user wallet with id: {userId}");

                return CommonConstants.GetInternalServerResponse<WalletResponse>();
            }
        }
        
        public async Task<BaseResponse<WalletResponse>> UpdateWallet(string walletId, WalletRequest request)
        {
            try
            {
                var wallet = await _dbContext.Wallets
                    .FirstOrDefaultAsync(w => w.Id.Equals(walletId));

                if (wallet == null)
                {
                    return new BaseResponse<WalletResponse>
                    {
                        Code = (int) HttpStatusCode.NotFound,
                        Message = "Wallet not found"
                    };
                }

                _mapper.Map(request, wallet);
                
                _dbContext.Update(wallet);
                var updateResponse = await _dbContext.SaveChangesAsync();

                if (updateResponse < 1)
                {
                    _logger.LogError($"An error occured updating a wallet with id: {walletId}" +
                                     $"\nRequest: {JsonConvert.SerializeObject(request)}");

                    return CommonConstants.GetFailedDependencyResponse<WalletResponse>();
                }
                
                var walletResponse = _mapper.Map<WalletResponse>(wallet);
                
                return new BaseResponse<WalletResponse>
                {
                    Code = (int) HttpStatusCode.OK,
                    Message = "Retrieved successfully",
                    Data = walletResponse
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occured getting wallet with id: {walletId}");

                return CommonConstants.GetInternalServerResponse<WalletResponse>();
            }
        }

        public async Task<BaseResponse<WalletResponse>> DeleteWalletByUserId(string userId)
        {
            try
            {
                var wallet = await _dbContext.Wallets
                    .FirstOrDefaultAsync(w => w.UserId.Equals(userId));

                if (wallet == null)
                {
                    return new BaseResponse<WalletResponse>
                    {
                        Code = (int) HttpStatusCode.NotFound,
                        Message = "Wallet not found"
                    };
                }
                
                _dbContext.Remove(wallet);
                var deleteResponse = await _dbContext.SaveChangesAsync();

                if (deleteResponse < 1)
                {
                    _logger.LogError($"An error occured deleting a wallet for user with id: {userId}");

                    return CommonConstants.GetFailedDependencyResponse<WalletResponse>();
                }
                
                return new BaseResponse<WalletResponse>
                {
                    Code = (int) HttpStatusCode.OK,
                    Message = "Deleted successfully"
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occured deleting wallet for user with id: {userId}");

                return CommonConstants.GetInternalServerResponse<WalletResponse>();
            }
        }
    }
}