using HRS_SharedLayer.Enums;

namespace HRS_DataAccess.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public double Discount{ get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public RoomType? RoomType { get; set; }

    }
}
