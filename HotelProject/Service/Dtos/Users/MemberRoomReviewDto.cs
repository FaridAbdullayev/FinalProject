using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Users
{
    public class MemberRoomReviewDto
    {
        public int RoomId { get; set; }
        public string Text { get; set; }
        public byte Rate { get; set; }
    }
}
