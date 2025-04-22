using HRS_BussinessLogic.DTOs.Queries;
using Microsoft.AspNetCore.Http;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HRS_ServiceLayer.IServices
{
    public interface IPaymentService
    {
        Task<ResponseDTO<List<PaymentDTO>>> GetAllPaymentsAsync(); 
        Task<ResponseDTO<PaymentDTO>> AddPaymentsAsync(PaymentDTO paymentDTO);
        Task<ResponseDTO<PaymentDTO>> StripeWebhookAsync(HttpContext context);
        Task<ResponseDTO<Session>> CreatePaymentIntentAsync(decimal amount, int reservationId, ClaimsPrincipal User);
    }
}
