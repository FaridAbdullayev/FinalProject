using System.ComponentModel.DataAnnotations;

namespace HotelUI.Models.Setting
{
    public class SettingUpdateRequest
    {
        [Required]
        [MinLength(1)]
        public string Value { get; set; }
    }
}
