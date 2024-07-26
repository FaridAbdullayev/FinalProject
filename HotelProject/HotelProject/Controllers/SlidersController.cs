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

        [HttpPost("")]
        public ActionResult Create(SliderCreateDto createDto)
        {
            return StatusCode(201, new { id = _sliderService.Create(createDto) });
        }

        [HttpGet("{id}")]
        public ActionResult GetById(int id)
        {
            var data = _sliderService.GetById(id);
            return StatusCode(200, data);
        }

        [HttpGet("all")]
        public ActionResult<List<SliderListItemGetDto>> GetAll()
        {
            return Ok(_sliderService.GetAll());
        }

        [HttpGet("")]
        public ActionResult<PaginatedList<SliderGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _sliderService.GetAllByPage(search, page, size));
        }


        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _sliderService.Delete(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromForm] SliderUpdateDto groupUpdateDto)
        {
            _sliderService.Update(groupUpdateDto, id);
            return NoContent();
        }
    }
}
