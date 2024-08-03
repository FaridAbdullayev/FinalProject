using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos.BedType;
using Service.Dtos.Branch;
using Service.Dtos;
using Service.Services.Interfaces;

namespace HotelProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BedsController : ControllerBase
    {
        private readonly IBedTypeService _service;

        public BedsController(IBedTypeService bedService)
        {
            _service = bedService;
        }
        [HttpPost("")]
        public ActionResult Create(BedTypeCreateDto createDto)
        {
            return StatusCode(201, new { id = _service.Create(createDto) });
        }
        [HttpGet("")]
        public ActionResult<PaginatedList<BedTypeGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _service.GetAllByPage(search, page, size));
        }

        [HttpGet("all")]
        public ActionResult<List<BedTypeListItemGetDto>> GetAllBedType()
        {
            return Ok(_service.GetAll());
        }


        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _service.Delete(id);
            return NoContent();
        }
    }
}
