using Microsoft.EntityFrameworkCore;
using StudentStorage.DataAccess.Data;
using StudentStorage.DataAccess.Repository.IRepository;
using StudentStorage.Models;

namespace StudentStorage.DataAccess.Repository
{
    public class SolutionRepository : Repository<Solution>, ISolutionRepository
    {
        private readonly ApplicationDbContext _db;
        public SolutionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Solution?> GetByIdAsync(int id)
        {
            return await _db.Solutions
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task UpdateAsync(Solution solution)
        {
            var objFromDb = await _db.Solutions.FirstOrDefaultAsync(s => s.Id == solution.Id);
            if (objFromDb != null)
            {
                objFromDb.FilePath = solution.FilePath;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Solution>> GetAllUserAssignmentSolutions(int assignmentId, int userId)
        {
            return await _db.Solutions
                .Where(s => s.AssignmentId == assignmentId && s.CreatorId == userId)
                .ToListAsync();
        }

    }
}
