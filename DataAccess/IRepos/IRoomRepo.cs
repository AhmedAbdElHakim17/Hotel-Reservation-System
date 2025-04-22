using HRS_DataAccess.Models;
using HRS_SharedLayer.Interfaces.IBases;

namespace HRS_DataAccess.IRepos
{
    public interface IRoomRepo : IBaseRepository<Room>
    {
        public Task<List<Room>> GetAllAvailableAsync(DateTime from, DateTime to);
    }
}
