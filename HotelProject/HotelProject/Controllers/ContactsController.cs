using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos.BedType;
using Service.Dtos;
using Service.Dtos.Users;
using Service.Services.Interfaces;
using System.Threading.Tasks;
using static Service.Exceptions.ResetException;
using Service.Dtos.Contact;

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

        [HttpPost("admin/message")]
        public async Task<ActionResult> SendMessageToUser(AdminAndIUserInteraction interaction)
        {
            try
            {
                await _service.ContactMessageAdmin(interaction);
                return Ok(new { message = "Email sent successfully" });
            }
            catch (RestException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
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


        [HttpGet("")]
        public ActionResult<PaginatedList<ContactGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _service.GetAllByPage(search, page, size));
        }

        [HttpGet("all")]
        public ActionResult<List<ContactListItemGetDto>> GetAllContact()
        {
            return Ok(_service.GetAll());
        }
    }
}
