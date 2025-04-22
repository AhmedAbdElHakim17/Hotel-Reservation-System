using HRS_BussinessLogic.DTOs.Commands;
using HRS_BussinessLogic.DTOs.Queries;
using HRS_DataAccess;
using HRS_Presentation.Middlewares;
using HRS_ServiceLayer.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HRS.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<LoggingMiddleware> logger;
        private readonly IAdminService adminService;

        public AdminController(ILogger<LoggingMiddleware> logger, IAdminService adminService)
        {
            this.logger = logger;
            this.adminService = adminService;
        }
        [HttpGet("AllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var response = await adminService.GetAllRoles();
                if (response.IsSuccess)
                    return Ok(response);
                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while Retrieving all Roles");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("AllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var response = await adminService.GetAllUsers();
                if (response.IsSuccess)
                    return Ok(response);
                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while Retrieving all Roles");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            try
            {
                var response = await adminService.GetUserById(userId);
                if (response.IsSuccess)
                    return Ok(response);
                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while Adding a Role");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPost("AddRole")]
        public async Task<IActionResult> AddRole(string roleName)
        {
            try
            {
                var response = await adminService.AddRole(roleName);
                if (response.IsSuccess)
                    return Ok(response);
                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while Adding a Role");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser(UserPostDTO userDTO)
        {
            try
            {
                var response = await adminService.AddUser(userDTO);
                if (response.IsSuccess)
                    return Ok(response);
                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while Adding a user");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPost("AddRoleToUser")]
        public async Task<IActionResult> AddRoleToUser(string roleName, string userId)
        {
            try
            {
                var response = await adminService.AddRoleToUser(roleName, userId);
                if (response.IsSuccess)
                    return Ok(response);
                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while Adding a Role to user");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPut("UpdateRole")]
        public async Task<IActionResult> UpdateRole(string roleId, string roleName)
        {
            try
            {
                var response = await adminService.UpdateRole(roleId, roleName);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while updating a Role");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(string userId, UserPostDTO userDTO)
        {
            try
            {
                var response = await adminService.UpdateUser(userId, userDTO);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while updating a user");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpDelete("DeleteRole")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            try
            {
                var response = await adminService.DeleteRole(roleId);
                if (!response.IsSuccess)
                    return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while Deleting a Role");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                var response = await adminService.DeleteUser(userId);
                if (!response.IsSuccess)
                    return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while Deleting a user");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
