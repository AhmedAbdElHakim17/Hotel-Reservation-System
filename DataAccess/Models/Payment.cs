using HRS_BussinessLogic.Models;
using HRS_SharedLayer.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRS_DataAccess.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public DateTime TransactionDate { get; set; }
        [ForeignKey(nameof(Reservation))]
        public int ReservationId { get; set; }

        public Reservation? Reservation { get; set; }
    }
}
