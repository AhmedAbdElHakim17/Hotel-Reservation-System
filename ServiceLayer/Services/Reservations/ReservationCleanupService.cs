using HRS_BussinessLogic.DTOs.Queries;
using HRS_BussinessLogic.Models;
using HRS_DataAccess;
using HRS_DataAccess.Models;
using HRS_SharedLayer.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stripe.Checkout;
using Stripe;

namespace HRS_ServiceLayer.Services.Reservations
{
    public class ReservationCleanupService
    {
        private readonly ILogger<ReservationCleanupService> logger;
        private readonly IUnitOfWork unitOfWork;

        public ReservationCleanupService(ILogger<ReservationCleanupService> logger, IUnitOfWork unitOfWork)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }
        public async Task MarkExpiredReservationsAsync()
        {
            var expiredRes = await unitOfWork.Reservations
                .FindAllAsync(
                    r => r.ReservationStatus != ReservationStatus.Expired && (
                    (r.CheckOutDate <= DateTime.Now &&
                         r.ReservationStatus == ReservationStatus.CheckedIn)
                         ||
                         (r.CheckInDate <= DateTime.Now &&
                         (r.ReservationStatus == ReservationStatus.Confirmed ||
                         r.ReservationStatus == ReservationStatus.Pending))), false,
                     nameof(Reservation.Room)
                     );
            foreach (var r in expiredRes)
            {
                if (r.ReservationStatus != ReservationStatus.Confirmed)
                {
                    var previosStatus = r.ReservationStatus;
                    r.ReservationStatus = ReservationStatus.Expired;
                    if (previosStatus == ReservationStatus.CheckedIn && r.Room != null)
                    {
                        r.Room.IsAvailable = true;
                    }
                    logger.LogInformation($"Reservation Status with id {r.Id} is Expired");
                }
                else
                {
                    var sessionService = new SessionService();
                    var allSessions = await sessionService.ListAsync(new SessionListOptions { Limit = 100 });
                    var session = allSessions.FirstOrDefault(s => s.ClientReferenceId == r.Id.ToString());
                    if (session == null || string.IsNullOrEmpty(session.PaymentIntentId))
                        return;
                    var payment = await unitOfWork.Payments.FindAsync(p => p.ReservationId == r.Id, false);
                    if (payment == null || payment.PaymentStatus != PaymentStatus.Paid)
                        return;

                    var refundService = new RefundService();
                    var refund = await refundService.CreateAsync(new RefundCreateOptions
                    {
                        PaymentIntent = session.PaymentIntentId.ToString(),
                    });
                    var previosStatus = r.ReservationStatus;
                    r.ReservationStatus = ReservationStatus.Expired;
                    payment.PaymentStatus = PaymentStatus.Refunded;
                    logger.LogInformation($"Reservation with id {r.Id} is refunded");
                }
            }
            await unitOfWork.CompleteAsync();
        }
    }
}
