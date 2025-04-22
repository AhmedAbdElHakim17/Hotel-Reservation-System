using HRS_BussinessLogic.DTOs.Commands;
using HRS_BussinessLogic.DTOs.Queries;
using Microsoft.AspNetCore.Identity;

namespace HRS_ServiceLayer.IServices
{
    public interface IAdminService
    {
        Task<ResponseDTO<List<RoleDTO>>> GetAllRoles();
        Task<ResponseDTO<List<UserGetDTO>>> GetAllUsers();
        Task<ResponseDTO<UserGetDTO>> GetUserById(string userId);
        Task<ResponseDTO<RoleDTO>> AddRole(string roleName);
        Task<ResponseDTO<UserGetDTO>> AddUser(UserPostDTO user);
        Task<ResponseDTO<UserGetDTO>> AddRoleToUser(string roleId, string userId);
        Task<ResponseDTO<RoleDTO>> UpdateRole(string roleId, string roleName);
        Task<ResponseDTO<UserGetDTO>> UpdateUser(string roleId, UserPostDTO userDTO);
        Task<ResponseDTO<RoleDTO>> DeleteRole(string roleId);
        Task<ResponseDTO<UserGetDTO>> DeleteUser(string userId);
    }
}
