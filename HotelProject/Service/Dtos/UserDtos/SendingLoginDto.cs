using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.UserDtos
{
    public class SendingLoginDto
    {
        public string? Token { get; set; }

        public bool PasswordResetRequired { get; set; }
    }
}
