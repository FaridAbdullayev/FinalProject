using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos.Slider;
using Service.Dtos;
using Service.Services.Interfaces;

namespace HotelProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlidersController : ControllerBase
    {
        private readonly ISliderService _sliderService;

        public SlidersController(ISliderService sliderService)
        {
            _sliderService = sliderService;
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPost("")]
        public ActionResult Create(SliderCreateDto createDto)
        {
            return StatusCode(201, new { id = _sliderService.Create(createDto) });
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("{id}")]
        public ActionResult GetById(int id)
        {
            var data = _sliderService.GetById(id);
            return StatusCode(200, data);
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("all")]
        public ActionResult<List<SliderListItemGetDto>> GetAll()
        {
            return Ok(_sliderService.GetAll());
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("")]
        public ActionResult<PaginatedList<SliderGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _sliderService.GetAllByPage(search, page, size));
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _sliderService.Delete(id);
            return NoContent();
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromForm] SliderUpdateDto groupUpdateDto)
        {
            _sliderService.Update(groupUpdateDto, id);
            return NoContent();
        }
    }
}
