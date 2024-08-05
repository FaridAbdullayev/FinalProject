using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos.Setting;
using Service.Dtos;
using Service.Services.Interfaces;

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

        [HttpGet("")]
        public ActionResult<PaginatedList<SettingListItemGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _settingService.GetAllByPage(search, page, size));
        }

        [HttpGet("all")]
        public ActionResult<List<SettingGetDto>> GetAll()
        {
            return StatusCode(200, _settingService.GetAll());
        }

        [HttpGet("settings/{key}")]
        public ActionResult<SettingGetDto> GetByKey(string key)
        {
            return StatusCode(200, _settingService.GetByKey(key));
        }

        [HttpPut("{key}")]
        public void Update(string key, SettingUpdateDto updateDto)
        {
            _settingService.Update(key, updateDto);
        }

        [HttpDelete("{key}")]
        public IActionResult Delete(string key)
        {
            _settingService.Delete(key);
            return NoContent();
        }

    }
}