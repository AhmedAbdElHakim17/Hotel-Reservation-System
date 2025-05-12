using HRS_BussinessLogic.DTOs.Commands;
using HRS_BussinessLogic.DTOs.Queries;
using HRS_BussinessLogic.Models;
using System.Security.Claims;

namespace HRS_ServiceLayer.IServices
{
    public interface IUserService
    {
        public Task<AppUser> GetCurrentUserAsync(ClaimsPrincipal user);
        Task<ResponseDTO<UserGetDTO>> RegisterAsync(UserPostDTO userPostDTO);
        Task<ResponseDTO<AuthDTO>> LoginAsync(LoginDTO loginDTO);
        Task<ResponseDTO<RoleDTO>> GetUserRoleAsync(ClaimsPrincipal claims);
    }
}
