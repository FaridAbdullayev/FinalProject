using Core.Entities;
using Service.Dtos.Contact;
using Service.Dtos;
using Service.Dtos.Room;
using Service.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Dtos.Reservation;
using Core.Entities.Enum;

namespace Service.Services.Interfaces
{
    public interface IReservationService
    {
        Task<int> CreateReservationAsync(ReservationsDto reservationsDto, string userId);

        Task<List<MemberReservationGetDto>> GetUserReservationsAsync(string userId);

        Task CancelReservationAsync(int reservationId, string userId);

        PaginatedList<ReservationGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);

        Task UpdateReservationStatus(int id, OrderStatus newStatus);
    }
}
