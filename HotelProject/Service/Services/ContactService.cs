using AutoMapper;
using Core.Entities;
using Data.Repositories;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Dtos;
using Service.Dtos.BedType;
using Service.Dtos.Branch;
using Service.Dtos.Contact;
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
        private readonly EmailService _emailService;

        public ContactService(IContactRepository contact, IMapper mapper, UserManager<AppUser> userManager, EmailService emailservice)
        {
            _contact = contact;
            _mapper = mapper;
            _userManager = userManager;
            _emailService = emailservice;

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

        public async Task ContactMessageAdmin(AdminAndIUserInteraction contact)
        {
            Contact userContact = _contact.Get(x=>x.Id == contact.Id);

            if (userContact == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Contact not found");
            }

            string email;
            if (!string.IsNullOrEmpty(userContact.AppUserId))
            {
                var user = await _userManager.FindByIdAsync(userContact.AppUserId);
                if (user == null)
                {
                    throw new RestException(StatusCodes.Status404NotFound, "User", "User not found");
                }
                email = user.Email;
            }
            else
            {
                email = userContact.Email;
            }

            _emailService.Send(email, contact.Subject, contact.Message);
        }


        public List<ContactListItemGetDto> GetAll()
        {
            return _mapper.Map<List<ContactListItemGetDto>>(_contact.GetAll(x=>true)).ToList();
        }

        public PaginatedList<ContactGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _contact.GetAll(x => x.Email.Contains(search) || search == null);
            var paginated = PaginatedList<Contact>.Create(query, page, size);
            return new PaginatedList<ContactGetDto>(_mapper.Map<List<ContactGetDto>>(paginated.Items), paginated.TotalPages, page, size);
        }
    }
}