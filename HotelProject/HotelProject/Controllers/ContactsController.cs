using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos.Branch;
using Service.Dtos.Users;
using Service.Services.Interfaces;

namespace HotelProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _service;

        public ContactsController(IContactService contact)
        {
            _service = contact;
        }
        [HttpPost("")]
        public ActionResult Create(ContactUserDto createDto)
        {
            return StatusCode(201, new { id = _service.ContactMessage(createDto) });
        }
    }
}
