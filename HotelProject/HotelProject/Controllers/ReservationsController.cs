using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos.Users;
using Service.Services.Interfaces;
using static Service.Exceptions.ResetException;
using System.Security.Claims;
using Service.Services;
using Core.Entities;
using Service.Dtos.Contact;
using Service.Dtos;
using Core.Entities.Enum;
using Service.Dtos.Reservation;
using Microsoft.AspNetCore.SignalR;

namespace HotelProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservation;
        private readonly IHttpContextAccessor _context;
        private readonly IHubContext<ReservationHub> _hubContext;

        public ReservationsController(IReservationService reservationService, IHttpContextAccessor httpContextAccessor, IHubContext<ReservationHub> hubContext)
        {
            _reservation = reservationService;
            _context = httpContextAccessor;
            _hubContext = hubContext;
        }
        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpPost("user")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> CreateReservation([FromBody] ReservationsDto reservationsDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            await _hubContext.Clients.All.SendAsync("ReceiveReservationAlert", "A new reservation has been made.");


            var reservationId = await _reservation.CreateReservationAsync(reservationsDto, userId);
            return Ok(new { ReservationId = reservationId });
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("viewReserves/user")]
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
        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpPost("cancelReservationUser/{reservationId}")]
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
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("")]
        public ActionResult<PaginatedList<MemberReservationGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _reservation.GetAllByPage(search, page, size));
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPut("reservationAccepted/{id}")]
        public IActionResult AcceptReview(int id)
        {

            _reservation.UpdateReservationStatus(id, OrderStatus.Accepted);
            return NoContent();
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPut("reservationRejected/{id}")]
        public IActionResult RejectOrder(int id)
        {

            _reservation.UpdateReservationStatus(id, OrderStatus.Rejected);
            return NoContent();

        }


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("last-12-months-income")]
        public async Task<IActionResult> GetLast12MonthsIncome()
        {
            var count = await _reservation.GetCurrentYearMonthlyIncomeJsonAsync();
            return Ok(count);
            //var monthlyIncome = await _reservation.GetCurrentYearMonthlyIncomeAsync();
            //return Ok(monthlyIncome);
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("count")]
        public async Task<IActionResult> GetTotalReservationsCount()
        {
            var count = await _reservation.GetTotalReservationsCountAsync();
            return Ok(new { TotalReservations = count });
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("total-price")]
        public async Task<IActionResult> GetTotalReservationPrice()
        {
            double totalPrice = await _reservation.GetTotalReservationPriceAsync();
            return Ok(new { TotalPrice = totalPrice });
        }

    }
}