using Core.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class RoomReview:BaseEntity
    {
        public string AppUserId { get; set; }
        public int RoomId { get; set; }
        public string Text { get; set; }
        public byte Rate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ReviewStatus Status { get; set; } = ReviewStatus.Pending;
        public AppUser AppUser { get; set; }
        public Room Room { get; set; }
    }
}
