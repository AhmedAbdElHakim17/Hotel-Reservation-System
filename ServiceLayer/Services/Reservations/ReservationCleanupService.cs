using HRS_BussinessLogic.Models;
using HRS_DataAccess;
using HRS_SharedLayer.Enums;
using Microsoft.Extensions.Logging;

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
                    r => (r.CheckOutDate <= DateTime.Now &&
                         r.ReservationStatus == ReservationStatus.CheckedIn)
                         ||
                         (r.CheckInDate <= DateTime.Now &&
                         (r.ReservationStatus == ReservationStatus.Confirmed ||
                         r.ReservationStatus == ReservationStatus.Pending)),
                     nameof(Reservation.Room)
                     );
            foreach (var r in expiredRes)
            {
                var previosStatus = r.ReservationStatus;
                r.ReservationStatus = ReservationStatus.Expired;
                if (previosStatus == ReservationStatus.CheckedIn && r.Room != null)
                {
                    r.Room.IsAvailable = true;
                    unitOfWork.Rooms.Update(r.Room);
                }
                logger.LogInformation($"Reservation Status with id {r.Id} is Expired");
                unitOfWork.Reservations.Update(r);
            }
            await unitOfWork.CompleteAsync();
        }
    }
}
