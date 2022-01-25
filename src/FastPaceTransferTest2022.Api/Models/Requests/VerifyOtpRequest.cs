using System.ComponentModel.DataAnnotations;

namespace FastPaceTransferTest2022.Api.Models.Requests
{
    public class VerifyOtpRequest
    {
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [Required]
        public int OtpCode { get; set; }
    }
}