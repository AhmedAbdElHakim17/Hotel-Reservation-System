﻿using AutoMapper;
using HRS_BussinessLogic.DTOs.Commands;
using HRS_BussinessLogic.DTOs.Queries;
using HRS_BussinessLogic.Models;
using HRS_DataAccess;
using HRS_DataAccess.Models;
using HRS_ServiceLayer.IServices;
using HRS_SharedLayer.Enums;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using PaymentMethod = HRS_SharedLayer.Enums.PaymentMethod;

namespace HRS_ServiceLayer.Services.Reservations
{
    public class ReservationService(IUnitOfWork unitOfWork, IMapper mapper,
        IConfiguration configuration, IEmailService emailService) : IReservationService
    {
        public async Task<ResponseDTO<List<ReservationGetDTO>>> GetAllReservationsAsync()
        {
            try
            {
                var reservations = await unitOfWork.Reservations.GetAllAsync(true, nameof(Reservation.Room), nameof(Reservation.User));
                var reservationDTOs = mapper.Map<List<ReservationGetDTO>>(reservations);
                return new ResponseDTO<List<ReservationGetDTO>>("Reservations retrieved successfully", reservationDTOs);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<ReservationGetDTO>>($"Error: {ex.Message}", null);
            }
        }
        public async Task<ResponseDTO<List<ReservationGetDTO>>> GetUpcomingReservationsAsync()
        {
            try
            {
                var reservations = await unitOfWork.Reservations
                    .FindAllAsync(r => r.CheckOutDate >= DateTime.Now &&
                                     (r.ReservationStatus == ReservationStatus.Pending ||
                                     r.ReservationStatus == ReservationStatus.Confirmed)
                                    ,true, nameof(Reservation.Room), nameof(Reservation.User));
                var reservationDTOs = mapper.Map<List<ReservationGetDTO>>(reservations);
                return new ResponseDTO<List<ReservationGetDTO>>("Reservations retrieved successfully", reservationDTOs);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<ReservationGetDTO>>($"Error: {ex.Message}", null);
            }
        }
        public async Task<ResponseDTO<List<ReservationGetDTO>>> GetAllUserReservationsAsync(ClaimsPrincipal claims)
        {
            try
            {
                var userId = claims?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (claims == null || userId == null)
                    return new ResponseDTO<List<ReservationGetDTO>>("User not found", null);
                var reservations = await unitOfWork.Reservations.FindAllAsync(r => r.UserId == userId,
                    true,nameof(Reservation.Room), nameof(Reservation.User));
                var reservationDTOs = mapper.Map<List<ReservationGetDTO>>(reservations);
                return new ResponseDTO<List<ReservationGetDTO>>("User reservations retrieved successfully", reservationDTOs);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<ReservationGetDTO>>($"Error: {ex.Message}", null);
            }
        }
        public async Task<ResponseDTO<List<ReservationGetDTO>>> GetUserUpcomingReservationsAsync(ClaimsPrincipal claims)
        {
            try
            {
                var userId = claims?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (claims == null || userId == null)
                    return new ResponseDTO<List<ReservationGetDTO>>("User not found", null);
                var reservations = await unitOfWork.Reservations
                    .FindAllAsync(r => r.UserId == userId &&
                                       r.CheckInDate > DateTime.Now &&
                                       (r.ReservationStatus == ReservationStatus.Pending ||
                                       r.ReservationStatus == ReservationStatus.Confirmed),
                                       true, nameof(Reservation.Room), nameof(Reservation.User));
                var reservationDTOs = mapper.Map<List<ReservationGetDTO>>(reservations);
                return new ResponseDTO<List<ReservationGetDTO>>("User upcoming reservations retrieved successfully", reservationDTOs);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<ReservationGetDTO>>($"Error: {ex.Message}", null);
            }
        }
        public async Task<ResponseDTO<ReservationGetDTO>> GetReservationById(int id)
        {
            try
            {
                var reservation = await unitOfWork.Reservations.GetByIdAsync(id);
                if (reservation == null)
                    return new ResponseDTO<ReservationGetDTO>("Reservation not found", null);
                var reservationDTO = mapper.Map<ReservationGetDTO>(reservation);
                return new ResponseDTO<ReservationGetDTO>("User upcoming reservations retrieved successfully", reservationDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<ReservationGetDTO>($"Error: {ex.Message}", null);
            }
        }

        public async Task<ResponseDTO<ReservationGetDTO>> AddReservationAsync(ReservationPostDTO reservationDTO, ClaimsPrincipal claims)
        {
            try
            {
                var validationResponse = await ValidateReservationAsync(reservationDTO);
                if (!validationResponse.IsSuccess) return validationResponse;
                var room = await unitOfWork.Rooms.FindAsync(r => r.RoomNum == reservationDTO.RoomNum, false);
                if (room == null) return new ResponseDTO<ReservationGetDTO>("Room not found", null);
                var reservation = mapper.Map<Reservation>(reservationDTO);
                reservation.RoomId = room.Id;
                reservation.UserId = claims.FindFirstValue(ClaimTypes.NameIdentifier);
                reservation.TotalAmount = await CalculateReservationTotalAmount(reservation, room);
                reservation.Room = null;
                await unitOfWork.Reservations.AddAsync(reservation);
                await unitOfWork.CompleteAsync();
                await emailService.SendReservationEmailAsync(claims.FindFirstValue(ClaimTypes.Email) ?? "No Email", claims.FindFirstValue(ClaimTypes.Name) ?? "No Username");
                var resultDTO = mapper.Map<ReservationGetDTO>(reservation);
                resultDTO.UserName = claims.FindFirstValue(ClaimTypes.Name) ?? "";
                resultDTO.RoomNum = room.RoomNum;
                return new ResponseDTO<ReservationGetDTO>("Reservation added successfully", resultDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<ReservationGetDTO>($"Error: {ex.Message}", null);
            }
        }

        public async Task<ResponseDTO<ReservationGetDTO>> UpdateReservationAsync(ReservationPostDTO reservationDTO, int id)
        {
            try
            {
                if (id != reservationDTO.Id)
                    return new ResponseDTO<ReservationGetDTO>("ID MisMatch", null);

                var reservation = await unitOfWork.Reservations.GetByIdAsync(id);
                if (reservation == null) return new ResponseDTO<ReservationGetDTO>("Reservation not found", null);
                if (reservation.CheckOutDate < DateTime.Now)
                    return new ResponseDTO<ReservationGetDTO>("Can't Update this reservation, it's Expired or past", null);
                var validationResponse = await ValidateReservationAsync(reservationDTO);
                if (!validationResponse.IsSuccess) return validationResponse;
                mapper.Map(reservationDTO, reservation);
                unitOfWork.Reservations.Update(reservation);
                await unitOfWork.CompleteAsync();

                var resultDTO = mapper.Map<ReservationGetDTO>(reservation);
                return new ResponseDTO<ReservationGetDTO>("Reservation updated successfully", resultDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<ReservationGetDTO>($"Error: {ex.Message}", null);
            }
        }

        public async Task<ResponseDTO<ReservationGetDTO>> CancelReservationAsync(int id)
        {
            try
            {
                var sessionService = new SessionService();
                var allSessions = await sessionService.ListAsync(new SessionListOptions { Limit = 100 });
                var session = allSessions.FirstOrDefault(s => s.ClientReferenceId == id.ToString());
                if (session == null || string.IsNullOrEmpty(session.PaymentIntentId))
                    return new ResponseDTO<ReservationGetDTO>("No matching Stripe session or PaymentIntent found.", null);

                var refundService = new RefundService();
                var refund = await refundService.CreateAsync(new RefundCreateOptions
                {
                    PaymentIntent = session.PaymentIntentId.ToString(),
                });

                var reservation = await unitOfWork.Reservations.FindAsync(r => r.Id == id, false, nameof(Reservation.Room));
                if (reservation == null || reservation.ReservationStatus != ReservationStatus.Confirmed ||
                    reservation.CheckOutDate < DateTime.Now)
                    return new ResponseDTO<ReservationGetDTO>("Reservation not eligable for cancellation", null);

                var payment = await unitOfWork.Payments.FindAsync(p => p.ReservationId == id, false);
                if (payment == null || payment.PaymentStatus != PaymentStatus.Paid)
                    return new ResponseDTO<ReservationGetDTO>("No payment found to refund", null);

                reservation.ReservationStatus = ReservationStatus.Cancelled;
                payment.PaymentStatus = PaymentStatus.Refunded;
                await unitOfWork.CompleteAsync();

                var resultDTO = mapper.Map<ReservationGetDTO>(reservation);
                return new ResponseDTO<ReservationGetDTO>("Reservation cancelled and Refund processed successfully", resultDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<ReservationGetDTO>($"Error: {ex.Message}", null);
            }
        }
        public async Task<ResponseDTO<ReservationGetDTO>> CheckInReservationAsync(int id, ClaimsPrincipal claims)
        {
            try
            {
                var reservation = await unitOfWork.Reservations
                    .FindAsync(r => r.Id == id, false, nameof(Reservation.Room));
                if (reservation == null)
                    return new ResponseDTO<ReservationGetDTO>("Reservation not found", null);
                if (reservation.CheckOutDate < DateTime.Now)
                    return new ResponseDTO<ReservationGetDTO>("Reservation Expired", null);
                if (reservation.CheckInDate > DateTime.Now)
                    return new ResponseDTO<ReservationGetDTO>("Reservation CheckIn Date hasn't come yet", null);
                if (reservation.ReservationStatus != ReservationStatus.Confirmed)
                    return new ResponseDTO<ReservationGetDTO>("Reservation must be confirmed before CheckedIn", null);
                reservation.ReservationStatus = ReservationStatus.CheckedIn;
                reservation.Room.IsAvailable = false;
                var resultDTO = mapper.Map<ReservationGetDTO>(reservation);
                resultDTO.UserName = claims.FindFirstValue(ClaimTypes.Name) ?? "No UserName";
                await unitOfWork.CompleteAsync();
                return new ResponseDTO<ReservationGetDTO>("Reservation CheckedIn", resultDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<ReservationGetDTO>($"Error: {ex.Message}", null);
            }
        }
        public async Task<ResponseDTO<ReservationGetDTO>> ConfirmReservationAsync(int id, ClaimsPrincipal claims)
        {
            try
            {
                var reservation = await unitOfWork.Reservations
                    .FindAsync(r => r.Id == id,false, nameof(Reservation.Room));
                if (reservation == null)
                    return new ResponseDTO<ReservationGetDTO>("Reservation not found", null);
                if (reservation.CheckOutDate < DateTime.Now)
                    return new ResponseDTO<ReservationGetDTO>("Reservation Expired", null);
                if (reservation.ReservationStatus != ReservationStatus.Pending)
                    return new ResponseDTO<ReservationGetDTO>("Reservation must be Pending before Confirmed", null);
                reservation.ReservationStatus = ReservationStatus.Confirmed;

                var payment = await unitOfWork.Payments.FindAsync(p => p.ReservationId == id, false);
                if (payment == null)
                    return new ResponseDTO<ReservationGetDTO>("Payment not found", null);
                payment.PaymentStatus = PaymentStatus.Paid;
                payment.PaymentMethod = PaymentMethod.Cash;

                await unitOfWork.CompleteAsync();

                var resultDTO = mapper.Map<ReservationGetDTO>(reservation);
                resultDTO.UserName = claims.FindFirstValue(ClaimTypes.Name) ?? "No Username";
                return new ResponseDTO<ReservationGetDTO>("Reservation Confirmed", resultDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<ReservationGetDTO>($"Error: {ex.Message}", null);
            }
        }
        public async Task<ResponseDTO<ReservationGetDTO>> CheckOutReservationAsync(int id, ClaimsPrincipal claims)
        {
            try
            {

                var reservation = await unitOfWork.Reservations
                    .FindAsync(r => r.Id == id,false, nameof(Reservation.Room));
                if (reservation == null)
                    return new ResponseDTO<ReservationGetDTO>("Reservation not found", null);
                if (reservation.CheckOutDate < DateTime.Now)
                    return new ResponseDTO<ReservationGetDTO>("Reservation Expired", null);
                if (reservation.ReservationStatus != ReservationStatus.CheckedIn)
                    return new ResponseDTO<ReservationGetDTO>("Reservation must be CheckedIn before CheckedOut", null);
                reservation.ReservationStatus = ReservationStatus.CheckedOut;
                if (reservation.Room == null)
                    return new ResponseDTO<ReservationGetDTO>("Room not found", null);
                reservation.Room.IsAvailable = true;
                var resultDTO = mapper.Map<ReservationGetDTO>(reservation);
                resultDTO.UserName = claims.FindFirstValue(ClaimTypes.Name) ?? "No Username";
                await unitOfWork.CompleteAsync();
                return new ResponseDTO<ReservationGetDTO>("Reservation CheckedOut", resultDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<ReservationGetDTO>($"Error: {ex.Message}", null);
            }

        }
        private async Task<ResponseDTO<ReservationGetDTO>> ValidateReservationAsync(ReservationPostDTO reservationDTO)
        {
            var room = await unitOfWork.Rooms.FindAsync(r => r.RoomNum == reservationDTO.RoomNum, true);
            if (room == null) return new ResponseDTO<ReservationGetDTO>("Room not found", null);

            var overlappingReservations = await unitOfWork.Reservations.FindAllAsync(r =>
                r.RoomId == room.Id &&
                r.Id != reservationDTO.Id &&
                (r.ReservationStatus == ReservationStatus.Confirmed || r.ReservationStatus == ReservationStatus.Pending) &&
                !(reservationDTO.CheckOutDate < r.CheckInDate || reservationDTO.CheckInDate > r.CheckOutDate),true);
            if (overlappingReservations.Any())
            {
                return new ResponseDTO<ReservationGetDTO>("Reservation conflicts with existing reservations", null);
            }
            if ((reservationDTO.CheckOutDate - reservationDTO.CheckInDate).Days < 1)
                return new ResponseDTO<ReservationGetDTO>("Reservation must be at least one day", null);

            return new ResponseDTO<ReservationGetDTO>("Validation successful", mapper.Map<ReservationGetDTO>(reservationDTO));
        }
        private Task<decimal> CalculateReservationTotalAmount(Reservation reservation, Room room)
        {
            var pricePerNight = room.PricePerNight;
            var numberOfDays = (reservation.CheckOutDate - reservation.CheckInDate).Days;
            var totalCost = (decimal)pricePerNight * numberOfDays;
            return ApplyOfferToReservationPaymentAsync(reservation, room, totalCost);
        }
        private async Task<decimal> ApplyOfferToReservationPaymentAsync(Reservation reservation, Room room, decimal totalCost)
        {
            if (reservation == null) return 0;
            var availableOffer = await unitOfWork.Offers.
                FindAsync(o => (o.StartDate.Day <= reservation.CheckInDate.Day &&
                                o.EndDate.Day >= reservation.CheckInDate.Day &&
                                o.RoomType == room.RoomType),true);
            if (availableOffer != null)
            {
                return totalCost * (1 - (decimal)(availableOffer.Discount) / 100m);
            }
            return totalCost;
        }
    }
}

