using BookStoreMVC.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using StudentStorage.DataAccess.Data;
using StudentStorage.DataAccess.Repository.IRepository;
using StudentStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<Course>> GetCourses(string userId)
        {
            return await _db.Courses
                .Include(c => c.Creator)
                .Where(c => c.CreatorId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Request>> GetRequests(string userId)
        {
            return await _db.Requests
                .Include(r => r.Course)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }
    }
}
