using HRS_SharedLayer.Enums;
using System.ComponentModel.DataAnnotations;

namespace HRS_BussinessLogic.DTOs.Queries
{
    public class PaymentDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Amount is required")]
        [Range(100,100000,ErrorMessage ="Amount ranges between 100 and 100,000")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage ="Payment method is required")]
        public PaymentMethod PaymentMethod { get; set; }
        [Required(ErrorMessage = "Payment Status is required")]
        public PaymentStatus PaymentStatus { get; set; }
        [Required(ErrorMessage ="Transaction Date is required")]
        public DateTime TransactionDate { get; set; }
        [Required(ErrorMessage ="Reservation Id is required")]
        [Range(1,int.MaxValue,ErrorMessage ="Reservation ID must be positive")]
        public int ReservationId { get; set; }
    }
}
