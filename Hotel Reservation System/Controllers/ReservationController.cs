using HRS_BussinessLogic.DTOs.Commands;
using HRS_Presentation.Middlewares;
using HRS_ServiceLayer.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRS.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController(ILogger<LoggingMiddleware> logger,
        IReservationService reservationService,
        IUserService userService, IEmailService emailService) : ControllerBase
    {
        [HttpGet("All")]
        [Authorize(Roles = "Admin,HotelStaff")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await reservationService.GetAllReservationsAsync();
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Getting All Reservations");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet("AllUpcoming")]
        [Authorize(Roles = "Admin,HotelStaff")]
        public async Task<IActionResult> GetAllUpcoming()
        {
            try
            {
                var response = await reservationService.GetUpcomingReservationsAsync();
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Getting All Reservations");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("MyOwn-Upcoming")]
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> GetMyUpcoming()
        {
            try
            {
                var response = await reservationService.GetUserUpcomingReservationsAsync(User);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Getting All Reservations");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet("MyOwn")]
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> GetMyReservations()
        {
            try
            {
                var response = await reservationService.GetAllUserReservationsAsync(User);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Getting User Reservations");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet("invoice-Pdf/{reservationId}")]
        [Authorize(Roles = "Guest,Admin,HotelStaff")]
        public async Task<IActionResult> GetInvoicePdf(int reservationId)
        {
            try
            {
                var response = await emailService.GetInvoicePdfAsync(reservationId);
                if (!response.IsSuccess) 
                    return BadRequest(response.Message);
                return File(response.Data, "application/pdf", $"invoice-{reservationId}.pdf");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Creating Invoice Pdf");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPost("Add")]
        [Authorize(Roles = "Guest,Admin,HotelStaff")]
        public async Task<IActionResult> Add(ReservationPostDTO reservationDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var response = await reservationService.AddReservationAsync(reservationDTO, User);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Adding Reservation");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("Update/{id:int}")]
        [Authorize(Roles = "Guest,Admin,HotelStaff")]
        public async Task<IActionResult> Edit(ReservationPostDTO reservationDTO, int id)
        {
            try
            {
                var response = await reservationService.UpdateReservationAsync(reservationDTO, id);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Updating Reservation");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPatch("CheckIn/{id:int}")]
        [Authorize(Roles = "Admin,HotelStaff")]
        public async Task<IActionResult> CheckIn(int id)
        {
            try
            {
                var response = await reservationService.CheckInReservationAsync(id,User);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Cancelling Reservation");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPatch("Confirm/{id:int}")]
        [Authorize(Roles = "Admin,HotelStaff")]
        public async Task<IActionResult> Confirm(int id)
        {
            try
            {
                var response = await reservationService.ConfirmReservationAsync(id, User);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Cancelling Reservation");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPatch("CheckOut/{id:int}")]
        [Authorize(Roles = "Admin,HotelStaff")]
        public async Task<IActionResult> CheckOut(int id)
        {
            try
            {
                var response = await reservationService.CheckOutReservationAsync(id, User);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Cancelling Reservation");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpDelete("Cancel/{id:int}")]
        [Authorize(Roles = "Guest,Admin,HotelStaff")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await reservationService.CancelReservationAsync(id);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Cancelling Reservation");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
