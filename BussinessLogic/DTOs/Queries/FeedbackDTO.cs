using System.ComponentModel.DataAnnotations;

namespace HRS_BussinessLogic.DTOs.Queries
{
    public class FeedbackDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "UserName is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string UserName {  get; set; }
        [Required(ErrorMessage ="Reservation Id is required")]
        [Range(1,int.MaxValue,ErrorMessage ="Reservation ID must be Positive")]
        public int ReservationId { get; set; }
        public DateTime SubmittedAt { get; set; }
        [Required(ErrorMessage ="Comment is required")]
        [MaxLength(500,ErrorMessage = "Comment cannot exceed 500 characters.")]
        [MinLength(10, ErrorMessage = "Comment must be at least 10 characters.")]
        public string Comment { get; set; }
        [Required(ErrorMessage ="Rating is required")]
        [Range(1,5,ErrorMessage ="Rating must be between 1 and 5")]
        public int Rating { get; set; }
    }
}
