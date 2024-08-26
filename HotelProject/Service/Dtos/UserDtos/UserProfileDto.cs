using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.UserDtos
{
    public class UserProfileDto
    {
        public string UserName { get; set; }
        public string Role { get; set; }
       
    }
}
