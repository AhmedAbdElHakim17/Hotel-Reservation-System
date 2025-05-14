using AutoMapper;
using HRS_BussinessLogic.DTOs.Commands;
using HRS_BussinessLogic.DTOs.Queries;
using HRS_BussinessLogic.Models;
using HRS_DataAccess;
using HRS_ServiceLayer.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HRS_ServiceLayer.Services
{
    public class AdminService(IUnitOfWork unitOfWork, IMapper mapper) : IAdminService
    {
        public async Task<ResponseDTO<List<RoleDTO>>> GetAllRoles()
        {
            try
            {
                var roles = await unitOfWork.RoleManager.Roles.ToListAsync();
                if (roles == null || roles.Count == 0)
                    return new ResponseDTO<List<RoleDTO>>("Roles not found", null);
                return new ResponseDTO<List<RoleDTO>>("Roles retrieved suceessfully", mapper.Map<List<RoleDTO>>(roles));
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<RoleDTO>>($"Error: {ex.Message}", null);
            }
        }
        public async Task<ResponseDTO<List<UserGetDTO>>> GetAllUsers()
        {
            try
            {
                var users = await unitOfWork.UserManager.Users.ToListAsync();
                if (users == null || users.Count == 0)
                    return new ResponseDTO<List<UserGetDTO>>("Users not found", null);

                var userRoles = new List<string>();
                foreach (var item in users)
                {
                    var role = (await unitOfWork.UserManager.GetRolesAsync(item)).FirstOrDefault();
                    userRoles.Add(role ?? "No Role");
                }
                var resultDTO = mapper.Map<List<UserGetDTO>>(users);
                for (var j = 0; j < resultDTO.Count; j++)
                {
                    resultDTO[j].Role = userRoles[j];
                }
                return new ResponseDTO<List<UserGetDTO>>("Users retrieved successfully", resultDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<UserGetDTO>>($"Error: {ex.Message}", null);
            }
        }
        public async Task<ResponseDTO<UserGetDTO>> GetUserById(string userId)
        {
            try
            {
                var user = await unitOfWork.UserManager.FindByIdAsync(userId);
                if (user == null)
                    return new ResponseDTO<UserGetDTO>("User not found", null);
                var userRole = (await unitOfWork.UserManager.GetRolesAsync(user)).FirstOrDefault();
                if (userRole == null)
                    return new ResponseDTO<UserGetDTO>("User doesn't have any roles", null);
                var userDTO = mapper.Map<UserGetDTO>(user);
                userDTO.Role = userRole;
                return new ResponseDTO<UserGetDTO>("User retrieved successfully", userDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<UserGetDTO>($"Error: {ex.Message}", null);
            }
        }
        public async Task<ResponseDTO<RoleDTO>> AddRole(string roleName)
        {
            try
            {
                if (await unitOfWork.RoleManager.RoleExistsAsync(roleName))
                    return new ResponseDTO<RoleDTO>("Role already exists", null);
                IdentityRole role = new IdentityRole();
                role.Name = roleName;
                IdentityResult result = await unitOfWork.RoleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    var resultDTO = mapper.Map<RoleDTO>(role);
                    return new ResponseDTO<RoleDTO>("Role Added Successfully", resultDTO);
                }
                return new ResponseDTO<RoleDTO>($"Adding role failed: {string.Join("; ", result.Errors.Select(e => e.Description))}", null);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<RoleDTO>($"Error: {ex.Message}", null);
            }
        }
        public async Task<ResponseDTO<UserGetDTO>> AddUser(UserPostDTO userDTO)
        {
            try
            {
                var user = mapper.Map<AppUser>(userDTO);
                IdentityResult result = await unitOfWork.UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    var resultDTO = mapper.Map<UserGetDTO>(user);
                    return new ResponseDTO<UserGetDTO>("User Added Successfully", resultDTO);
                }
                return new ResponseDTO<UserGetDTO>($"Adding user failed: {string.Join("; ", result.Errors.Select(e => e.Description))}", null);

            }
            catch (Exception ex)
            {
                return new ResponseDTO<UserGetDTO>($"Error: {ex.Message}", null);
            }
        }
        public async Task<ResponseDTO<UserGetDTO>> AddRoleToUser(string roleName, string userId)
        {
            try
            {
                var user = await unitOfWork.UserManager.FindByIdAsync(userId);
                if (user == null)
                    return new ResponseDTO<UserGetDTO>("User not found", null);
                var role = await unitOfWork.RoleManager.FindByNameAsync(roleName);
                if (role == null)
                    return new ResponseDTO<UserGetDTO>("Role not found", null);
                IdentityResult result = await unitOfWork.UserManager.AddToRoleAsync(user, roleName);
                if (result.Succeeded)
                {
                    var resultDTO = mapper.Map<UserGetDTO>(user);
                    resultDTO.Role = role.Name;
                    return new ResponseDTO<UserGetDTO>("Role Added To User Successfully", resultDTO);
                }
                return new ResponseDTO<UserGetDTO>($"Adding role to user failed: {string.Join("; ", result.Errors.Select(e => e.Description))}", null);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<UserGetDTO>($"Error: {ex.Message}", null);
            }
        }
        public async Task<ResponseDTO<RoleDTO>> UpdateRole(string roleId, string roleName)
        {
            try
            {
                var role = await unitOfWork.RoleManager.FindByIdAsync(roleId);
                if (role == null)
                    return new ResponseDTO<RoleDTO>("Role doesn't Exist", null);
                role.Name = roleName;
                IdentityResult result = await unitOfWork.RoleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    var resultDTO = mapper.Map<RoleDTO>(role);
                    return new ResponseDTO<RoleDTO>("Role updated Successfully", resultDTO);
                }
                return new ResponseDTO<RoleDTO>("Updating role process failed", null);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<RoleDTO>($"Error: {ex.Message}", null);
            }
        }
        public async Task<ResponseDTO<UserGetDTO>> UpdateUser(string userId, UserPostDTO userDTO)
        {
            try
            {
                var user = await unitOfWork.UserManager.FindByIdAsync(userId);
                if (user == null)
                    return new ResponseDTO<UserGetDTO>("User doesn't Exist", null);
                var userRole = (await unitOfWork.UserManager.GetRolesAsync(user)).FirstOrDefault();
                if (userRole == null)
                    return new ResponseDTO<UserGetDTO>("User doesn't have any roles", null);
                mapper.Map(userDTO, user);
                IdentityResult result = await unitOfWork.UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    var resultDTO = mapper.Map<UserGetDTO>(user);
                    resultDTO.Role = userRole;
                    return new ResponseDTO<UserGetDTO>("User updated Successfully", resultDTO);
                }
                return new ResponseDTO<UserGetDTO>("Updating user process failed", null);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<UserGetDTO>($"Error: {ex.Message}", null);
            }
        }
        public async Task<ResponseDTO<RoleDTO>> DeleteRole(string roleId)
        {
            try
            {
                var role = await unitOfWork.RoleManager.FindByIdAsync(roleId);
                if (role == null)
                    return new ResponseDTO<RoleDTO>("Role doesn't Exist", null);
                IdentityResult result = await unitOfWork.RoleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    var resultDTO = mapper.Map<RoleDTO>(role);
                    return new ResponseDTO<RoleDTO>("Role Deleted Successfully", resultDTO);
                }
                return new ResponseDTO<RoleDTO>("Deleting role process failed", null);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<RoleDTO>($"Error: {ex.Message}", null);
            }
        }
        public async Task<ResponseDTO<UserGetDTO>> DeleteUser(string userId)
        {
            try
            {
                var user = await unitOfWork.UserManager.FindByIdAsync(userId);
                if (user == null)
                    return new ResponseDTO<UserGetDTO>("User doesn't Exist", null);
                var userRole = (await unitOfWork.UserManager.GetRolesAsync(user)).FirstOrDefault();
                if (userRole == null)
                    return new ResponseDTO<UserGetDTO>("User doesn't have any roles", null);
                IdentityResult result = await unitOfWork.UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    var resultDTO = mapper.Map<UserGetDTO>(user);
                    resultDTO.Role = userRole;
                    return new ResponseDTO<UserGetDTO>("User Deleted Successfully", resultDTO);
                }
                return new ResponseDTO<UserGetDTO>("Deleting user process failed", null);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<UserGetDTO>($"Error: {ex.Message}", null);
            }
        }
    }
}
