using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Users
{
    public class MemberOurStaffGetDto
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}
