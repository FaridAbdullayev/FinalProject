using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos.BedType;
using Service.Dtos;
using Service.Dtos.Users;
using Service.Services.Interfaces;
using System.Threading.Tasks;
using static Service.Exceptions.ResetException;
using Service.Dtos.Contact;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("admin/message")]
        public async Task<ActionResult> SendMessageToUser(AdminAndIUserInteraction interaction)
        {
           
                await _service.ContactMessageAdmin(interaction);
                return Ok(new { message = "Email sent successfully" });
        }
        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpPost("")]
        public async Task<ActionResult> Create(ContactUserDto createDto)
        {
           
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
             
                var contact = await _service.ContactMessage(createDto,userId);
                return StatusCode(201, new { id = contact.Id });
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
