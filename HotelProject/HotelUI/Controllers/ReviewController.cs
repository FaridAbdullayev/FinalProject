using HotelUI.Exceptions;
using HotelUI.Models.Branch;
using HotelUI.Models.Review;
using HotelUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelUI.Controllers
{
    public class ReviewController : Controller
    {
        private HttpClient _client;
        private readonly ICrudService _crudService;

        public ReviewController(ICrudService crudService)
        {
            _client = new HttpClient();
            _crudService = crudService;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                return View(await _crudService.GetAllPaginated<ReviewListItemDetailGetResponse>("reviews", page));
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
    }
}
