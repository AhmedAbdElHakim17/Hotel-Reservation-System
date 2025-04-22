using HRS_BussinessLogic.DTOs.Queries;
using HRS_Presentation.Middlewares;
using HRS_ServiceLayer.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRS.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfferController(ILogger<LoggingMiddleware> logger, IOfferService offerService) : ControllerBase
    {
        [HttpGet("All")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await offerService.GetAllOffersAsync();
                if (!response.IsSuccess || response.Data.Count == 0) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Retrieving All Offers");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPost("Add")]
        [Authorize(Roles ="Admin,HotelStaff")]
        public async Task<IActionResult> Add([FromBody] OfferDTO offerDTO)
        {
            try
            {
                var response = await offerService.AddOfferAsync(offerDTO);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Adding an Offer");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPut("Update/{id:int}")]
        [Authorize(Roles = "Admin,HotelStaff")]
        public async Task<IActionResult> Update(int id, OfferDTO offerDTO)
        {
            try
            {
                var response = await offerService.UpdateOfferAsync(id, offerDTO);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating an Offer");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpDelete("Delete/{id:int}")]
        [Authorize(Roles = "Admin,HotelStaff")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await offerService.DeleteOfferAsync(id);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Deleting an Offer");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}