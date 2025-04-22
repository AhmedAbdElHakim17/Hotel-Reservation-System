using HRS_BussinessLogic.DTOs.Commands;
using HRS_BussinessLogic.Models;
using HRS_Presentation.Middlewares;
using HRS_ServiceLayer.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HRS.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IUserService userService,
        ILogger<LoggingMiddleware> logger) : ControllerBase
    {
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserPostDTO registerDTO)
        {
            try
            {
                var response = await userService.RegisterAsync(registerDTO);
                if (response == null)
                    return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while Registering a user");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginDTO loginDTO)
        {
            try
            {
                var response = await userService.LoginAsync(loginDTO);
                if (response == null) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error While Logging");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
