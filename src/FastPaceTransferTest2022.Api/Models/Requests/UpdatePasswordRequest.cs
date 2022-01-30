using System.ComponentModel.DataAnnotations;

namespace FastPaceTransferTest2022.Api.Models.Requests
{
    public class UpdatePasswordRequest
    {
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string NewPassword { get; set; }
        [MinLength(6)]
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}