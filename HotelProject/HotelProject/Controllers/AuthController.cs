using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos;
using Service.Dtos.UserDtos;
using Service.Dtos.Users;
using Service.Services.Interfaces;
using static Service.Exceptions.ResetException;

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

        //[HttpGet("users")]
        //public async Task<IActionResult> CreateUser()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
        //    await _roleManager.CreateAsync(new IdentityRole("Admin"));
        //    await _roleManager.CreateAsync(new IdentityRole("Member"));


        //    AppUser user1 = new AppUser
        //    {
        //        FullName = "SuperAdmin",
        //        UserName = "superadmin",
        //    };

        //    await _userManager.CreateAsync(user1, "SuperAdmin123");

        //    AppUser user2 = new AppUser
        //    {
        //        FullName = "Member",
        //        UserName = "member",
        //    };

        //    await _userManager.CreateAsync(user2, "Member123");

        //    await _userManager.AddToRoleAsync(user1, "SuperAdmin");
        //    await _userManager.AddToRoleAsync(user2, "Member");

        //    return Ok(user1.Id);
        //}

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPost("login")]
        public ActionResult Login(AdminLoginDto loginDto)
        {
            var token = _authService.Login(loginDto);
            return Ok(new { token });
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("profile")]
        public ActionResult Profile()
        {
            var userName = User.Identity.Name;


            var user = _userManager.FindByNameAsync(userName).Result;

            if (user == null)
            {
                return NotFound("User not found.");
            }
            var userDto = new AdminGetDto
            {
                Id = user.Id,
                UserName = user.UserName
            };

            return Ok(userDto);
        }


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("createAdmin")]
        public IActionResult Create(AdminCreateDto createDto)
        {

            return StatusCode(201, new { Id = _authService.Create(createDto) });
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("update/{id}")]
        public IActionResult Update(string id, AdminUpdateDto updateDto)
        {
            _authService.Update(id, updateDto);
            return NoContent();
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPut("updatePassword")]
        public async Task<IActionResult> UpdatePassword(AdminUpdateDto updatePasswordDto)
        {

            await _authService.UpdatePasswordAsync(updatePasswordDto);
            return NoContent();

        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin")] 
        [HttpGet("adminAllByPage")]
        public ActionResult<PaginatedList<AdminPaginatedGetDto>> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var paginatedAdmins = _authService.GetAllByPage(search, page, size);
            return Ok(paginatedAdmins);
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpPost("user/login")]
        public async Task<IActionResult> LoginForUser([FromBody] MemberLoginDto loginDto)
        {

            var token = await _authService.MemberLogin(loginDto);


            return Ok(new { Result = token });

        }


        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpPost("user/register")]
        public ActionResult RegisterForUser([FromBody] MemberRegisterDto registerDto)
        {

            var Id = _authService.MemberRegister(registerDto);
            return Ok(new { Result = Id });

        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpPost("user/verify")]
        public async Task<IActionResult> Verify([FromBody] MemberVerifyDto verifyDto)
        {
            try
            {
                bool isValid = await _authService.VerifyEmailAndToken(verifyDto.Email, verifyDto.Token);
                if (isValid)
                {
                    return Ok("Email and token are valid. You can now reset your password.");
                }
                else
                {
                    return BadRequest("Invalid email or token.");
                }
            }
            catch (RestException ex)
            {
                return StatusCode(ex.Code, ex.Message);
            }
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("user/verifyemail")]
        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid email verification request.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok("Email confirmed successfully!");
            }

            return BadRequest("Failed to confirm email.");
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [Authorize(Roles = "Member")]
        [HttpPost("user/profile/update")]
        public async Task<IActionResult> UpdateProfile([FromBody] MemberProfileEditDto profileEditDto)
        {
            await _authService.UpdateProfile(profileEditDto);
            return Ok(new { message = "Profile updated successfully!" });
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpPost("user/forgetpassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] MemberForgetPasswordDto forgetPasswordDto)
        {
            try
            {
                var resetUrl = await _authService.ForgetPassword(forgetPasswordDto);
                return Ok(new { Message = "Password reset link has been sent to your email.", ResetUrl = resetUrl });
            }
            catch (RestException ex)
            {
                return StatusCode(ex.Code, ex.Message);
            }
        }


        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpPost("user/resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] MemberResetPasswordDto resetPasswordDto)
        {
            try
            {
                await _authService.ResetPassword(resetPasswordDto);
                return Ok("Password has been reset successfully. Please log in with your new password.");
            }
            catch (RestException ex)
            {
                return StatusCode(ex.Code, ex.Message);
            }
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("registered-users-count")]
        public async Task<IActionResult> GetRegisteredUsersCount()
        {
            var usersCount = await _authService.GetRegisteredUsersCount();
            return Ok(new {Count = usersCount});
        }
    }
}
