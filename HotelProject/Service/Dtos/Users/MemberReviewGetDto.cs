using Core.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Users
{
    public class MemberReviewGetDto
    {
        public string? UserName { get; set; }
        public string Text { get; set; }
        public byte Rate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
