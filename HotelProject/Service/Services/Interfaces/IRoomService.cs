using Service.Dtos.Branch;
using Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Dtos.Room;

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
    }
}
