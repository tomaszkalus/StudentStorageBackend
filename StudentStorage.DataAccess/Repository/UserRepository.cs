using StudentStorage.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using StudentStorage.DataAccess.Data;
using StudentStorage.DataAccess.Repository.IRepository;
using StudentStorage.Models;

namespace StudentStorage.DataAccess.Repository
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        private readonly ApplicationDbContext _db;
        public UserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            var objFromDb = await _db.Users.FindAsync(user.Id);
            if (objFromDb != null)
            {
                objFromDb.FirstName = user.FirstName;
                objFromDb.LastName = user.LastName;
                objFromDb.Email = user.Email;
                objFromDb.PhoneNumber = user.PhoneNumber;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Course>> GetUserCreatedCoursesAsync(int userId)
        {
            return await _db.Courses
                .Include(c => c.Creator)
                .Where(c => c.CreatorId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesAsync(int userId)
        {
            return await _db.Courses
                .Include(c => c.Students)
                .Where(c => c.Students.Any(s => s.Id == userId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Request>> GetRequestsAsync(int userId)
        {
            return await _db.Requests
                .Include(r => r.Course)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }
        public async Task<bool> IsCourseMemberAsync(int userId, int courseId)
        {
            return await _db.Courses
                .AnyAsync(c => c.Id == courseId && c.Students.Any(s => s.Id == userId));
        }

        public async Task<bool> IsCourseAuthorAsync(int userId, int courseId)
        {
            return await _db.Courses
                .AnyAsync(c => c.Id == courseId && c.CreatorId == userId);
        }

        public async Task<IEnumerable<ApplicationUser>> GetCourseMembers(int courseId)
        {
            return await _db.Courses
                .Include(c => c.Students)
                .Where(c => c.Id == courseId)
                .SelectMany(c => c.Students)
                .ToListAsync();
        }
    }
}
