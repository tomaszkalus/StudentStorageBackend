using StudentStorage.DataAccess.Repository.IRepository;
using StudentStorage.Models;

namespace StudentStorage.DataAccess.Repository.IRepository
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        Task<IEnumerable<Course>> GetUserCreatedCoursesAsync(string userId);
        Task<IEnumerable<Request>> GetRequestsAsync(string userId);
        Task<IEnumerable<Course>> GetCoursesAsync(string userId);
        Task<bool> IsCourseMemberAsync(string userId, int courseId);
        Task<bool> IsCourseAuthorAsync(string userId, int courseId);
    }
}
