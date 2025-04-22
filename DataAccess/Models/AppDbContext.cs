using HRS_BussinessLogic.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HRS_DataAccess.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    }
}
