using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelUI.Models.Admin
{
    public class ResetPasswordAdmin
    {
        public string UserName { get; set; }

        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
