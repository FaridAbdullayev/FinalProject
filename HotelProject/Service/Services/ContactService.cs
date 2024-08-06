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
using System.Diagnostics.Contracts;
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
            Contact contactEntity;

            if (string.IsNullOrEmpty(contact.AppUserId))
            {
                contactEntity = new Contact
                {
                    FullName = contact.FullName,
                    Email = contact.Email,
                    Message = contact.Message,
                    Subject = contact.Subject,
                    CreatedAt = DateTime.UtcNow
                };
            }
            else
            {
                var user = await _userManager.FindByIdAsync(contact.AppUserId);

                if (user != null && await _userManager.IsInRoleAsync(user, "member"))
                {
                    contactEntity = _mapper.Map<Contact>(contact);
                    contactEntity.AppUserId = user.Id;
                }
                else
                {
                    throw new RestException(StatusCodes.Status400BadRequest, "AppUserId", "User not found");
                }
            }

            _contact.Add(contactEntity);
            _contact.Save();
        }
    }

}

