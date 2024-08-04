using System.ComponentModel.DataAnnotations;

namespace HotelUI.Models.OurStaff
{
    public class OurStaffUpdateRequest
    {
        [MaxLength(50)]
        [MinLength(3)]
        [Required]
        public string Name { get; set; }
        [MaxLength(50)]
        [MinLength(3)]
        [Required]
        public string Position { get; set; }
        [MaxLength(50)]
        [MinLength(3)]
        [Required]
        public string Description { get; set; }
        public IFormFile? File { get; set; }
    }
}
