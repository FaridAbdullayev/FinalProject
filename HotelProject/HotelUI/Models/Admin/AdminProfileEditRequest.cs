using System.ComponentModel.DataAnnotations;

namespace HotelUI.Models.Admin
{
    public class AdminProfileEditRequest
    {
        [Required]
        public string UserName { get; set; }


        [MaxLength(50)]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }


        [MaxLength(50)]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [MaxLength(50)]
        [MinLength(8)]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
