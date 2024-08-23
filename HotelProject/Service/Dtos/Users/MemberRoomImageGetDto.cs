using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Users
{
    public class MemberRoomImageGetDto
    {
        public int Id { get; set; }
        public string? Image { get; set; }
        public bool IsMain { get; set; } = false;
    }
}
