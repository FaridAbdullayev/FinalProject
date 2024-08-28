using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelUI.Models.Admin
{
    public class ResetPasswordAdmin
    {
        [MinLength(3)]
        [MaxLength(50)]
        public string UserName { get; set; }
        [MinLength(8)]
        [MaxLength(50)]
        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; }
        [MinLength(8)]
        [MaxLength(50)]

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [MinLength(8)]
        [MaxLength(50)]
        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
