using System.Net.Mime;
using System.Threading.Tasks;
using FastPaceTransferTest2022.Api.Models.Requests;
using FastPaceTransferTest2022.Api.Models.Responses;
using FastPaceTransferTest2022.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FastPaceTransferTest2022.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponse<EmptyResponse>))]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IOtpService _otpService;

        public AuthController(IAuthService authService,
            IUserService userService,
            IOtpService otpService)
        {
            _authService = authService;
            _userService = userService;
            _otpService = otpService;
        }

        /// <summary>
        /// Login a user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login/email")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<LoginResponse>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse<EmptyResponse>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(BaseResponse<EmptyResponse>))]
        [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(BaseResponse<EmptyResponse>))]
        [SwaggerOperation(nameof(Login), OperationId = nameof(Login))]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.Login(request);

            return !200.Equals(response.Code)
                ? StatusCode(response.Code, response)
                : Ok(response);
        }

        /// <summary>
        /// Register a user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BaseResponse<UserResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponse<EmptyResponse>))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(BaseResponse<EmptyResponse>))]
        [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(BaseResponse<EmptyResponse>))]
        [SwaggerOperation(nameof(Register), OperationId = nameof(Register))]
        public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
        {
            var response = await _userService.CreateUser(request);

            if (!201.Equals(response.Code))
            {
                return StatusCode(response.Code, response);
            }

            var contextRequest = HttpContext.Request;
            var url = $"{contextRequest.Scheme}://{contextRequest.Host}{contextRequest.Path}/{response.Data.Id}";

            return Created(url, response);
        }

        /// <summary>
        /// Send otp code to user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("otp/send")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<SendOtpResponse>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse<EmptyResponse>))]
        [SwaggerOperation("Send otp code to email address", OperationId = nameof(SendOtpCode))]
        public async Task<IActionResult> SendOtpCode(OtpRequest request)
        {
            var response = await _otpService.SendOtpAsync(request);

            return !200.Equals(response.Code)
                ? StatusCode(response.Code, response)
                : Ok(response);
        }
        
        /// <summary>
        /// Verify otp code sent to user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("otp/verify")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<SendOtpResponse>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse<EmptyResponse>))]
        [SwaggerOperation("Verify otp code sent to email address", OperationId = nameof(VerifyOtpCode))]
        public async Task<IActionResult> VerifyOtpCode(VerifyOtpRequest request)
        {
            var response = await _otpService.VerifyOtp(request);

            return !200.Equals(response.Code)
                ? StatusCode(response.Code, response)
                : Ok(response);
        }
    }
}