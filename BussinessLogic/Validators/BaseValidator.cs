using HRS_DataAccess.Models;
using HRS_SharedLayer.Interfaces.IBases;

namespace HRS_BussinessLogic.Validators
{
    public class BaseValidator<T> : IBaseValidator<T> where T : class
    {
        private readonly AppDbContext context;

        public BaseValidator(AppDbContext context)
        {
            this.context = context;
        }

        public bool IsRoomNumberUnique(int num, int roomId)
        {
            return !context.Rooms.Any(r => r.RoomNum == num && r.Id != roomId);
        }
    }
}
