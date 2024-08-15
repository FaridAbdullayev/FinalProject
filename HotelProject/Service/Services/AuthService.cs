using AutoMapper;
using Azure.Core;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using Service.Dtos;
using Service.Dtos.UserDtos;
using Service.Dtos.Users;
using Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Service.Exceptions.ResetException;

namespace Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly EmailService _emailService;

        public AuthService(UserManager<AppUser> userManager, IConfiguration configuration, IMapper mapper, SignInManager<AppUser> signInManager, EmailService emailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
            _signInManager = signInManager;
            _emailService = emailService;
        }
        public string Create(AdminCreateDto createDto)
        {
            var existingUser = _userManager.FindByNameAsync(createDto.UserName).Result;
            if (existingUser != null)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "UserName", "UserName already taken");
            }

            var user = new AppUser
            {
                UserName = createDto.UserName,
                IsPasswordResetRequired = true
            };

            var result = _userManager.CreateAsync(user, createDto.Password).Result;
            if (!result.Succeeded)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Password", "Failed to create Admin user.");
            }


            var roleResult = _userManager.AddToRoleAsync(user, "Admin").Result;


            if (!roleResult.Succeeded)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "UserName", "Failed to assign role to Admin user.");
            }


            return user.Id;
        }//
        public void Delete(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;

            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "User not found.");
            }

            var result = _userManager.DeleteAsync(user).Result;

            if (!result.Succeeded)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Failed to delete Admin user.");
            }
        }//
        public PaginatedList<AdminPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var users = _userManager.Users.ToList();


            var adminUsers = _mapper.Map<List<AdminPaginatedGetDto>>(users);


            var filteredAdminUsers = new List<AdminPaginatedGetDto>();

            foreach (var adminUser in adminUsers)
            {
                var user = users.FirstOrDefault(u => u.Id == adminUser.Id);
                var roles = _userManager.GetRolesAsync(user).Result;
                if (roles.Contains("Admin"))
                {
                    filteredAdminUsers.Add(adminUser);
                }
            }


            if (!string.IsNullOrEmpty(search))
            {
                filteredAdminUsers = filteredAdminUsers
                    .Where(u => u.UserName.Contains(search))
                    .ToList();
            }


            var paginatedResult = PaginatedList<AdminPaginatedGetDto>.Create(filteredAdminUsers.AsQueryable(), page, size);

            return paginatedResult;
        }//
        public SendingLoginDto Login(AdminLoginDto loginDto)
        {
            AppUser? user = _userManager.FindByNameAsync(loginDto.UserName).Result;

            if (user == null || !_userManager.CheckPasswordAsync(user, loginDto.Password).Result)
            {
                throw new RestException(StatusCodes.Status401Unauthorized, "UserName or Password incorrect!");
            }
            if (user.IsPasswordResetRequired)
            {
                string resetToken = _userManager.GeneratePasswordResetTokenAsync(user).Result;
                return new SendingLoginDto { Token = resetToken, PasswordResetRequired = true };
            }

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            if (user.FullName != null)
            {
                claims.Add(new Claim("FullName", user.FullName));
            }

            var roles = _userManager.GetRolesAsync(user).Result;

            claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList());

            string secret = _configuration.GetSection("JWT:Secret").Value;

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: creds,
                issuer: _configuration.GetSection("JWT:Issuer").Value,
                audience: _configuration.GetSection("JWT:Audience").Value,
                expires: DateTime.Now.AddDays(2)
            );

            string tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            return new SendingLoginDto { Token = tokenStr, PasswordResetRequired = false };
        }//
        public void Update(string id, AdminUpdateDto updateDto)
        {

            var user = _userManager.FindByIdAsync(id).Result;

            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "User not found.");
            }

            var existingUser = _userManager.FindByNameAsync(updateDto.UserName).Result;
            if (existingUser != null && existingUser.UserName != updateDto.UserName)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "UserName", "UserName already taken");
            }

            user.UserName = updateDto.UserName;

            if (!string.IsNullOrEmpty(updateDto.CurrentPassword) && !string.IsNullOrEmpty(updateDto.NewPassword))
            {

                var passwordCheck = _userManager.CheckPasswordAsync(user, updateDto.CurrentPassword).Result;
                if (!passwordCheck)
                {
                    throw new RestException(StatusCodes.Status400BadRequest, "Current password is incorrect.");
                }


                if (updateDto.NewPassword != updateDto.ConfirmPassword)
                {
                    throw new RestException(StatusCodes.Status400BadRequest, "New password and confirm password do not match.");
                }

                var changePasswordResult = _userManager.ChangePasswordAsync(user, updateDto.CurrentPassword, updateDto.NewPassword).Result;

                if (!changePasswordResult.Succeeded)
                {
                    var errors = string.Join(", ", changePasswordResult.Errors.Select(e => e.Description));
                    throw new RestException(StatusCodes.Status400BadRequest, $"Failed to change password: {errors}");
                }
                user.IsPasswordResetRequired = false;
            }

            var updateResult = _userManager.UpdateAsync(user).Result;

            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                throw new RestException(StatusCodes.Status400BadRequest, $"Failed to update user: {errors}");
            }
        }//
        public async Task UpdatePasswordAsync(AdminUpdateDto updatePasswordDto)
        {
            var user = await _userManager.FindByNameAsync(updatePasswordDto.UserName);
            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "User not found.");
            }

            var passwordCheck = await _userManager.CheckPasswordAsync(user, updatePasswordDto.CurrentPassword);
            if (!passwordCheck)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Current password is incorrect.");
            }

            if (updatePasswordDto.NewPassword != updatePasswordDto.ConfirmPassword)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "New password and confirm password do not match.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, updatePasswordDto.CurrentPassword, updatePasswordDto.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                var errors = string.Join(", ", changePasswordResult.Errors.Select(e => e.Description));
                throw new RestException(StatusCodes.Status400BadRequest, $"Failed to change password: {errors}");
            }

            user.IsPasswordResetRequired = false;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                throw new RestException(StatusCodes.Status400BadRequest, $"Failed to update user: {errors}");
            }
        }//
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
        }//
        public async Task<string> MemberLogin(MemberLoginDto memberLoginDto)
        {
            var user = await _userManager.FindByEmailAsync(memberLoginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, memberLoginDto.Password))
            {
                throw new RestException(StatusCodes.Status401Unauthorized, "UserName or Email incorrect!");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new RestException(StatusCodes.Status401Unauthorized, "Email", "Email not confirmed.");
            }


            var token = await GenerateJwtToken(user);

            return token;
        }//
        public async Task<string> MemberRegister(MemberRegisterDto register)
        {
            if (register.Password != register.ConfirmPassword)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Password and ConfirmPassword do not match.");
            }


            if (_userManager.Users.Any(u => u.Email.ToLower() == register.Email.ToLower()))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Email is already taken.");
            }


            if (_userManager.Users.Any(u => u.UserName.ToLower() == register.UserName.ToLower()))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "UserName is already taken.");
            }


            var appUser = new AppUser
            {
                UserName = register.UserName,
                Email = register.Email,
                FullName = register.FullName,
                IsPasswordResetRequired = false
            };


            var result = await _userManager.CreateAsync(appUser, register.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new RestException(StatusCodes.Status400BadRequest, $"Failed to register user: {errors}");
            }


            var roleResult = await _userManager.AddToRoleAsync(appUser, "member");
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new RestException(StatusCodes.Status400BadRequest, $"Failed to assign role: {errors}");
            }


            var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);


            var url = $"{_configuration["AppSettings:AppBaseUrl"]}/api/auth/user/verifyemail?userId={appUser.Id}&token={Uri.EscapeDataString(token)}";

            var subject = "Email Verification";


            var emailbody = $"<h1><a href=\"{url}\"> Emailinizi tesdiqleyin</a></h1>";


            _emailService.Send(appUser.Email, subject, emailbody);

            return appUser.Id;
        }//
        public async Task<string> ForgetPassword(MemberForgetPasswordDto forgetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordDto.Email);
            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "User not found.");
            }
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Email is not confirmed.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetUrl = $"{_configuration["AppSettings:AppBaseUrl"]}/api/Auth/user/resetpassword?email={user.Email}&token={Uri.EscapeDataString(token)}";

            var subject = "Reset Password Link";

            var body = $"<h1>Reset <a href=\"{resetUrl}\">password</a></h1>";
            _emailService.Send(user.Email, subject, body);

            return resetUrl;
        }//
        public async Task ResetPassword(MemberResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "User not found.");
            }

            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmNewPassword)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "New password and confirm password do not match.");
            }

            var decodedToken = Uri.UnescapeDataString(resetPasswordDto.Token);
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new RestException(StatusCodes.Status400BadRequest, $"Failed to reset password: {errors}");
            }
        }//
        public async Task<bool> VerifyEmailAndToken(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Email is not confirmed.");
            }
            var decodedToken = Uri.UnescapeDataString(token);
            var result = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "ResetPassword", decodedToken);
            if (!result)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Invalid token.");
            }

            return true;
        }// 
        public async Task UpdateProfile(MemberProfileEditDto profileEditDto)
        {
            var user = await _userManager.FindByEmailAsync(profileEditDto.Email);
            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "UserName", "User not found.");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Email", "Email is not confirmed.");
            }

            user.UserName = profileEditDto.UserName;
            user.FullName = profileEditDto.FullName;

            if (_userManager.Users.Any(x => x.Id != user.Id && x.NormalizedEmail == profileEditDto.Email.ToUpper()))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Email", "Email is already taken.");
            }

            if (!string.IsNullOrEmpty(profileEditDto.NewPassword))
            {
                if (string.IsNullOrEmpty(profileEditDto.CurrentPassword))
                {
                    throw new RestException(StatusCodes.Status400BadRequest, "CurrentPassword", "Current password is required.");
                }

                var changePasswordResult = await _userManager.ChangePasswordAsync(user, profileEditDto.CurrentPassword, profileEditDto.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    var errors = string.Join(", ", changePasswordResult.Errors.Select(e => e.Description));
                    throw new RestException(StatusCodes.Status400BadRequest, $"Failed to change password: {errors}");
                }
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                throw new RestException(StatusCodes.Status400BadRequest, $"Failed to update profile: {errors}");
            }
        }//
    }
}
