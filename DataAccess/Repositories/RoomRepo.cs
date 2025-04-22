using HRS_DataAccess.IRepos;
using HRS_DataAccess.Models;
using HRS_SharedLayer.Enums;
using Microsoft.EntityFrameworkCore;
namespace HRS_DataAccess.Repositories
{
    public class RoomRepo : BaseRepository<Room>, IRoomRepo
    {
        public RoomRepo(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Room>> GetAllAvailableAsync(DateTime from, DateTime to)
        {
            var reservardRoomsIds = await context.Reservations.Where(r=> r.CheckInDate < to && r.CheckOutDate > from
                && r.ReservationStatus != ReservationStatus.Cancelled).Select(r => r.RoomId).Distinct().ToListAsync();
            return await context.Rooms.Where(r=>!reservardRoomsIds.Contains(r.Id) && r.IsAvailable).ToListAsync();
            
        }
    }
}
