using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos;
using Service.Services.Interfaces;
using Service.Dtos.Service;

namespace HotelProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceService _service;

        public ServicesController(IServiceService serviceService)
        {
            _service = serviceService;
        }
        [HttpPost("")]
        public ActionResult Create(ServiceCreateDto createDto)
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
        public ActionResult<List<ServiceListItemGetDto>> GetAllService()
        {
            return Ok(_service.GetAll());
        }
        [HttpGet("")]
        public ActionResult<PaginatedList<ServiceGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
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
        public ActionResult Update(ServiceUpdateDto serviceUpdateDto, int id)
        {
            _service.Update(serviceUpdateDto, id);
            return NoContent();
        }
    }
}
