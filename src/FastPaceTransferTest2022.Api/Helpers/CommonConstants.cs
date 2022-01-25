using System.Net;
using FastPaceTransferTest2022.Api.Models.Responses;

namespace FastPaceTransferTest2022.Api.Helpers
{
    public static class CommonConstants
    {
        public const string InternalServerErrorMessage = "Something went wrong, try again later!";
        public const string FailedDependencyErrorMessage = "An error occured, try again later!";
        public const string AppAuthIdentity = "fastpacetransfer_user";

        public static BaseResponse<T> GetInternalServerResponse<T>()
        {
            return new BaseResponse<T>
            {
                Code = (int) HttpStatusCode.InternalServerError,
                Message = InternalServerErrorMessage
            };
        }
        
        public static BaseResponse<T> GetFailedDependencyResponse<T>()
        {
            return new BaseResponse<T>
            {
                Code = (int) HttpStatusCode.FailedDependency,
                Message = FailedDependencyErrorMessage
            };
        }
    }
}