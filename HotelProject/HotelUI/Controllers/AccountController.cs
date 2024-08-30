using HotelUI.Exceptions;
using HotelUI.Extentions;
using HotelUI.Models;
using HotelUI.Models.Admin;
using HotelUI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;

namespace HotelUI.Controllers
{
    public class AccountController : Controller
    {
        private HttpClient _client;
        private readonly ICrudService _crudService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccountController(IHttpContextAccessor httpContextAccessor,ICrudService crudService)
        {
            _client = new HttpClient();
            _httpContextAccessor = httpContextAccessor;
            _crudService = crudService;
        }
        public IActionResult GoogleLogin()
        {
            return View();
        }
        public IActionResult ExternalLoginCallback(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            return Ok("You are logged in 👍");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            var content = new StringContent(JsonSerializer.Serialize(loginRequest, options), System.Text.Encoding.UTF8, "application/json");
            using (var response = await _client.PostAsync("https://localhost:7119/api/auth/login", content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(await response.Content.ReadAsStringAsync(), options);
                    if (loginResponse.Token.PasswordResetRequired)
                    {
                        TempData["ResetUserName"] = loginRequest.UserName;
                        Response.Cookies.Append("token", "Bearer " + loginResponse.Token.Token);

                        _httpContextAccessor.HttpContext.Session.SetBool("PasswordResetRequired", true);

                        return RedirectToAction("ResetPassword");
                    }
                    Response.Cookies.Append("token", "Bearer " + loginResponse.Token.Token);
                    _httpContextAccessor.HttpContext.Session.SetBool("PasswordResetRequired", false);
                    return RedirectToAction("index", "home");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError("", "UserName or Password incorrect!");
                    return View();
                }
                else
                {
                    TempData["Error"] = "Something went wrong";
                }
            }

            return View();
        }
        public IActionResult ResetPassword()
         {
            var userName = TempData["ResetUserName"] as string;


            if (userName == null)
            {
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordAdmin
            {
                UserName = userName,
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordAdmin model)
        {
            if (!ModelState.IsValid)
            {

                return View(model);
            }
            try
            {
                await _crudService.Update<ResetPasswordAdmin>(model, "Auth/updatePassword");

                return RedirectToAction("Login");
            }
            catch (ModelException e)
            {
                foreach (var item in e.Error.Errors)
                {

                    ModelState.AddModelError(item.Key, item.Message);
                }

                return View(model);
            }
        }
        public IActionResult AdminCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AdminCreate(AdminCreateRequest createRequest)
        {
            if (!ModelState.IsValid)
                return View(createRequest);

            try
            {
                await _crudService.CreateForAdmins(createRequest, "Auth/createAdmin");
                return RedirectToAction("ShowAdmin");
            }
            catch (ModelException e)
            {
                foreach (var item in e.Error.Errors)
                    ModelState.AddModelError(item.Key, item.Message);
                return View(createRequest);
            }
        }
        public async Task<IActionResult> ShowAdmin(int page = 1)
        {
            try
            {
                var paginatedResponse = await _crudService.GetAllPaginated<AdminPaginatedGetResponse>("Auth/adminAllByPage", page);

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
        public async Task<IActionResult> Profile()
        {
            var token = HttpContext.Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            var user = await _crudService.Get<AdminGetResponse>("Auth/profile");

            if (user == null)
            {
                return NotFound();
            }

            HttpContext.Session.SetString("UserId", user.Id.ToString());

            var adminProfile = new AdminProfileEditRequest
            {
                UserName = user.UserName
            };

            return View(adminProfile);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(AdminProfileEditRequest editRequest)
        {
            if (!ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (!string.IsNullOrEmpty(userId))
                {
                    HttpContext.Session.SetString("UserId", userId);
                }
                return View(editRequest);
            }

            try
            {
                // `UserId`'yi Session'dan alıyoruz
                var userId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login");
                }

                await _crudService.Update<AdminProfileEditRequest>(editRequest, "Auth/update/" + userId);

                if(Request.Cookies.ContainsKey("token"))
                {
                    Response.Cookies.Delete("token");
                }

                
                return RedirectToAction("Login", "Account");
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
        public async Task<IActionResult> Logout()
        {

            if (Request.Cookies.ContainsKey("token"))
            {
                Response.Cookies.Delete("token");
            }

            return RedirectToAction("Login", "Account");
        }

        
    }
}
