using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos.Users;
using Service.Services.Interfaces;
using System.Threading.Tasks;
using static Service.Exceptions.ResetException;

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
        public async Task<ActionResult> Create(ContactUserDto createDto)
        {
            try
            {
                var contact = await _service.ContactMessage(createDto);
                return StatusCode(201, new { id = contact.Id });
            }
            catch (RestException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
