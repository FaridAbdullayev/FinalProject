using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Contact:BaseEntity
    {
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public string FullName { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
