using HotelUI.Exceptions;
using HotelUI.Filters;
using HotelUI.Models.Branch;
using HotelUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelUI.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class BranchController : Controller
    {
        private HttpClient _client;
        private readonly ICrudService _crudService;

        public BranchController(ICrudService crudService)
        {
            _client = new HttpClient();
            _crudService = crudService;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                return View(await _crudService.GetAllPaginated<BranchListItemDetailedGetResponse>("branches", page));
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
        public async Task<IActionResult> Create(BranchCreateRequest createRequest)
        {
            if (!ModelState.IsValid) return View();

            try
            {
                await _crudService.Create<BranchCreateRequest>(createRequest, "branches");
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
            return View(await _crudService.Get<BranchUpdateRequest>("branches/" + id));
        }
        [HttpPost]
        public async Task<IActionResult> Edit(BranchUpdateRequest editRequest, int id)
        {
            if (!ModelState.IsValid) return View();

            try
            {
                await _crudService.Update<BranchUpdateRequest>(editRequest, "branches/" + id);
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
                await _crudService.Delete("branches/" + id);
                return Ok();
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }

    }
}
