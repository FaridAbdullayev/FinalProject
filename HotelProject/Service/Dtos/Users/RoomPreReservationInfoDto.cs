using Service.Dtos.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Users
{
    public class RoomPreReservationInfoDto
    {
        public int BranchId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Nights { get; set; }
        public int ChildrenCount { get; set; }
        public int AdultsCount { get; set; }
        public double TotalPrice { get; set; }
        public List<RoomImageGetDto> Images { get; set; }
    }
}
