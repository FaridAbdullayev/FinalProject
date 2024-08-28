using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;
using Service.Dtos.OurStaff;
using Service.Dtos;
using Core.Entities;
using Service.Dtos.Users;

namespace HotelProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OurStaffController : ControllerBase
    {
        private readonly IOurStaffService _service;

        public OurStaffController(IOurStaffService staffService)
        {
            _service = staffService;
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPost("")]
        public ActionResult Create(OurStaffCreateDto createDto)
        {
            return StatusCode(201, new { id = _service.Create(createDto) });
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("{id}")]
        public ActionResult GetById(int id)
        {
            var data = _service.GetById(id);
            return StatusCode(200, data);
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("all")]
        public ActionResult<List<OurStaffListItemGetDto>> GetAllStaff()
        {
            return Ok(_service.GetAll());
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("")]
        public ActionResult<PaginatedList<OurStaffGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _service.GetAllByPage(search, page, size));
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _service.Delete(id);
            return NoContent();
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPut("{id}")]
        public ActionResult Update(int id,OurStaffUpdateDto staffUpdateDto)
        {
            _service.Update(staffUpdateDto, id);
            return NoContent();
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("count")]
        public async Task<IActionResult> GetTotalOurStaffCount()
        {
            var count = await _service.OurStaffCount();
            return Ok(new { Count = count });
        }


        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("all/user")]
        public ActionResult<List<OurStaffListItemGetDto>> MemberGetAllStaff()
        {
            return Ok(_service.MemberGetAllOurStaff());
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("all/user/about")]
        public ActionResult<List<OurStaffGetForAboutDto>> MemberGetAllStaffForAbout()
        {
            return Ok(_service.MemberGetAllOurStaff());
        }
    }
}
