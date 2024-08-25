using Service.Dtos.Setting;
using Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Dtos.Users;

namespace Service.Services.Interfaces
{
    public interface ISettingService
    {
        PaginatedList<SettingListItemGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        List<SettingGetDto> GetAll(string? search = null);
        SettingGetDto GetByKey(string key);
        void Update(string key, SettingUpdateDto updateDto);
        void Delete(string key);

        List<MemberSettingGetDto> UserGetAll(string? search = null);
    }
}
