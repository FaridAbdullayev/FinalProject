using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Users
{
    public class ContactUserDto
    {
        public string? FullName { get; set; }
        public string? Message { get; set; }
        public string? Subject { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
