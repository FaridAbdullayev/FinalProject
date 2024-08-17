using HotelUI.Exceptions;
using HotelUI.Models.Reservation;
using HotelUI.Models.Review;
using HotelUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelUI.Controllers
{
    public class ReservationController : Controller
    {
        private HttpClient _client;
        private readonly ICrudService _crudService;

        public ReservationController(ICrudService crudService)
        {
            _client = new HttpClient();
            _crudService = crudService;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                return View(await _crudService.GetAllPaginated<ReservationListItemDetailGetResponse>("reservations", page));
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
                await _crudService.Status($"reservations/reservationAccepted/{id}");
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
                await _crudService.Status($"reservations/reservationRejected/{id}");
                return RedirectToAction("Index");
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }
    }
}
