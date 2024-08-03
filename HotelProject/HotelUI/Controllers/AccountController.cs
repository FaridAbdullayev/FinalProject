using HotelUI.Exceptions;
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
                        TempData["Token"] = loginResponse.Token.Token;

                        return RedirectToAction("ResetPassword");
                    }
                    Response.Cookies.Append("token", "Bearer " + loginResponse.Token.Token);
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
            //if (Request.Cookies.ContainsKey("token"))
            //{
            //    Response.Cookies.Delete("token");
            //}

            var userName = TempData["ResetUserName"] as string;
            var token = TempData["Token"] as string;
            if (userName == null)
            {
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordAdmin
            {
                UserName = userName,
                Token = token
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
            var user = await _crudService.Get<AdminGetResponse>("Auth/profile");

            AdminProfileEditRequest adminProfile = new AdminProfileEditRequest
            {
                UserName = user.UserName
            };
            ViewBag.Id = user.Id;

            if (adminProfile == null)
            {
                return NotFound();
            }
            return View(adminProfile);
        }
        [HttpPost]
        public async Task<IActionResult> Profile(AdminProfileEditRequest editRequest, string id)
        {
            if (!ModelState.IsValid)
            {
                TempData["ProfileUpdateError"] = "Please correct the errors and try again.";
                return View(editRequest);
            }

            try
            {
                await _crudService.Update<AdminProfileEditRequest>(editRequest, "Auth/update/" + id);

                if (Request.Cookies.ContainsKey("token"))

                {
                    Response.Cookies.Delete("token");
                }
                return RedirectToAction("login", "account");
            }
            catch (ModelException e)
            {
                foreach (var item in e.Error.Errors)
                {
                    TempData["ProfileUpdateError"] = "Please correct the errors and try again.";
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
