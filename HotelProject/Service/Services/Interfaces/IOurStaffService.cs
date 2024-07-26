using Service.Dtos.Branch;
using Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Dtos.OurStaff;

namespace Service.Services.Interfaces
{
    public interface IOurStaffService
    {
        int Create(OurStaffCreateDto createDto);
        OurStaffGetDto GetById(int id);
        void Update(OurStaffUpdateDto updateDto, int Id);
        void Delete(int id);
        List<OurStaffListItemGetDto> GetAll();
        PaginatedList<OurStaffGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
    }
}
