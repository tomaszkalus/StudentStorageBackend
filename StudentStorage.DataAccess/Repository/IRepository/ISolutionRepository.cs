using StudentStorage.Models;

namespace StudentStorage.DataAccess.Repository.IRepository
{
    public interface ISolutionRepository : IRepository<Solution>
    {
        Task<Solution?> GetByIdAsync(int id);
        Task UpdateAsync(Solution solution);
    }
}
