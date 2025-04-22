using System.Linq.Expressions;

namespace HRS_SharedLayer.Interfaces.IBases
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync(params string[] includes);
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate, params string[] includes);
        Task<List<T>> FindAllAsync(Expression<Func<T, bool>> predicate, params string[] includes);

        Task<T> AddAsync(T entity);
        T Update(T entity);
        void Delete(T entity);
    }
}
