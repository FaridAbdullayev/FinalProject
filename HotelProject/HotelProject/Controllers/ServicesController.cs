using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos;
using Service.Services.Interfaces;
using Service.Dtos.Service;
using Service.Dtos.Users;

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
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPost("")]
        public ActionResult Create(ServiceCreateDto createDto)
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
        public ActionResult<List<ServiceListItemGetDto>> GetAllService()
        {
            return Ok(_service.GetAll());
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("")]
        public ActionResult<PaginatedList<ServiceGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
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
        public ActionResult Update(ServiceUpdateDto serviceUpdateDto, int id)
        {
            _service.Update(serviceUpdateDto, id);
            return NoContent();
        }


        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("all/user")]
        public ActionResult<List<ServiceGetDtoForUser>> GetAllServiceUser()
        {
            return Ok(_service.GetAll());
        }
    }
}
