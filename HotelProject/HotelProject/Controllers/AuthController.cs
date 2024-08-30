using Core.Entities;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos;
using Service.Dtos.UserDtos;
using Service.Dtos.Users;
using Service.Services.Interfaces;
using System.Security.Claims;
using static Service.Exceptions.ResetException;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace HotelProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _authService = authService;
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
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


        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleLogin()
        {

            var response = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (response.Principal == null)
                return BadRequest("Google authentication failed.");


            var email = response.Principal.FindFirstValue(ClaimTypes.Email);
            var fullName = response.Principal.FindFirstValue(ClaimTypes.Name);
            var userName = email;


            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(userName))
                return BadRequest("Incomplete Google account information.");


            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {

                return BadRequest("A user with this email address already exists.");
            }


            user = new AppUser
            {
                Email = email,
                UserName = userName,
                FullName = fullName,
                EmailConfirmed = true,
            };
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                return BadRequest("User creation failed.");
            }


            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleResult.Succeeded)
            {
                return BadRequest("Failed to assign role to user.");
            }

            var token = await GenerateJwtToken(user);
            if (token == null)
                return BadRequest("Login failed. Please try again.");


            var redirectUrl = $"{_configuration["Client:URL"]}/account/ExternalLoginCallback?token={token}";
            return Redirect(redirectUrl);
        }





        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("login-google")]
        public IActionResult Login()
        {
            var props = new AuthenticationProperties { RedirectUri = "api/Auth/signin-google" };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }


        private async Task<string> GenerateJwtToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("FullName", user.FullName)
             };


            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


            var secret = _configuration.GetSection("JWT:Secret").Value;
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration.GetSection("JWT:Issuer").Value,
                audience: _configuration.GetSection("JWT:Audience").Value,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("profileLayout")]
        public ActionResult ProfileForLayout()
        {
            var userName = User.Identity.Name;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            return Ok(new UserProfileDto { UserName = userName, Role = role });
        }






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
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut("update/{id}")]
        public IActionResult Update(string id, AdminUpdateDto updateDto)
        {
            _authService.Update(id, updateDto);
            return NoContent();
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
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
        public async Task<ActionResult> RegisterForUser([FromBody] MemberRegisterDto registerDto)
        {

            var Id = await _authService.MemberRegister(registerDto);
            return Ok(new { Result = Id });

        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpPost("user/verify")]
        public async Task<IActionResult> Verify([FromBody] MemberVerifyDto verifyDto)
        {
            try
            {
                bool isValid = await _authService.VerifyEmailToken(verifyDto.Email, verifyDto.Token);
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
                return Ok("Email confirmed");
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
            var message = await _authService.ForgetPassword(forgetPasswordDto);
            return Ok(new { Message = message });
        }


        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpPost("user/resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] MemberResetPasswordDto resetPasswordDto)
        {
            await _authService.ResetPassword(resetPasswordDto);
            return Ok("Password has been reset");
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("registered-users-count")]
        public async Task<IActionResult> GetRegisteredUsersCount()
        {
            var usersCount = await _authService.GetRegisteredUsersCount();
            return Ok(new { Count = usersCount });
        }
    }
}
