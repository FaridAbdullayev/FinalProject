using AutoMapper;
using Core.Entities;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Service.Dtos.Users;
using Service.Services.Interfaces;
using System;
using System.Collections.Generic;
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
