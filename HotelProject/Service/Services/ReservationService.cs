using AutoMapper;
using Core.Entities;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Dtos.Room;
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
    public class ReservationService : IReservationService
    {

        private readonly IReservationRepository _repo;
        private readonly IRoomRepository _roomRepo;
        private readonly UserManager<AppUser> _userManager;

        public ReservationService(IReservationRepository repo, UserManager<AppUser> userManager, IRoomRepository roomRepo)
        {
            _repo = repo;
            _userManager = userManager;
            _roomRepo = roomRepo;
        }
        public async Task<int> CreateReservationAsync(ReservationsDto reservationsDto, string userId)
        {
            var room = _roomRepo.Get(x => x.Id == reservationsDto.RoomId && !x.IsDeleted);
            if (room == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "RoomId", "Room not found by given RoomId");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "UserId", "User not found by given UserId");
            }

            var existingReservation = _repo.GetAll(r => r.RoomId == reservationsDto.RoomId &&
                                                         r.StartDate < reservationsDto.EndDate &&
                                                         r.EndDate > reservationsDto.StartDate)
                                           .Any();

            if (existingReservation)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "RoomId", "The room is already reserved for the selected dates.");
            }

            Reservation reservation = new Reservation
            {
                RoomId = reservationsDto.RoomId,
                StartDate = reservationsDto.StartDate,
                EndDate = reservationsDto.EndDate,
                AppUserId = userId
            };

            _repo.Add(reservation);
            _repo.Save();

            return reservation.Id;
        }

    }
}
