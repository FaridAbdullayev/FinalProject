using HotelUI.Exceptions;
using HotelUI.Models.Branch;
using HotelUI.Models.Contact;
using HotelUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelUI.Controllers
{
    public class ContactController : Controller
    {
        private HttpClient _client;
        private readonly ICrudService _crudService;

        public ContactController(ICrudService crudService)
        {
            _client = new HttpClient();
            _crudService = crudService;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                return View(await _crudService.GetAllPaginated<ContactListItemDetailResponse>("contacts", page));
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


        public IActionResult ContactMessageAdmin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ContactMessageAdmin(AdminAndIUserInteraction interaction)
        {
            try
            {
                await _crudService.SendMessageToUser(interaction);
                return RedirectToAction("Index"); 
            }
            catch (HttpException e)
            {
                if (e.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("login", "auth");
                }
                else
                {
                    return RedirectToAction("error", "home");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("error", "home");
            }
        }



    }
}
