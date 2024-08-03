
using HotelUI.Models.Enums;

namespace HotelUI.Models.Room
{
    public class RoomUpdateRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public double? Area { get; set; }
        public int? MaxChildrenCount { get; set; }
        public int? MaxAdultsCount { get; set; }
        public int? BedTypeId { get; set; }
        public int? BranchId { get; set; }
        public List<IFormFile>? Images { get; set; } = new List<IFormFile>();
        public List<int>? RoomImageIds { get; set; }
        public List<int>? ServiceIds { get; set; } = new List<int>();
    }
}
