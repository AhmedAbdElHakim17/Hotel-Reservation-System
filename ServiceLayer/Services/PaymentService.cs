﻿using AutoMapper;
using HRS_DataAccess;
using HRS_DataAccess.Models;
using HRS_ServiceLayer.IServices;
using HRS_SharedLayer.Enums;
using Microsoft.AspNetCore.Http;
using Stripe;
using Stripe.Checkout;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using PaymentMethod = HRS_SharedLayer.Enums.PaymentMethod;
using HRS_BussinessLogic.Models;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using HRS_BussinessLogic.DTOs.Queries;
namespace HRS_ServiceLayer.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper,
            IConfiguration configuration, IEmailService emailService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.configuration = configuration;
            this.emailService = emailService;
        }
        public async Task<ResponseDTO<PaymentDTO>> AddPaymentsAsync(PaymentDTO paymentDTO)
        {
            try
            {
                var payment = mapper.Map<Payment>(paymentDTO);
                await unitOfWork.Payments.AddAsync(payment);
                await unitOfWork.CompleteAsync();

                var resultDTO = mapper.Map<PaymentDTO>(payment);
                return new ResponseDTO<PaymentDTO>("Payment added successfully", resultDTO);
            }
            catch (Exception ex)
            {
                return HandleError<PaymentDTO>("Failed To add payment", ex);
            }
        }

        public async Task<ResponseDTO<List<PaymentDTO>>> GetAllPaymentsAsync()
        {
            try
            {
                var payments = await unitOfWork.Payments.GetAllAsync(true);
                if (payments == null) return new ResponseDTO<List<PaymentDTO>>("Payments not found", null);

                var resultDTO = mapper.Map<List<PaymentDTO>>(payments);
                return new ResponseDTO<List<PaymentDTO>>("Payment retrieved successfully", resultDTO);
            }
            catch (Exception ex)
            {
                return HandleError<List<PaymentDTO>>("Failed to get all payments", ex);
            }
        }
        public async Task<ResponseDTO<Session>> CreatePaymentIntentAsync(decimal amount, int reservationId, ClaimsPrincipal User)
        {
            try
            {
                var user = await unitOfWork.UserManager.FindByNameAsync(User.Identity.Name);
                if (user == null) return new ResponseDTO<Session>("User not found", null);

                var reservations = (await unitOfWork.Reservations
                    .FindAllAsync(r => r.UserId == user.Id.ToString() &&
                                       r.Id == reservationId &&
                                       r.CheckOutDate >= DateTime.Now, true));
                if (reservations == null || reservations.Count == 0)
                {
                    return new ResponseDTO<Session>("Invalid Reservation Id, Please enter your reservation Id correctly", null);
                }
                var reservation = await unitOfWork.Reservations.GetByIdAsync(reservationId);
                if (reservation == null)
                    return new ResponseDTO<Session>("Incorrect ReservationId, Please enter The valid one", null);
                if (amount != reservation.TotalAmount)
                    return new ResponseDTO<Session>("Incorrect Input, Please enter the requested amount", null);

                var resPayment = await unitOfWork.Payments.FindAsync(p => p.ReservationId == reservationId, true);
                if (resPayment != null)
                {
                    return new ResponseDTO<Session>("This Reservation is paid before, please try again", null);
                }

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)amount * 100,
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = "Hotel Reservation"
                                },
                            },
                            Quantity = 1,
                        },
                    },
                    CustomerEmail = user.Email,
                    Mode = "payment",
                    SuccessUrl = configuration["Stripe:successUrl"],
                    CancelUrl = configuration["Stripe:cancelUrl"],
                    ClientReferenceId = reservationId.ToString(),
                };

                var service = new SessionService();
                Session session = await service.CreateAsync(options);
                return new ResponseDTO<Session>("Session Succeeded", session);
            }
            catch (Exception ex)
            {
                return HandleError<Session>("Failed to create payment intent", ex);
            }
        }
        public async Task<ResponseDTO<PaymentDTO>> StripeWebhookAsync(HttpContext context)
        {
            var json = await new StreamReader(context.Request.Body).ReadToEndAsync();
            var sigHeader = context.Request.Headers["Stripe-Signature"];
            if (String.IsNullOrEmpty(sigHeader))
                return new ResponseDTO<PaymentDTO>("It's not usable", null);
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    sigHeader,
                    configuration["Stripe:WebhookSecret"],
                    tolerance: 300
                );

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;
                    var reservationId = session.ClientReferenceId;

                    var reservation = await unitOfWork.Reservations
                        .FindAsync(r => r.Id == int.Parse(reservationId),
                        false, nameof(Reservation.Room));
                    if (reservation != null)
                    {
                        reservation.ReservationStatus = ReservationStatus.Confirmed;

                        var payment = new Payment
                        {
                            ReservationId = reservation.Id,
                            Amount = reservation.TotalAmount,
                            PaymentMethod = PaymentMethod.CreditCard,
                            PaymentStatus = PaymentStatus.Paid,
                            TransactionDate = DateTime.Now
                        };

                        await unitOfWork.Payments.AddAsync(payment);
                        await unitOfWork.CompleteAsync();

                        var response = await emailService.GetInvoicePdfAsync(reservation.Id,true);
                        if (!response.IsSuccess)
                            return new ResponseDTO<PaymentDTO>("Invoce doesn't exist", null);
                    }
                }
            }
            catch (Exception ex)
            {
                return HandleError<PaymentDTO>("Invalid Stripe Signature", ex);
            }
            return new ResponseDTO<PaymentDTO>("Succedded", new PaymentDTO());
        }
        private static ResponseDTO<T> HandleError<T>(string message, Exception ex)
        {
            return new ResponseDTO<T>($"Error: {message}-{ex.Message}", default);
        }
    }
}
