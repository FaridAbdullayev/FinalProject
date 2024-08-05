using AutoMapper;
using Core.Entities;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Dtos.Users;
using Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Service.Exceptions.ResetException;

namespace Service.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contact;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public ContactService(IContactRepository contact, IMapper mapper, UserManager<AppUser> userManager)
        {
            _contact = contact;
            _mapper = mapper;
            _userManager = userManager;
        }
        public async Task ContactMessage(ContactUserDto contact)
        {
            var user = await _userManager.FindByEmailAsync(contact.Email);



            if (user != null || await _userManager.IsInRoleAsync(user, "member"))
            {
                Contact contact1 = _mapper.Map<Contact>(contact);


                contact.AppUserId = user.Id;
                contact.CreatedAt = DateTime.UtcNow;

                _contact.Add(contact1);
                _contact.Save();
            }
            else
            {
                throw new RestException(StatusCodes.Status401Unauthorized, "UserName or Password incorrect!");
            }
        }
    }
}
