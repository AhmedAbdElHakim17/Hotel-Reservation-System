using System.ComponentModel.DataAnnotations.Schema;

namespace HRS_BussinessLogic.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        public string? Comment { get; set; }
        public DateTime SubmittedAt { get; set; }

        public int Rating { get; set; }
        [ForeignKey(nameof(User))]
        public string? UserId { get; set; }
        public AppUser? User { get; set; }
        [ForeignKey(nameof(Reservation))]
        public int ReservationId { get; set; }
        public Reservation? Reservation { get; set; }
    }
}
