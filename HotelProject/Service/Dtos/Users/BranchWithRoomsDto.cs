using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Users
{
    public class BranchWithRoomsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<MemberRoomGetDto> Rooms { get; set; }
    }
}
