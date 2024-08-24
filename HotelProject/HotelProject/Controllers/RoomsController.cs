using Hangfire.MemoryStorage.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos;
using Service.Dtos.Room;
using Service.Dtos.Users;
using Service.Services.Interfaces;
using static Service.Exceptions.ResetException;

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

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPost("")]
        public ActionResult Create(RoomCreateDto createDto)
        {
            return StatusCode(201, new { id = _repo.Create(createDto) });
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("all")]
        public ActionResult<List<RoomListItemGetDto>> GetAll()
        {
            return Ok(_repo.GetAll());
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("")]
        public ActionResult<PaginatedList<RoomGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _repo.GetAllByPage(search, page, size));
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("{id}")]
        public ActionResult GetById(int id)
        {
            var data = _repo.GetById(id);
            return StatusCode(200, data);
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _repo.Delete(id);
            return NoContent();
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromForm] RoomUpdateDto UpdateDto)
        {
            _repo.Update(UpdateDto, id);
            return NoContent();
        }
        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("filter/user")]
        public async Task<IActionResult> GetFilteredRooms([FromQuery] RoomFilterCriteriaDto criteriaDto)
        {
            var roomDtos = await _repo.GetFilteredRoomsAsync(criteriaDto);
            return Ok(roomDtos);
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("PreReservationInfo/user")]
        public IActionResult GetRoomPreReservationInfo(int roomId, DateTime checkIn, DateTime checkOut)
        {
            RoomPreReservationInfoDto info = _repo.RoomPreReservationInfo(roomId, checkIn, checkOut);
            return StatusCode(200, info);
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("{roomId}/reviews")]
        public ActionResult<List<MemberReviewGetDto>> GetRoomReviews(int roomId)
        {
            var reviews = _repo.GetRoomReviews(roomId);
            return Ok(reviews);
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("getAll/user")]
        public ActionResult<PaginatedList<MemberRoomGetDto>> GetAllRoomMember(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _repo.UserRoomGetAll(search, page, size));
        }
        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("getById/user/{id}")]
        public ActionResult GetByIdRoomMember(int id)
        {
            var data = _repo.UserGetById(id);
            return StatusCode(200, data);
        }

    }


}
