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

        [ApiExplorerSettings(GroupName = "user_v1")]
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
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("")]
        public ActionResult<PaginatedList<ReviewGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _service.GetAllByPage(search, page, size));
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("all")]
        public ActionResult<List<ReviewListItemGetDto>> GetAllReviews()
        {
            return Ok(_service.GetAll());
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("detail/{id}")]
        public IActionResult GetReviewById(int id)
        {
            return StatusCode(200, _service.GetById(id));
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPut("reviewsAccepted/{id}")]
        public IActionResult AcceptReview(int id)
        {

            _service.UpdateOrderStatus(id, ReviewStatus.Accepted);
            return NoContent();
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPut("reviewsRejected/{id}")]
        public IActionResult RejectOrder(int id)
        {

            _service.UpdateOrderStatus(id, ReviewStatus.Rejected);
            return NoContent();

        }
    }
}
