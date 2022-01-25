using System.Net.Mime;
using System.Threading.Tasks;
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
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        /// <summary>
        /// Get wallet details
        /// </summary>
        /// <param name="walletId"></param>
        /// <returns></returns>
        [HttpGet("{walletId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<WalletResponse>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse<EmptyResponse>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(BaseResponse<EmptyResponse>))]
        [SwaggerOperation("Get wallet details", OperationId = nameof(GetUserWallet))]
        public async Task<IActionResult> GetUserWallet(string walletId)
        {
            var response = await _walletService.GetWallet(walletId);
            
            return !200.Equals(response.Code)
                ? StatusCode(response.Code, response)
                : Ok(response);
        }
        
        /// <summary>
        /// Create wallet
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BaseResponse<WalletResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponse<EmptyResponse>))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(BaseResponse<EmptyResponse>))]
        [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(BaseResponse<EmptyResponse>))]
        [SwaggerOperation("Create wallet", OperationId = nameof(CreateWallet))]
        public async Task<IActionResult> CreateWallet([FromBody] WalletRequest request)
        {
            var response = await _walletService.CreateWallet(request);
            
            if (!201.Equals(response.Code))
            {
                return StatusCode(response.Code, response);
            }

            var contextRequest = HttpContext.Request;
            var url = $"{contextRequest.Scheme}://{contextRequest.Host}{contextRequest.Path}/{response.Data.Id}";
            
            return Created(url, response);
        }

        /// <summary>
        /// Update wallet
        /// </summary>
        /// <param name="walletId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{walletId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<WalletResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponse<EmptyResponse>))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(BaseResponse<EmptyResponse>))]
        [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(BaseResponse<EmptyResponse>))]
        [SwaggerOperation("Update wallet", OperationId = nameof(UpdateWallet))]
        public async Task<IActionResult> UpdateWallet([FromRoute] string walletId,  [FromBody] WalletRequest request)
        {
            var response = await _walletService.UpdateWallet(walletId, request);
            
            return !200.Equals(response.Code)
                ? StatusCode(response.Code, response)
                : Ok(response);
        }
    }
}