using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Slider:BaseEntity
    {
        public string Image { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }  
    }
}
