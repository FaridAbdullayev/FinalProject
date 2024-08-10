using Core.Entities;
using Service.Dtos.Room;
using Service.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IReservationService
    {
        Task<int> CreateReservationAsync(ReservationsDto reservationsDto, string userId);
    }
}
