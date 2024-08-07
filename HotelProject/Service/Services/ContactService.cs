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

        public async Task<Contact> ContactMessage(ContactUserDto contact)
        {
            Contact contactEntity;

            if (string.IsNullOrEmpty(contact.AppUserId))
            {
                // For non-logged-in users
                if (string.IsNullOrEmpty(contact.FullName) || string.IsNullOrEmpty(contact.Email))
                {
                    throw new RestException(StatusCodes.Status400BadRequest, "ContactUserDto", "FullName and Email are required for non-logged-in users");
                }

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
                // For logged-in users
                var user = await _userManager.FindByIdAsync(contact.AppUserId);

                if (user != null && await _userManager.IsInRoleAsync(user, "member"))
                {
                    contactEntity = new Contact
                    {
                        FullName = user.FullName,
                        Email = user.Email,
                        AppUserId = contact.AppUserId,
                        Message = contact.Message,
                        Subject = contact.Subject,
                        CreatedAt = DateTime.UtcNow
                    };
                }
                else
                {
                    throw new RestException(StatusCodes.Status400BadRequest, "AppUserId", "User not found or not in member role");
                }
            }

            _contact.Add(contactEntity);
            _contact.Save();

            return contactEntity;
        }
    }
}