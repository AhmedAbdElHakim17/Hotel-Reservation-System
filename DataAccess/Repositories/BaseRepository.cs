using HRS_DataAccess.Models;
using HRS_SharedLayer.Interfaces.IBases;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace HRS_DataAccess.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        #region Fields
        protected readonly AppDbContext context;
        #endregion
        #region Constructor
        public BaseRepository(AppDbContext context)
        {
            this.context = context;
        }
        #endregion
        #region Handles Functions
        public Task<List<T>> GetAllAsync(params string[] includes)
        {
            IQueryable<T> query = context.Set<T>().AsNoTracking();
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);
            return query.ToListAsync();
        }
        public async Task<T?> GetByIdAsync(int id) => await context.Set<T>().FindAsync(id);
        public Task<T?> FindAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            IQueryable<T> query = context.Set<T>().AsNoTracking();
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);
            return query.FirstOrDefaultAsync(predicate);
        }
        public Task<List<T>> FindAllAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            IQueryable<T> query = context.Set<T>().AsNoTracking();
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);
            return query.Where(predicate).ToListAsync();
        }
        public async Task<T> AddAsync(T entity)
        {
            await context.Set<T>().AddAsync(entity);
            return entity;
        }

        public T Update(T entity)
        {
            context.Set<T>().Update(entity);
            return entity;
        }

        public void Delete(T entity)
        {
            context.Set<T>().Remove(entity);
        }
        #endregion
    }
}
