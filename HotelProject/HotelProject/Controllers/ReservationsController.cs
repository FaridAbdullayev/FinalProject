using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos.Users;
using Service.Services.Interfaces;
using static Service.Exceptions.ResetException;
using System.Security.Claims;

namespace HotelProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservation;

        public ReservationsController(IReservationService reservationService)
        {
            _reservation = reservationService;
        }

        [HttpPost("")]
        [Authorize]
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

    }
}
