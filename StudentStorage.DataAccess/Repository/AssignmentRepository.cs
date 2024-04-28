using BookStoreMVC.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using StudentStorage.DataAccess.Data;
using StudentStorage.DataAccess.Repository.IRepository;
using StudentStorage.Models;
using System.Linq.Expressions;

namespace BookStoreMVC.DataAccess.Repository
{
    public class AssignmentRepository : Repository<Assignment>, IAssignmentRepository
    {
        private readonly ApplicationDbContext _db;
        public AssignmentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Assignment?> GetByIdAsync(int id)
        {
            return await _db.Assignments
                .Include(c => c.Course)
                .Include(c => c.Solutions)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(Assignment assignment)
        {
            var objFromDb = await _db.Assignments.FirstOrDefaultAsync(s => s.Id == assignment.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = assignment.Title;
                objFromDb.Description = assignment.Description;
                await _db.SaveChangesAsync();
            }
        }
    }
}
