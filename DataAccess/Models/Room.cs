using HRS_BussinessLogic.Models;
using HRS_SharedLayer.Enums;

namespace HRS_DataAccess.Models
{
    public class Room
    {
        public int Id { get; set; }
        public int RoomNum { get; set; }
        public double PricePerNight { get; set; }
        public bool IsAvailable { get; set; }

        public string? ImageUrl {  get; set; }
        public string? Facilities {  get; set; }
        public RoomType?  RoomType { get; set; }
        public List<Reservation>? Reservations { get; set; }
    }
}
