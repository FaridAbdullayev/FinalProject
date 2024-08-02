using AutoMapper;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.Dtos;
using Service.Dtos.UserDtos;
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

        public AuthService(UserManager<AppUser> userManager, IConfiguration configuration , IMapper mapper)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
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
        }

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
        }

        public List<AdminGetDto> GetAll(string? search = null)
        {
            var users = _userManager.Users.ToList();

            var adminUsers = _mapper.Map<List<AdminGetDto>>(users);

            var filteredAdminUsers = adminUsers.Where(adminUser =>
            {
                var user = users.FirstOrDefault(u => u.Id == adminUser.Id);
                var roles = _userManager.GetRolesAsync(user).Result;
                return roles.Contains("Admin");
            }).ToList();


            if (!string.IsNullOrEmpty(search))
            {
                filteredAdminUsers = filteredAdminUsers
                    .Where(u => u.UserName.Contains(search))
                    .ToList();
            }

            return filteredAdminUsers;
        }

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
        }


        //public string Login(UserLoginDto loginDto)
        //{
        //    AppUser? user = _userManager.FindByNameAsync(loginDto.UserName).Result;

        //    if (user == null || !_userManager.CheckPasswordAsync(user, loginDto.Password).Result) throw new RestException(StatusCodes.Status401Unauthorized, "UserName or Password incorrect!");

        //    List<Claim> claims = new List<Claim>();
        //    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
        //    claims.Add(new Claim(ClaimTypes.Name, user.UserName));
        //    claims.Add(new Claim("FullName", user.FullName));

        //    var roles = _userManager.GetRolesAsync(user).Result;

        //    claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList());

        //    string secret = _configuration.GetSection("JWT:Secret").Value;

        //    var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    JwtSecurityToken token = new JwtSecurityToken(
        //        claims: claims,
        //        signingCredentials: creds,
        //        issuer: _configuration.GetSection("JWT:Issuer").Value,
        //        audience: _configuration.GetSection("JWT:Audience").Value,
        //        expires: DateTime.Now.AddDays(3)
        //        );

        //    string tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

        //    return tokenStr;
        //}


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
            claims.Add(new Claim("FullName", user.FullName));

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
        }

        public void Update(string id, AdminUpdateDto updateDto)
        {
            var user = _userManager.FindByIdAsync(id).Result;

            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "User not found.");
            }

            var existingUser = _userManager.FindByNameAsync(updateDto.UserName).Result;
            if (existingUser != null)
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
        }

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
        }
    }
}
