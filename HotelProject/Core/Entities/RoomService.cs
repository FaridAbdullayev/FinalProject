using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class RoomService:BaseEntity
    {
        public int RoomId { get; set; }
        public Room Room { get; set; }
        public int ServiceId {  get; set; }
        public Service Service { get; set; }
    }
}
