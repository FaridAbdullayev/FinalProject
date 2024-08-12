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
        [HttpPost]
        public async Task<IActionResult> Accept(int id)
        {
            try
            {
                await _crudService.Status($"reviews/reviewsAccepted/{id}");
                return RedirectToAction("Index");
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                await _crudService.Status($"reviews/reviewsRejected/{id}");
                return RedirectToAction("Index");
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                var reviewDetails = await _crudService.Get<ReviewGetResponse>($"reviews/detail/{id}");
                return View(reviewDetails);
            }
            catch (HttpException ex)
            {
                if (ex.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Auth");
                }
                return RedirectToAction("Error", "Home");
            }
        }
    }

}

