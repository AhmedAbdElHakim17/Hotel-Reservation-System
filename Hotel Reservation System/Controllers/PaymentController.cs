using HRS_BussinessLogic.DTOs.Queries;
using HRS_Presentation.Middlewares;
using HRS_ServiceLayer.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRS.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(ILogger<LoggingMiddleware> logger, IPaymentService paymentService) : ControllerBase
    {
        [HttpGet("All")]
        [Authorize(Roles = "Admin,HotelStaff")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await paymentService.GetAllPaymentsAsync();
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving Payment History");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet("success")]
        public IActionResult Success()
        {
            return Ok($"🎉 Payment successful");
        }

        [HttpGet("cancel")]
        public IActionResult Cancel()
        {
            return Content("❌ Payment was cancelled");
        }

        [HttpPost("Add")]
        [Authorize(Roles = "Admin,HotelStaff")]
        public async Task<IActionResult> Add([FromBody] PaymentDTO paymentDTO)
        {
            try
            {
                var response = await paymentService.AddPaymentsAsync(paymentDTO);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Adding a Payment");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPost("Pay/{reservationId:int}")]
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] decimal amount, int reservationId)
        {
            try
            {
                var response = await paymentService.CreatePaymentIntentAsync(amount, reservationId, User);
                if (!response.IsSuccess) return BadRequest(response.Message);
                return Ok(new { url = response.Data.Url });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Creating a Payment Intent");
                return StatusCode(500, "Internal Server Error");
            }

        }
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            try
            {
                var response = await paymentService.StripeWebhookAsync(HttpContext);
                if (!response.IsSuccess)
                    return BadRequest(response.Message);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error With Stripe Webhook");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
