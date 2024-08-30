using AutoMapper;
using Core.Entities;
using Core.Entities.Enum;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Dtos;
using Service.Dtos.Contact;
using Service.Dtos.Reservation;
using Service.Dtos.Room;
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
    public class ReservationService : IReservationService
    {

        private readonly IReservationRepository _repo;
        private readonly IRoomRepository _roomRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly EmailService _emailService;
        public ReservationService(IReservationRepository repo, UserManager<AppUser> userManager, IRoomRepository roomRepo, IMapper mapper, EmailService emailService)
        {
            _repo = repo;
            _userManager = userManager;
            _roomRepo = roomRepo;
            _mapper = mapper;
            _emailService = emailService;
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
        public async Task<List<MemberReservationGetDto>> GetUserReservationsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "UserId", "User not found by given UserId");
            }

            var reservations = _repo.GetAll(r => r.AppUserId == userId).Where(x => x.Status == OrderStatus.Accepted)
                                    .Include(r => r.Room) // Oda bilgilerini de dahil etmek için
                                    .ToList();
            // Rezervasyonları Dto'ya dönüştürün
            var reservationDtos = reservations.Select(r => new MemberReservationGetDto
            {
                Id = r.Id,
                RoomName = r.Room.Name,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                Status = r.Status,
                Night = (r.EndDate - r.StartDate).Days,
                TotalPrice = CalculateTotalPrice(r.StartDate, r.EndDate, r.Room.Price)
            }).ToList();

            return reservationDtos;
        }
        private double CalculateTotalPrice(DateTime startDate, DateTime endDate, double roomPrice)
        {
            int nights = (endDate - startDate).Days;
            return nights * roomPrice;
        }
        public async Task CancelReservationAsync(int reservationId, string userId)
        {
            var reservation = _repo.Get(r => r.Id == reservationId && r.AppUserId == userId);

            if (reservation == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "ReservationId", "Reservation not found or you do not have permission to cancel this reservation.");
            }

            reservation.Status = OrderStatus.Canceled;
            _repo.Save();
        }
        public PaginatedList<ReservationGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _repo.GetAll(x => true, "AppUser", "Room");
            var paginated = PaginatedList<Reservation>.Create(query, page, size);
            return new PaginatedList<ReservationGetDto>(_mapper.Map<List<ReservationGetDto>>(paginated.Items), paginated.TotalPages, page, size);
        }
        public async Task UpdateReservationStatus(int id, OrderStatus newStatus)
        {
            Reservation reserv = _repo.Get(o => o.Id == id, "AppUser");

            if (reserv == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Reservation not found");
            }

            if (reserv.Status == newStatus)
            {
                throw new RestException(StatusCodes.Status400BadRequest, $"Reservation is already {newStatus}");
            }

            reserv.Status = newStatus;
            string subject = "Admin";
            string body = newStatus == OrderStatus.Accepted ? "Rezervasyonunuz Qəbul Edildi" : "Rezervasyonunuz Rəddedildi";
            _emailService.Send(reserv.AppUser?.Email, subject, body);
            _repo.Save();
        }
        public async Task<Dictionary<string, double>> GetCurrentYearMonthlyIncomeJsonAsync()
        {
            DateTime currentDate = DateTime.Now;
            DateTime startOfYear = new DateTime(currentDate.Year, 1, 1);
            DateTime endOfYear = new DateTime(currentDate.Year, 12, 31);

            var reservations = await _repo.GetAll(r => r.StartDate >= startOfYear && r.EndDate <= endOfYear, "Room").Where(x => x.Status == OrderStatus.Accepted)
                                          .ToListAsync();

            var monthlyIncomes = Enumerable.Range(1, 12)
                .Select(month => new
                {
                    MonthName = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }[month - 1],
                    TotalIncome = reservations
                        .Where(r => r.StartDate.Year == currentDate.Year && r.StartDate.Month == month)
                        .Sum(r => CalculateTotalPrice(r.StartDate, r.EndDate, r.Room.Price))
                })
                .OrderBy(m => Array.IndexOf(new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }, m.MonthName))
                .ToList();

            var result = monthlyIncomes.ToDictionary(
                m => m.MonthName,
                m => m.TotalIncome
            );

            return result;
        }

        public async Task<int> GetTotalReservationsCountAsync()
        {
            return await _repo.GetAll(x => true).Where(x => x.Status == OrderStatus.Accepted).CountAsync();
        }
        //public async Task<double> GetTotalReservationPriceAsync()
        //{
        //    var reservations = await _repo.GetAll(r => true, "Room").ToListAsync();

        //    double totalPrice = reservations.Sum(r => (r.EndDate - r.StartDate).TotalDays * r.Room.Price);

        //    return totalPrice;
        //}

        public async Task<double> GetTotalReservationPriceAsync()
        {
            var currentDate = DateTime.Now;

            var lastWeekDate = currentDate.AddDays(-7);

            var reservations = await _repo.GetAll(r => r.EndDate >= lastWeekDate && r.StartDate <= currentDate, "Room").Where(x => x.Status == OrderStatus.Accepted).ToListAsync();

            double totalWeeklyPrice = reservations.Sum(r => (r.EndDate - r.StartDate).TotalDays * r.Room.Price);

            return totalWeeklyPrice;
        }

    }
}
