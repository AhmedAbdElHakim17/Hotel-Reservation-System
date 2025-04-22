using HRS_SharedLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRS_BussinessLogic.DTOs.Queries
{
    public class InvoiceDTO
    {
        public string InvoiceNumber { get; set; }
        public string GuestName { get; set; }
        public string GuestEmail { get; set; }
        public int RoomNumber { get; set; }
        public RoomType RoomType { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfNights { get; set; }
        public double RatePerNight { get; set; }
        public string Discount {  get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
