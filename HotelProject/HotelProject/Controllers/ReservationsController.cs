using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos.Users;
using Service.Services.Interfaces;
using static Service.Exceptions.ResetException;
using System.Security.Claims;
using Service.Services;

namespace HotelProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservation;
        private readonly IHttpContextAccessor _context;

        public ReservationsController(IReservationService reservationService,IHttpContextAccessor httpContextAccessor)
        {
            _reservation = reservationService;
            _context = httpContextAccessor;
        }

        [HttpPost("")]
        [Authorize(Roles ="Member")]
        public async Task<IActionResult> CreateReservation([FromBody] ReservationsDto reservationsDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var reservationId = await _reservation.CreateReservationAsync(reservationsDto, userId);
            return Ok(new { ReservationId = reservationId });
        }


        [HttpGet("viewReserves/member")]
        public async Task<ActionResult<List<MemberReservationGetDto>>> GetUserReservations()
        {
            var userId = _context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var reservations = await _reservation.GetUserReservationsAsync(userId);

            return Ok(reservations);
        }

        [HttpPost("cancelReservationMember/{reservationId}")]
        public async Task<ActionResult> CancelReservation(int reservationId)
        {
            var userId = _context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            await _reservation.CancelReservationAsync(reservationId, userId);

            return NoContent();
        }

    }
}
