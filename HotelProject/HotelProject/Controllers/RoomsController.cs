using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos;
using Service.Dtos.Room;
using Service.Dtos.Users;
using Service.Services.Interfaces;

namespace HotelProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _repo;

        public RoomsController(IRoomService room)
        {
            _repo = room;
        }
        [HttpPost("")]
        public ActionResult Create(RoomCreateDto createDto)
        {
            return StatusCode(201, new { id = _repo.Create(createDto) });
        }


        [HttpGet("all")]
        public ActionResult<List<RoomListItemGetDto>> GetAll()
        {
            return Ok(_repo.GetAll());
        }

        [HttpGet("")]
        public ActionResult<PaginatedList<RoomGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _repo.GetAllByPage(search, page, size));
        }

        [HttpGet("{id}")]
        public ActionResult GetById(int id)
        {
            var data = _repo.GetById(id);
            return StatusCode(200, data);
        }


        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _repo.Delete(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromForm] RoomUpdateDto UpdateDto)
        {
            _repo.Update(UpdateDto, id);
            return NoContent();
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredRooms([FromQuery] RoomFilterCriteriaDto criteriaDto)
        {
            var roomDtos = await _repo.GetFilteredRoomsAsync(criteriaDto);
            return Ok(roomDtos);
        }
    }
}
