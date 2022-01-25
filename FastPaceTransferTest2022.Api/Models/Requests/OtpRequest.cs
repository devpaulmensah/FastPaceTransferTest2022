using System.ComponentModel.DataAnnotations;

namespace FastPaceTransferTest2022.Api.Models.Requests
{
    public class OtpRequest
    {
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
    }
}