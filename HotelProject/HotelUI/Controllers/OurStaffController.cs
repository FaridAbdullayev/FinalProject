using HotelUI.Exceptions;
using HotelUI.Models.OurStaff;
using HotelUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelUI.Controllers
{
    public class OurStaffController : Controller
    {
        private HttpClient _client;
        private readonly ICrudService _crudService;

        public OurStaffController(ICrudService crudService)
        {
            _client = new HttpClient();
            _crudService = crudService;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                return View(await _crudService.GetAllPaginated<OurStaffListItemDetailGetResponse>("ourstaff", page));
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
        public async Task<IActionResult> Create(OurStaffCreateRequest createRequest)
        {
            if (!ModelState.IsValid) return View();

            try
            {
                await _crudService.CreateFromForm<OurStaffCreateRequest>(createRequest, "ourstaff");
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
            var staff = await _crudService.Get<OurStaffGetResponse>("ourstaff/" + id);

            var data = new OurStaffUpdateRequest()
            {
                Name = staff.Name,
                Position = staff.Position,
                Description = staff.Description,
            };
            ViewBag.Image = staff.ImageUrl;
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(OurStaffUpdateRequest editRequest, int id)
        {
            if (!ModelState.IsValid) return View();

            try
            {
                await _crudService.UpdateFormForm<OurStaffUpdateRequest>(editRequest, "ourstaff/" + id);
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
                await _crudService.Delete("ourstaff/" + id);
                return Ok();
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }
    }
}
