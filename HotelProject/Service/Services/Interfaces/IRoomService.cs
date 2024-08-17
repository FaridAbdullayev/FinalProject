using Service.Dtos.Branch;
using Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Dtos.Room;
using Service.Dtos.Users;
using Core.Entities;

namespace Service.Services.Interfaces
{
    public interface IRoomService
    {
        int Create(RoomCreateDto createDto);
        RoomGetDto GetById(int id);
        void Update(RoomUpdateDto updateDto, int Id);
        void Delete(int id);
        List<RoomListItemGetDto> GetAll();
        PaginatedList<RoomGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);

        Task<List<RoomGetDto>> GetFilteredRoomsAsync(RoomFilterCriteriaDto criteriaDto);

        List<int> GetReservedRoomIds(DateTime startDate, DateTime endDate);

        RoomPreReservationInfoDto RoomPreReservationInfo(int roomId, DateTime checkIn, DateTime checkOut);
        //double CalculateRoomPrice(Room room, DateTime startDate, DateTime endDate);

        //Task<bool> ReserveRoomAsync(ReservationsDto reservationDto, string userId);
    }
}
