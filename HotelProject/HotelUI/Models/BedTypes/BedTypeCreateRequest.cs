using System.ComponentModel.DataAnnotations;

namespace HotelUI.Models.BedTypes
{
    public class BedTypeCreateRequest
    {
        [MaxLength(50)]
        [MinLength(3)]
        [Required]
        public string Name { get; set; }
    }
}
