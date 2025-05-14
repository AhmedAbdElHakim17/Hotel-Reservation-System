using HRS_BussinessLogic.DTOs.Commands;
using HRS_BussinessLogic.DTOs.Queries;
using HRS_Presentation.Middlewares;
using HRS_ServiceLayer.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRS.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController(IRoomService roomService, ILogger<LoggingMiddleware> logger) : ControllerBase
    {
        [HttpGet("All")]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var response = await roomService.GetAllRoomsAsync();
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Getting all rooms");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("Available")]
        public async Task<IActionResult> GetAllAvailableAsync(DateTime from, DateTime to)
        {
            try
            {
                var response = await roomService.GetAvailableRoomsAsync(from, to);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Getting all available rooms");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var response = await roomService.GetRoomByIdAsync(id);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Getting the room by ID");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("Add")]
        [Authorize(Roles = "Admin,HotelStaff")]
        public async Task<IActionResult> Add(RoomPostDTO roomDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var response = await roomService.AddRoomAsync(roomDTO);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding the room");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("Update/{id:int}")]
        [Authorize(Roles = "Admin,HotelStaff")]
        public async Task<IActionResult> Update(int id, RoomPostDTO roomDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var response = await roomService.UpdateRoomAsync(id, roomDTO);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating the room");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("Delete/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await roomService.DeleteRoomAsync(id);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Deleting the room");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
