using HRS_DataAccess.Models;
using HRS_SharedLayer.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRS_BussinessLogic.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalAmount { get; set; }
        public ReservationStatus ReservationStatus {  get; set; }
        public DateTime CreatedAt { get; set; }
        [ForeignKey(nameof(AppUser))]
        public string? UserId { get; set; }
        public AppUser? User { get; set; }
        [ForeignKey(nameof(Room))]
        public int RoomId { get; set; }
        public Room? Room { get; set; }
    }
}
