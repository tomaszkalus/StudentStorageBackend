using System.Linq.Expressions;

namespace BookStoreMVC.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(string? includeProperties = null);
        Task<T?> GetAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);
        void AddAsync(T entity);
        void RemoveAsync(T entity);
    }
}
