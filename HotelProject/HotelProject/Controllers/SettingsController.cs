using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos.Setting;
using Service.Dtos;
using Service.Services.Interfaces;
using Service.Dtos.Users;

namespace HotelProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        public ISettingService _settingService;

        public SettingsController(ISettingService settingService)
        {
            _settingService = settingService;
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("")]
        public ActionResult<PaginatedList<SettingListItemGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _settingService.GetAllByPage(search, page, size));
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("all")]
        public ActionResult<List<SettingGetDto>> GetAll()
        {
            return StatusCode(200, _settingService.GetAll());
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("settings/{key}")]
        public ActionResult<SettingGetDto> GetByKey(string key)
        {
            return StatusCode(200, _settingService.GetByKey(key));
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPut("{key}")]
        public void Update(string key, SettingUpdateDto updateDto)
        {
            _settingService.Update(key, updateDto);
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpDelete("{key}")]
        public IActionResult Delete(string key)
        {
            _settingService.Delete(key);
            return NoContent();
        }


        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("member/all")]
        public ActionResult<List<MemberSettingGetDto>> UserGetAll()
        {
            return StatusCode(200, _settingService.UserGetAll());
        }
    }
}