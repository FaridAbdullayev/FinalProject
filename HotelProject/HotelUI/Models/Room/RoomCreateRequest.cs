
using System.ComponentModel.DataAnnotations;

namespace HotelUI.Models.Room
{
    public class RoomCreateRequest
    {
        [MaxLength(50)]
        [MinLength(3)]
        [Required]
        public string Name { get; set; }
        [MaxLength(50)]
        [MinLength(3)]
        [Required]
        public string Description { get; set; }
        public double Price { get; set; }
        public double Area { get; set; }
        [Range(2, int.MaxValue, ErrorMessage = "MaxChildrenCount must be at least 2.")]
        public int MaxChildrenCount { get; set; }

        [Range(2, int.MaxValue, ErrorMessage = "MaxAdultsCount must be at least 2.")]
        public int MaxAdultsCount { get; set; }
        public int BedTypeId { get; set; }
        public int BranchId { get; set; }
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
        public List<int> ServiceIds { get; set; } = new List<int>();
    }
}
