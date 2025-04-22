using HRS_BussinessLogic.Models;
using HRS_DataAccess.IRepos;
using HRS_DataAccess.Models;
using HRS_DataAccess.Repositories;
using HRS_SharedLayer.Interfaces.IBases;
using Microsoft.AspNetCore.Identity;

namespace HRS_DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext context;
        public IRoomRepo Rooms { get; private set; }
        public IBaseRepository<Reservation> Reservations { get; private set; }
        public IBaseRepository<Offer> Offers { get; private set; }
        public IBaseRepository<Payment> Payments { get; private set; }
        public IBaseRepository<Feedback> Feedbacks { get; private set; }

        public SignInManager<AppUser> SignInManager { get; private set; }
        public RoleManager<IdentityRole> RoleManager { get; private set; }
        public UserManager<AppUser> UserManager { get; private set; }

        public UnitOfWork(AppDbContext context, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.context = context;
            SignInManager = signInManager;
            UserManager = userManager;
            RoleManager = roleManager;
            Rooms = new RoomRepo(context);
            Reservations = new BaseRepository<Reservation>(context);
            Offers = new BaseRepository<Offer>(context);
            Payments = new BaseRepository<Payment>(context);
            Feedbacks = new BaseRepository<Feedback>(context);
        }

        public async Task<int> CompleteAsync()
        {
            return await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
