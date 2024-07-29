using HotelUI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace HotelUI.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
