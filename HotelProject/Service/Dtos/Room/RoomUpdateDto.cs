using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Room
{
    public class RoomUpdateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double Area { get; set; }
        public int MaxChildrenCount { get; set; }
        public int MaxAdultsCount { get; set; }
        public string BedType { get; set; }
        public int BranchId { get; set; }
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
        public List<int>? RoomImageIds { get; set; } = new List<int>();
        public List<int>? ServiceIds { get; set; } = new List<int>();
    }
}
