using HRS_BussinessLogic.Models;
using HRS_DataAccess;
using HRS_SharedLayer.Enums;
using Microsoft.Extensions.Logging;

namespace HRS_ServiceLayer.Services.Reservations
{
    public class ReservationStatusService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<ReservationStatusService> logger;

        public ReservationStatusService(IUnitOfWork unitOfWork,
            ILogger<ReservationStatusService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }
        public async Task UpdateRoomAvailabilityAsync()
        {
            var ActiveRes = (await unitOfWork.Reservations
                .FindAllAsync(
                    r => r.CheckInDate <= DateTime.Now &&
                         r.CheckOutDate >= DateTime.Now &&
                         r.ReservationStatus == ReservationStatus.CheckedIn,
                     nameof(Reservation.Room)
                )).ToList();
            foreach (var r in ActiveRes)
            {
                r.Room.IsAvailable = false;
                logger.LogInformation($"Room Number:{r.Room.RoomNum} is now occupied");
                unitOfWork.Reservations.Update(r);
            }
            await unitOfWork.CompleteAsync();
        }
    }
}
