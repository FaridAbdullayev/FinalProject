using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Contact
{
    public class ContactListItemGetDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }
        public string Email { get; set; }
    }
}
