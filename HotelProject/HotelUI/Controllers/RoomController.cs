using HotelUI.Exceptions;
using HotelUI.Filters;
using HotelUI.Models.BedTypes;
using HotelUI.Models.Branch;
using HotelUI.Models.Room;
using HotelUI.Models.ServiceModels;
using HotelUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Text.Json;

namespace HotelUI.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class RoomController : Controller
    {
        private HttpClient _client;
        private readonly ICrudService _crudService;

        public RoomController(ICrudService crudService)
        {
            _client = new HttpClient();
            _crudService = crudService;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                return View(await _crudService.GetAllPaginated<RoomListItemDetailGetResponse>("rooms", page));
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


        public async Task<IActionResult> Create()
        {
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, Request.Cookies["token"]);

            ViewBag.Branches = await GetBranches();
            ViewBag.Services = await GetService();
            ViewBag.BedType = await GetBedType();



            if (ViewBag.Branches == null) return RedirectToAction("error", "home");
            if (ViewBag.Services == null) return RedirectToAction("error", "home");
            if (ViewBag.BedType == null) return RedirectToAction("error", "home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoomCreateRequest createRequest)
        {
            if (!ModelState.IsValid) return View();

            try
            {
                await _crudService.CreateFromForm<RoomCreateRequest>(createRequest, "rooms");
                return RedirectToAction("index");
            }
            catch (ModelException e)
            {
                foreach (var item in e.Error.Errors) ModelState.AddModelError(item.Key, item.Message);
                ViewBag.Branches = await _crudService.Get<List<BranchGetResponse>>("branches/all");
                ViewBag.Services = await _crudService.Get<List<ServiceGetResponse>>("services/all");
                ViewBag.BedType = await _crudService.Get<List<BedTypeGetResponse>>("beds/all");
                return View();
            }
        }


        public async Task<IActionResult> Edit(int id)
        {

            var room = await _crudService.Get<RoomGetResponse>("rooms/" + id);

            var data = new RoomUpdateRequest()
            {
                Name = room.Name,
                Price = room.Price,
                Description = room.Description,
                BranchId = room.BranchId,
                BedTypeId = room.BedTypeId,
                MaxAdultsCount = room.MaxAdultsCount,
                MaxChildrenCount = room.MaxChildrenCount,
                Area = room.Area,
                ServiceIds = room.ServiceIds,
            };


            ViewBag.Images = room.Images;
            ViewBag.Branches = await GetBranches();
            ViewBag.Services = await GetService();
            ViewBag.BedType = await GetBedType();
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(RoomUpdateRequest editRequest, int id)
        {
            if (!ModelState.IsValid)
            {
                var room = await _crudService.Get<RoomGetResponse>("rooms/" + id);

                var data = new RoomUpdateRequest()
                {
                    Name = room.Name,
                    Price = room.Price,
                    Description = room.Description,
                    BranchId = room.BranchId,
                    MaxAdultsCount = room.MaxAdultsCount,
                    MaxChildrenCount = room.MaxChildrenCount,
                    BedTypeId = room.BedTypeId,
                    Area = room.Area,
                    ServiceIds = room.ServiceIds,
                };

                ViewBag.Images = room.Images;
                ViewBag.Services = await GetService();
                ViewBag.Branches = await GetBranches();
                ViewBag.BedType = await GetBedType();
                return View(data);
            }
            try
            {
                await _crudService.UpdateFormForm<RoomUpdateRequest>(editRequest, "rooms/" + id);
                return RedirectToAction("index");
            }
            catch (ModelException e)
            {
                foreach (var item in e.Error.Errors)
                    ModelState.AddModelError(item.Key, item.Message);


                ViewBag.Branches = await _crudService.Get<List<BranchGetResponse>>("branches/all");
                ViewBag.Services = await _crudService.Get<List<ServiceGetResponse>>("services/all");
                ViewBag.BedType = await _crudService.Get<List<BedTypeGetResponse>>("beds/all");
                return View();
            }
        }


        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _crudService.Delete("rooms/" + id);
                return Ok();
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }



        private async Task<List<BranchGetResponse>> GetBranches()
        {
            using (var response = await _client.GetAsync("https://localhost:7119/api/branches/all"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var data = JsonSerializer.Deserialize<List<BranchGetResponse>>(await response.Content.ReadAsStringAsync(), options);

                    return data;
                }
            }
            return null;
        }

        private async Task<List<ServiceGetResponse>> GetService()
        {
            using (var response = await _client.GetAsync("https://localhost:7119/api/services/all"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var data = JsonSerializer.Deserialize<List<ServiceGetResponse>>(await response.Content.ReadAsStringAsync(), options);

                    return data;
                }
            }
            return null;
        }

        private async Task<List<BedTypeGetResponse>> GetBedType()
        {
            using (var response = await _client.GetAsync("https://localhost:7119/api/beds/all"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var data = JsonSerializer.Deserialize<List<BedTypeGetResponse>>(await response.Content.ReadAsStringAsync(), options);

                    return data;
                }
            }
            return null;
        }



    }
}
