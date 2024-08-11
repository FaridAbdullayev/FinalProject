using AutoMapper;
using Core.Entities;
using Core.Entities.Enum;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Dtos;
using Service.Dtos.Contact;
using Service.Dtos.Review;
using Service.Dtos.Users;
using Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Service.Exceptions.ResetException;

namespace Service.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IRoomReviewRepository _repo;
        private readonly IMapper _mapper;
        private readonly IRoomRepository _roomRepository;
        private UserManager<AppUser> _userManager;
        public ReviewService(IRoomReviewRepository repo, IMapper mapper, IRoomRepository roomRepository,UserManager<AppUser> userManager)
        {
            _repo = repo;
            _mapper = mapper;
            _roomRepository = roomRepository;
            _userManager = userManager;
        }
        public async Task<int> MemberReview(MemberRoomReviewDto review, string userId)
        {
            var room = _roomRepository.Get(x => x.Id == review.RoomId && !x.IsDeleted);
            if (room == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "RoomId", "Room not found by given RoomId");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "UserId", "User not found by given UserId");
            }

            var existingReview = _repo.Get(x => x.RoomId == review.RoomId && x.AppUserId == userId);
            if (existingReview != null)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Review", "User has already reviewed this room.");
            }

            var roomReview = new RoomReview
            {
                RoomId = review.RoomId,
                Text = review.Text,
                AppUserId = userId,
                Rate = review.Rate,
            };

            _repo.Add(roomReview);
            _repo.Save();

            return roomReview.Id;
        }


        public List<ReviewListItemGetDto> GetAll()
        {
            return _mapper.Map<List<ReviewListItemGetDto>>(_repo.GetAll(x => true)).ToList();
        }

        public PaginatedList<ReviewGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _repo.GetAll(x => x.Text.Contains(search) || search == null);
            var paginated = PaginatedList<RoomReview>.Create(query, page, size);
            return new PaginatedList<ReviewGetDto>(_mapper.Map<List<ReviewGetDto>>(paginated.Items), paginated.TotalPages, page, size);
        }


        public void UpdateOrderStatus(int id, ReviewStatus newStatus)
        {
            var review = _repo.Get(o => o.Id == id);

            if (review == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Review not found");
            }


            if (review.Status == newStatus)
            {
                throw new RestException(StatusCodes.Status400BadRequest, $"Review is already {newStatus}");
            }


            review.Status = newStatus;

            _repo.Save();
        }
        //public async Task<int> MemberReview(MemberRoomReviewDto review,string userId)
        //{
        //    var room = _roomRepository.Get(x => x.Id == review.RoomId && !x.IsDeleted);
        //    if (room == null)
        //    {
        //        throw new RestException(StatusCodes.Status404NotFound, "RoomId", "Room not found by given RoomId");
        //    }

        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null)
        //    {
        //        throw new RestException(StatusCodes.Status404NotFound, "UserId", "User not found by given UserId");
        //    }

        //    var roomreview = new RoomReview
        //    {
        //        RoomId = review.RoomId,
        //        Text = review.Text,
        //        AppUserId = userId,
        //        Rate = review.Rate,
        //    };

        //    _repo.Add(roomreview);
        //    _repo.Save();

        //    return roomreview.Id;
        //}

    }
}
