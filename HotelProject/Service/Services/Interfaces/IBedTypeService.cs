using Service.Dtos.Branch;
using Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Dtos.BedType;

namespace Service.Services.Interfaces
{
    public interface IBedTypeService
    {
        int Create(BedTypeCreateDto createDto);
        void Delete(int id);

      
        List<BedTypeListItemGetDto> GetAll();
        PaginatedList<BedTypeGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
    }
}
