using System.ComponentModel.DataAnnotations;

namespace FastPaceTransferTest2022.Api.Models.Requests
{
    public class CreateUserRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string FirstName { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string LastName { get; set; }
        [Required(AllowEmptyStrings = false)]
        [MinLength(10, ErrorMessage = "Phone number must not be less than 10")]
        [MaxLength(13, ErrorMessage = "Phone number must not be more than 13")]
        [DataType(DataType.PhoneNumber)]
        public string MobileNumber { get; set; }
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [MinLength(6)]
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [MinLength(6)]
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}