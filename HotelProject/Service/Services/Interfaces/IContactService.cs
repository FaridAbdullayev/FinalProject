using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IContactService
    {
        Task ContactMessage(ContactUserDto contact);
    }
}
