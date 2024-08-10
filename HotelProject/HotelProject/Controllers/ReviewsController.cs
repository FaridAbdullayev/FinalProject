using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos.Branch;
using Service.Dtos.Users;
using Service.Services.Interfaces;
using System.Security.Claims;

namespace HotelProject.Controllers
{
  
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _service;

        public ReviewsController(IReviewService reviewService)
        {
            _service = reviewService;
        }
        [HttpPost("api/member/Reviews")]
        [Authorize]
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
    }
}
