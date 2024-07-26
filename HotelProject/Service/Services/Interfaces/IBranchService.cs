using Service.Dtos;
using Service.Dtos.Branch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IBranchService
    {
        int Create(BranchCreateDto createDto);
        BranchGetDto GetById(int id);
        void Update(BranchUpdateDto updateDto, int Id);
        void Delete(int id);
        List<BranchListItemGetDto> GetAll();
        PaginatedList<BranchGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
    }
}
