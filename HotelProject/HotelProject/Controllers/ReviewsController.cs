using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos.BedType;
using Service.Dtos;
using Service.Dtos.Branch;
using Service.Dtos.Users;
using Service.Services.Interfaces;
using System.Security.Claims;
using Service.Dtos.Review;
using Core.Entities.Enum;
using Service.Services;

namespace HotelProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _service;

        public ReviewsController(IReviewService reviewService)
        {
            _service = reviewService;
        }
        [HttpPost("member")]
        [Authorize(Roles ="Member")]
        public async Task<ActionResult> MemberReview(MemberRoomReviewDto memberRoomReviewDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var reviewId = await _service.MemberReview(memberRoomReviewDto, userId);
            return Ok(new { ReviewId = reviewId });
        }

        [HttpGet("")]
        public ActionResult<PaginatedList<ReviewGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _service.GetAllByPage(search, page, size));
        }

        [HttpGet("all")]
        public ActionResult<List<ReviewListItemGetDto>> GetAllBedType()
        {
            return Ok(_service.GetAll());
        }

        [HttpPut("reviewsAccepted/{id}")]
        public IActionResult AcceptReview(int id)
        {

            _service.UpdateOrderStatus(id, ReviewStatus.Accepted);
            return NoContent();
        }
        [HttpPut("reviewsRejected/{id}")]
        public IActionResult RejectOrder(int id)
        {

            _service.UpdateOrderStatus(id, ReviewStatus.Rejected);
            return NoContent();

        }
    }
}
