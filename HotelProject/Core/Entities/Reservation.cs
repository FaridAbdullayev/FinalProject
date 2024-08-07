using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Reservation:BaseEntity
    {
        public int RoomId { get; set; }
        public Room Room { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
