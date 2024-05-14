using System.Linq.Expressions;

namespace StudentStorage.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(string? includeProperties = null);
        Task<T?> GetAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);
        Task RemoveAsync(T entity);
        Task AddAsync(T entity);
    }
}
