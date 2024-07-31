using HotelUI.Exceptions;
using HotelUI.Models.Slider;
using HotelUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelUI.Controllers
{
    public class SliderController : Controller
    {
        private HttpClient _client;
        private readonly ICrudService _crudService;

        public SliderController(ICrudService crudService)
        {
            _client = new HttpClient();
            _crudService = crudService;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                return View(await _crudService.GetAllPaginated<SliderListItemDetailGetResponse>("sliders", page));
            }
            catch (HttpException e)
            {
                if (e.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("login", "auth");
                }
                else return RedirectToAction("error", "home");
            }
            catch (Exception e)
            {
                return RedirectToAction("error", "home");
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SliderCreateRequest createRequest)
        {
            if (!ModelState.IsValid) return View();

            try
            {
                await _crudService.CreateFromForm<SliderCreateRequest>(createRequest, "sliders");
                return RedirectToAction("index");
            }
            catch (ModelException e)
            {
                foreach (var item in e.Error.Errors) ModelState.AddModelError(item.Key, item.Message);
                return View();
            }
        }


        public async Task<IActionResult> Edit(int id)
        {
            var slider = await _crudService.Get<SliderGetResponse>("sliders/" + id);

            var data = new SliderUpdateRequest()
            {
                Description = slider.Description,
                Order = slider.Order,
            };

            ViewBag.Image = slider.ImageUrl;
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SliderUpdateRequest editRequest, int id)
        {
            if (!ModelState.IsValid) return View();

            try
            {
                await _crudService.UpdateFormForm<SliderUpdateRequest>(editRequest, "sliders/" + id);
                return RedirectToAction("index");
            }
            catch (ModelException e)
            {
                foreach (var item in e.Error.Errors)
                    ModelState.AddModelError(item.Key, item.Message);

                return View();
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _crudService.Delete("sliders/" + id);
                return Ok();
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }
    }
}
