using StudentStorage.Models;

namespace BookStoreMVC.DataAccess.Repository.IRepository
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<Course?> GetByIdAsync(int id);
        Task UpdateAsync(Course course);
    }
}
