using StudentStorage.Models;

namespace StudentStorage.DataAccess.Repository.IRepository
{
    public interface IRequestRepository : IRepository<Request>
    {
        Task<Request?> GetByIdAsync(int id);
        Task UpdateAsync(Request request);
    }
}
