using StudentStorage.DataAccess.Repository.IRepository;
using StudentStorage.Models;

namespace StudentStorage.DataAccess.Repository.IRepository
{
    public interface IAssignmentRepository : IRepository<Assignment>
    {
        Task<Assignment?> GetByIdAsync(int id);
        Task UpdateAsync(Assignment assignment);
    }
}