using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Users
{
    public class MemberResetPasswordDto
    {
        public string? NewPassword { get; set; }
        public string? ConfirmNewPassword { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
