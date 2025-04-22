using HRS_BussinessLogic.Models;
using HRS_DataAccess.IRepos;
using HRS_DataAccess.Models;
using HRS_SharedLayer.Interfaces.IBases;
using Microsoft.AspNetCore.Identity;

namespace HRS_DataAccess
{
    public interface IUnitOfWork: IDisposable
    {
        public IRoomRepo Rooms { get; }
        public IBaseRepository<Reservation> Reservations { get; }
        public IBaseRepository<Offer> Offers { get; }
        public IBaseRepository<Payment> Payments { get; }
        public IBaseRepository<Feedback> Feedbacks { get; }

        public SignInManager<AppUser> SignInManager { get; }
        public RoleManager<IdentityRole> RoleManager { get; }

        public UserManager<AppUser> UserManager { get; }

        Task<int> CompleteAsync();
    }
}
