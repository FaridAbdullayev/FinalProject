using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;
using Service.Dtos.OurStaff;
using Service.Dtos;

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
        [HttpPost("")]
        public ActionResult Create(OurStaffCreateDto createDto)
        {
            return StatusCode(201, new { id = _service.Create(createDto) });
        }

        [HttpGet("{id}")]
        public ActionResult GetById(int id)
        {
            var data = _service.GetById(id);
            return StatusCode(200, data);
        }

        [HttpGet("all")]
        public ActionResult<List<OurStaffListItemGetDto>> GetAllStaff()
        {
            return Ok(_service.GetAll());
        }
        [HttpGet("")]
        public ActionResult<PaginatedList<OurStaffGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _service.GetAllByPage(search, page, size));
        }


        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _service.Delete(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id,OurStaffUpdateDto staffUpdateDto)
        {
            _service.Update(staffUpdateDto, id);
            return NoContent();
        }
    }
}
