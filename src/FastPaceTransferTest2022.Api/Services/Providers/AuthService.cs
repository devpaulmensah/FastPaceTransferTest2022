using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FastPaceTransferTest2022.Api.Configurations;
using FastPaceTransferTest2022.Api.Database;
using FastPaceTransferTest2022.Api.Helpers;
using FastPaceTransferTest2022.Api.Models;
using FastPaceTransferTest2022.Api.Models.Requests;
using FastPaceTransferTest2022.Api.Models.Responses;
using FastPaceTransferTest2022.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace FastPaceTransferTest2022.Api.Services.Providers
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;
        private readonly BearerTokenConfig _bearerTokenConfig;

        public AuthService(ILogger<AuthService> logger,
            IMapper mapper,
            ApplicationDbContext dbContext,
            IOptions<BearerTokenConfig> bearerTokenConfig)
        {
            _logger = logger;
            _mapper = mapper;
            _dbContext = dbContext;
            _bearerTokenConfig = bearerTokenConfig.Value;
        }

        /// <summary>
        /// Signs in a user with email and password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse<LoginResponse>> Login(LoginRequest request)
        {
            try
            {
                var user = await _dbContext.Users.AsNoTracking()
                    .FirstOrDefaultAsync(u => 
                        u.EmailAddress.Equals(request.EmailAddress));
                
                if (user == null)
                {
                    return new BaseResponse<LoginResponse>
                    {
                        Code = (int) HttpStatusCode.NotFound,
                        Message = "This account does not exist!"
                    };
                }
                
                var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
                if (!isPasswordValid)
                {
                    return new BaseResponse<LoginResponse>
                    {
                        Code = (int) HttpStatusCode.Unauthorized,
                        Message = "Incorrect username and password"
                    };
                }
                
                var userResponse = _mapper.Map<UserResponse>(user);
                var authToken = GenerateToken(userResponse);
                
                return new BaseResponse<LoginResponse>
                {
                    Code = (int) HttpStatusCode.OK,
                    Message = "Login successful",
                    Data = new LoginResponse
                    {
                        User = userResponse,
                        Token = authToken.BearerToken,
                        Expiry = authToken.Expiry
                    }
                };

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error signing in a user" +
                                    $"\nRequest: {JsonConvert.SerializeObject(request)}");

                return CommonConstants.GetInternalServerResponse<LoginResponse>();
            }
        }
        
        public GenerateTokenResponse GenerateToken(UserResponse user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_bearerTokenConfig.Key));
            var now = DateTime.UtcNow;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Thumbprint, JsonConvert.SerializeObject(user))
            };

            var token = new JwtSecurityToken(
                _bearerTokenConfig.Issuer,
                _bearerTokenConfig.Audience,
                claims,
                now.AddMilliseconds(-30),
                now.AddHours(12),
                new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature));

            var tokenString = tokenHandler.WriteToken(token);

            return new GenerateTokenResponse
            {
                Expiry  = token.Payload.Exp,
                BearerToken = tokenString
            };
        }
    }
}