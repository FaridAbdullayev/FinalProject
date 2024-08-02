using Service.Dtos;
using Service.Dtos.UserDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IAuthService
    {
        SendingLoginDto Login(AdminLoginDto loginDto);

        string Create(AdminCreateDto createDto);
        PaginatedList<AdminPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        List<AdminGetDto> GetAll(string? search = null);
        void Update(string id, AdminUpdateDto updateDto);
        void Delete(string id);
        Task UpdatePasswordAsync(AdminUpdateDto updatePasswordDto);
    }
}
