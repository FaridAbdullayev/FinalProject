using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Users
{
    public class RoomFilterCriteriaDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<int> ServiceIds { get; set; } = new List<int>();
        public int? BranchId { get; set; }
    }
}
