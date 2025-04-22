using Microsoft.AspNetCore.Identity;

namespace HRS_BussinessLogic.Models
{
    public class AppUser:IdentityUser
    {
        public List<Reservation>? Reservations { get; set; }
        public List<Feedback>? Feedbacks { get; set; }

    }
}
