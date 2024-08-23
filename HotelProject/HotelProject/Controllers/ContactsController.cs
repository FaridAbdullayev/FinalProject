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
        [ApiExplorerSettings(GroupName = "admin_v1")]
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
        [ApiExplorerSettings(GroupName = "user_v1")]
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

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("")]
        public ActionResult<PaginatedList<ContactGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _service.GetAllByPage(search, page, size));
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("all")]
        public ActionResult<List<ContactListItemGetDto>> GetAllContact()
        {
            return Ok(_service.GetAll());
        }
    }
}
