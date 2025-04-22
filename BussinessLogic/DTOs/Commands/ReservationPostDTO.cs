using HRS_BussinessLogic.Attributes;
using HRS_SharedLayer.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace HRS_BussinessLogic.DTOs.Commands
{
    public class ReservationPostDTO
    {
        public int? Id { get; set; }
        [Required(ErrorMessage ="CheckIn Date is required")]
        public DateTime CheckInDate { get; set; }
        [Required(ErrorMessage ="CheckOut Date is required")]
        [ValidationDates]
        public DateTime CheckOutDate { get; set; }
        [JsonIgnore]
        public decimal TotalAmount { get; set; }
        [JsonIgnore]
        public ReservationStatus ReservationStatus { get; set; } = ReservationStatus.Pending;
        public DateTime CreatedAt { get; set; }
        [Required(ErrorMessage ="User Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string? UserName { get; set; }
        [Required(ErrorMessage ="Room Number is required")]
        [Range(1,200,ErrorMessage ="Room Number must be between 1 and 200")]
        public int RoomNum { get; set; }
    }
}
