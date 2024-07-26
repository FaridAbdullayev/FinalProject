using Service.Dtos.Branch;
using Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Dtos.Service;

namespace Service.Services.Interfaces
{
    public interface IServiceService
    {
        int Create(ServiceCreateDto createDto);
        ServiceGetDto GetById(int id);
        void Update(ServiceUpdateDto updateDto, int Id);
        void Delete(int id);
        List<ServiceListItemGetDto> GetAll();
        PaginatedList<ServiceGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
    }
}
