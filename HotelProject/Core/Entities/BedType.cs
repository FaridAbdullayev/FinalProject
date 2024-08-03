using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class BedType:BaseEntity
    {
        public string Name { get; set; }
        public List<Room> Room { get; set; }
    }
}
