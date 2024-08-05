using HotelUI.Exceptions;
using HotelUI.Filters;
using HotelUI.Models.Setting;
using HotelUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelUI.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class SettingController : Controller
    {
        private HttpClient _client;
        private readonly ICrudService _crudService;

        public SettingController(ICrudService crudService)
        {
            _crudService = crudService;
            _client = new HttpClient();
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                var paginatedResponse = await _crudService.GetAllPaginated<SettingListItemDetailGetResponse>("settings", page);

                return View(paginatedResponse);

            }
            catch (HttpException ex)
            {

                if (ex.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Auth");
                }

                return RedirectToAction("Error", "Home");

            }

            catch (System.Exception)
            {
                return RedirectToAction("Error", "Home");
            }

        }

        public async Task<IActionResult> Delete(string key)
        {
            try
            {
                await _crudService.Delete("settings/" + key);
                return Ok();
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }

        public async Task<IActionResult> Edit(string key)
        {
            var setting = await _crudService.Get<SettingUpdateRequest>("settings/" + key);

            if (setting == null)
            {
                return NotFound();
            }

            ViewBag.Key = key;

            return View(setting);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(SettingUpdateRequest editRequest, string key)
        {
            if (!ModelState.IsValid) return View(editRequest);
            try
            {
                await _crudService.Update<SettingUpdateRequest>(editRequest, "settings/" + key);
                return RedirectToAction("index");
            }
            catch (ModelException e)
            {
                foreach (var item in e.Error.Errors)
                {
                    ModelState.AddModelError(item.Key, item.Message);
                }

                return View(editRequest);
            }


        }
    }
}
