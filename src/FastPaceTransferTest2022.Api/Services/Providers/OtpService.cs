using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using AutoMapper;
using Bogus;
using FastPaceTransferTest2022.Api.Configurations;
using FastPaceTransferTest2022.Api.Database;
using FastPaceTransferTest2022.Api.Helpers;
using FastPaceTransferTest2022.Api.Models.Requests;
using FastPaceTransferTest2022.Api.Models.Responses;
using FastPaceTransferTest2022.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace FastPaceTransferTest2022.Api.Services.Providers
{
    public class OtpService : IOtpService
    {
        private readonly ILogger<OtpService> _logger;
        private readonly IConnectionMultiplexer _redis;
        private readonly ApplicationDbContext _dbContext;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly EmailConfiguration _emailConfig;
        private readonly Faker _faker;

        public OtpService(ILogger<OtpService> logger,
        IConnectionMultiplexer redis,
        ApplicationDbContext dbContext,
        IOptions<EmailConfiguration> emailConfig,
        IAuthService authService,
        IMapper mapper)
        {
            _logger = logger;
            _redis = redis;
            _dbContext = dbContext;
            _authService = authService;
            _mapper = mapper;
            _emailConfig = emailConfig.Value;
            _faker = new Faker();
        }
        
        public async Task<BaseResponse<SendOtpResponse>> SendOtpAsync(OtpRequest request)
        {
            try
            {
                var user = await _dbContext.Users.AsNoTracking()
                    .FirstOrDefaultAsync(u =>
                        u.EmailAddress.Equals(request.EmailAddress, StringComparison.OrdinalIgnoreCase));

                if (user == null)
                {
                    return new BaseResponse<SendOtpResponse>
                    {
                        Code = (int) HttpStatusCode.NotFound,
                        Message = $"No user found with email address: {request.EmailAddress}"
                    };
                }

                var otpCode = _faker.Random.Number(100000, 999999);
                
                // Save top in redis with phone number
                await _redis.GetDatabase().StringSetAsync($"otp:{user.MobileNumber}", otpCode);
                
                var emailSent = await SendOtpViaEmailAddress(user.EmailAddress, otpCode);

                if (!emailSent)
                {
                    return CommonConstants.GetInternalServerResponse<SendOtpResponse>();
                }
                
                return new BaseResponse<SendOtpResponse>
                {
                    Code = (int) HttpStatusCode.OK,
                    Message = "OTP sent to your email address successfully"
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occured sending otp to email address: {request.EmailAddress}");
                return CommonConstants.GetInternalServerResponse<SendOtpResponse>();
            }
        }

        public async Task<BaseResponse<LoginResponse>> VerifyOtp(VerifyOtpRequest request)
        {
            try
            {
                var user = await _dbContext.Users.AsNoTracking()
                    .FirstOrDefaultAsync(u =>
                        u.EmailAddress.Equals(request.EmailAddress, StringComparison.OrdinalIgnoreCase));

                if (user == null)
                {
                    return new BaseResponse<LoginResponse>
                    {
                        Code = (int) HttpStatusCode.NotFound,
                        Message = $"No user found with email: {request.EmailAddress}"
                    };
                }

                var redisKey = $"otp:{user.MobileNumber}";
                var otpCode = await _redis.GetDatabase().StringGetAsync(redisKey);
                
                if (!request.OtpCode.Equals(int.Parse(otpCode.ToString())))
                {
                    return new BaseResponse<LoginResponse>
                    {
                        Code = (int) HttpStatusCode.BadRequest,
                        Message = "Incorrect OTP code"
                    };
                }

                await _redis.GetDatabase().KeyDeleteAsync(redisKey);

                var userResponse = _mapper.Map<UserResponse>(user);

                var tokenData = _authService.GenerateToken(userResponse);

                return new BaseResponse<LoginResponse>
                {
                    Code = (int) HttpStatusCode.OK,
                    Message = "Verified successfully",
                    Data = new LoginResponse
                    {
                        Expiry = tokenData.Expiry,
                        Token = tokenData.BearerToken,
                        User = userResponse
                    }
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured verifying otp with" +
                                    $"\nRequest: {JsonConvert.SerializeObject(request)}");
                return CommonConstants.GetInternalServerResponse<LoginResponse>();
            }
        }

        private async Task<bool> SendOtpViaEmailAddress(string email, int otpCode)
        {
            await Task.Delay(0);
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailConfig.EmailAddress),
                    To = { email },
                    Subject = "FastPaceTransferTest OTP",
                    IsBodyHtml = true,
                    Body = $"Your OTP Code to sign in is {otpCode}"
                };

                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Port = _emailConfig.Port;
                    smtpClient.Host = _emailConfig.Host;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.Credentials = new NetworkCredential(_emailConfig.EmailAddress, _emailConfig.Password);
                    smtpClient.Send(mailMessage);
                }
                
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occured sending otp to user: {email}");
                return false;
            }
        }
    }
}