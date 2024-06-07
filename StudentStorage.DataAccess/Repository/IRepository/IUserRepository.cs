using StudentStorage.DataAccess.Repository.IRepository;
using StudentStorage.Models;

namespace StudentStorage.DataAccess.Repository.IRepository
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        Task<IEnumerable<Course>> GetUserCreatedCoursesAsync(int userId);
        Task<IEnumerable<Request>> GetRequestsAsync(int userId);
        Task<IEnumerable<Course>> GetCoursesAsync(int userId);
        Task<bool> IsCourseMemberAsync(int userId, int courseId);
        Task<bool> IsCourseAuthorAsync(int userId, int courseId);
        Task<IEnumerable<ApplicationUser>> GetCourseMembers(int courseId);
    }
}
