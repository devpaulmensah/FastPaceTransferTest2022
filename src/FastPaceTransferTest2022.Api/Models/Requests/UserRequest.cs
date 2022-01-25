using System.ComponentModel.DataAnnotations;

namespace FastPaceTransferTest2022.Api.Models.Requests
{
    public class UserRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string FirstName { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string LastName { get; set; }
        [Required(AllowEmptyStrings = false)]
        [MinLength(10)]
        [DataType(DataType.PhoneNumber)]
        public string MobileNumber { get; set; }
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
    }
}