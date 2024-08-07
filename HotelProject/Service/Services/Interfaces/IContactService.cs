using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos.BedType;
using Service.Dtos;
using Service.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Dtos.Contact;

namespace Service.Services.Interfaces
{
    public interface IContactService
    {
        Task<Contact> ContactMessage(ContactUserDto contact);

        List<ContactListItemGetDto> GetAll();
        PaginatedList<ContactGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);

        Task ContactMessageAdmin(AdminAndIUserInteraction contact);
    }
}
