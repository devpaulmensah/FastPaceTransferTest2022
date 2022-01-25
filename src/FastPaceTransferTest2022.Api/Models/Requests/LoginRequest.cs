using System.ComponentModel.DataAnnotations;

namespace FastPaceTransferTest2022.Api.Models.Requests
{
    public class LoginRequest
    {
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}