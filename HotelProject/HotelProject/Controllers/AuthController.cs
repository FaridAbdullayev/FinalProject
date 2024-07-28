using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos.UserDtos;
using Service.Services.Interfaces;

namespace HotelProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public AuthController(IAuthService authService, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _authService = authService;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet("users")]
        public async Task<IActionResult> CreateUser()
        {
            await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
            await _roleManager.CreateAsync(new IdentityRole("Member"));


            AppUser user1 = new AppUser
            {
                FullName = "SuperAdmin",
                UserName = "superadmin",
            };

            await _userManager.CreateAsync(user1, "SuperAdmin123");

            AppUser user2 = new AppUser
            {
                FullName = "Member",
                UserName = "member",
            };

            await _userManager.CreateAsync(user2, "Member123");

            await _userManager.AddToRoleAsync(user1, "SuperAdmin");
            await _userManager.AddToRoleAsync(user2, "Member");

            return Ok(user1.Id);
        }


        [HttpPost("login")]
        public ActionResult Login(UserLoginDto loginDto)
        {
            var token = _authService.Login(loginDto);
            return Ok(new { token });
        }


        [Authorize]
        [HttpGet("profile")]
        public ActionResult Profile()
        {
            return Ok(User.Identity.Name);
        }
    }
}
