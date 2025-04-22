using AutoMapper;
using HRS_BussinessLogic.DTOs.Commands;
using HRS_BussinessLogic.DTOs.Queries;
using HRS_BussinessLogic.Models;
using HRS_DataAccess;
using HRS_ServiceLayer.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HRS_ServiceLayer.Services
{
    public class UserService(IUnitOfWork unitOfWork, IMapper mapper
        , IConfiguration configuration) : IUserService
    {
        public async Task<AppUser> GetCurrentUserAsync(ClaimsPrincipal user)
        {
            return await unitOfWork.UserManager.FindByNameAsync(user.Identity.Name);
        }

        public async Task<ResponseDTO<AuthDTO>> LoginAsync(LoginDTO loginDTO)
        {
            try
            {
                var user = await unitOfWork.UserManager.FindByNameAsync(loginDTO.UserName);
                if (user == null)
                    return new ResponseDTO<AuthDTO>("user not found", null);
                bool found = await unitOfWork.UserManager.CheckPasswordAsync(user, loginDTO.Password);
                if (found == true)
                {
                    List<Claim> Claims = new List<Claim>();
                    Claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

                    Claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                    Claims.Add(new Claim(ClaimTypes.Name, user.UserName));

                    var Roles = await unitOfWork.UserManager.GetRolesAsync(user);
                    foreach (var Role in Roles)
                    {
                        Claims.Add(new Claim(ClaimTypes.Role, Role));
                    }
                    SymmetricSecurityKey SignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]));

                    SigningCredentials SignInCreds = new SigningCredentials(SignInKey, SecurityAlgorithms.HmacSha256);

                    JwtSecurityToken token = new JwtSecurityToken(
                         issuer: configuration["JWT:IssuerIP"],
                         audience: configuration["JWT:AudienceIP"],
                         expires: DateTime.Now.AddHours(1),
                         claims: Claims,
                         signingCredentials: SignInCreds
                         );
                    var authDTO = new AuthDTO
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        Expiration = DateTime.Now.AddHours(1)
                    };
                    return new ResponseDTO<AuthDTO>("User's Login succeeded",authDTO);
                }
                return new ResponseDTO<AuthDTO>("User not found", null);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<AuthDTO>($"Error: {ex.Message}", null);
            }
        }

        public async Task<ResponseDTO<UserGetDTO>> RegisterAsync(UserPostDTO userPostDTO)
        {
            try
            {
                AppUser user = new AppUser();
                user.UserName = userPostDTO.UserName;
                user.Email = userPostDTO.Email;
                IdentityResult result = await unitOfWork.UserManager.CreateAsync(user, userPostDTO.Password);
                if (result.Succeeded)
                {
                    IdentityResult result1 = await unitOfWork.UserManager.AddToRoleAsync(user, "Guest");
                    if (result1.Succeeded)
                    {
                        var resultDTO = mapper.Map<UserGetDTO>(user);
                        resultDTO.Role = "Guest";
                        return new ResponseDTO<UserGetDTO>("User registered successfully", resultDTO);
                    }
                }
                return new ResponseDTO<UserGetDTO>("User's register process failed", null);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<UserGetDTO>($"Error: {ex.Message}", null);
            }
        }
    }
}
