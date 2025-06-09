using HRS_BussinessLogic.DTOs.Queries;
using HRS_Presentation.Middlewares;
using HRS_ServiceLayer.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRS.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController(ILogger<LoggingMiddleware> logger, IFeedbackService feedbackService) : ControllerBase
    {
        [HttpGet("All")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await feedbackService.GetAllFeedbacksAsync();
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting all feedbacks");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet("MyOwn")]
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> GetMyFeedbacks()
        {
            try
            {
                var response = await feedbackService.GetMyFeedbacksAsync(User);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Getting Your Feedbacks");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPost("Add")]
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> Add(FeedbackDTO feedbackDTO)
        {
            try
            {
                var response = await feedbackService.AddFeedbackAsync(feedbackDTO, User);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Adding a feedback");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPut("Update/{id:int}")]
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> Edit(FeedbackDTO feedbackDTO, int id)
        {
            try
            {
                var response = await feedbackService.UpdateFeedbackAsync(feedbackDTO, id);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Editing the feedback");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpDelete("Delete/{id:int}")]
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await feedbackService.DeleteFeedbackAsync(id);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Deleting the feedback");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
