using System.Net.Mime;
using System.Threading.Tasks;
using FastPaceTransferTest2022.Api.Helpers;
using FastPaceTransferTest2022.Api.Models.Filters;
using FastPaceTransferTest2022.Api.Models.Requests;
using FastPaceTransferTest2022.Api.Models.Responses;
using FastPaceTransferTest2022.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FastPaceTransferTest2022.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Create user account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BaseResponse<UserResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponse<EmptyResponse>))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(BaseResponse<EmptyResponse>))]
        [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(BaseResponse<EmptyResponse>))]
        [SwaggerOperation("Create user account", OperationId = nameof(CreateUser))]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
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
        /// Get user details
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<UserResponse>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse<EmptyResponse>))]
        [SwaggerOperation("Get user details", OperationId = nameof(GetUserDetails))]
        public async Task<IActionResult> GetUserDetails([FromRoute] string userId)
        {
            var response = await _userService.GetUser(userId);
            
            return !200.Equals(response.Code)
                ? StatusCode(response.Code, response)
                : Ok(response);
        }
        
        /// <summary>
        /// Get paginated list of users
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<PaginatedList<UserResponse>>))]
        [SwaggerOperation("Get paginated list of users", OperationId = nameof(GetUsers))]
        public async Task<IActionResult> GetUsers([FromQuery] UserFilter filter)
        {
            var response = await _userService.GetUsers(filter);

            return !200.Equals(response.Code)
                ? StatusCode(response.Code, response)
                : Ok(response);
        }

        /// <summary>
        /// Update user details
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{userId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<UserResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponse<EmptyResponse>))]
        [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(BaseResponse<EmptyResponse>))]
        [SwaggerOperation("Update user details", OperationId = nameof(UpdateUserDetails))]
        public async Task<IActionResult> UpdateUserDetails([FromRoute] string userId, [FromBody] UserRequest request)
        {
            var response = await _userService.UpdateUser(userId, request);
            
            return !200.Equals(response.Code)
                ? StatusCode(response.Code, response)
                : Ok(response);
        }
        
        /// <summary>
        /// Delete user details
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete("{userId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<UserResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponse<EmptyResponse>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse<EmptyResponse>))]
        [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(BaseResponse<EmptyResponse>))]
        [SwaggerOperation("Delete user details", OperationId = nameof(DeleteUserDetails))]
        public async Task<IActionResult> DeleteUserDetails([FromRoute] string userId)
        {
            var response = await _userService.DeleteUser(userId);
            
            return !200.Equals(response.Code)
                ? StatusCode(response.Code, response)
                : Ok(response);
        }
        
        /// <summary>
        /// Get user profile from auth token
        /// </summary>
        /// <returns></returns>
        [HttpGet("profile")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<UserProfileResponse>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse<EmptyResponse>))]
        [SwaggerOperation("Get user profile from auth token", OperationId = nameof(GetUserProfile))]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = User.GetUserData().Id;
            var response = await _userService.GetUserProfile(userId);
            
            return !200.Equals(response.Code)
                ? StatusCode(response.Code, response)
                : Ok(response);
        }

        /// <summary>
        /// Update user's password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("password")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<UserResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponse<EmptyResponse>))]
        [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(BaseResponse<EmptyResponse>))]
        [SwaggerOperation("Update user password", OperationId = nameof(UpdateUserPassword))]
        public async Task<IActionResult> UpdateUserPassword(UpdatePasswordRequest request)
        {
            var userId = User.GetUserData().Id;
            var response = await _userService.UpdatePassword(userId, request);

            return !200.Equals(response.Code)
                ? StatusCode(response.Code, response)
                : Ok(response);
        }
    }
}