using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Service:AuditEntity
    {
        public string Name { get; set; }
        public List<RoomService> RoomServices { get; set; }
    }
}
