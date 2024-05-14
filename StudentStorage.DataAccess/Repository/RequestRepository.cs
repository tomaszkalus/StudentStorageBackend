using Microsoft.EntityFrameworkCore;
using StudentStorage.DataAccess.Data;
using StudentStorage.DataAccess.Repository.IRepository;
using StudentStorage.Models;

namespace StudentStorage.DataAccess.Repository
{
    public class RequestRepository : Repository<Request>, IRequestRepository
    {
        private readonly ApplicationDbContext _db;
        public RequestRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Request?> GetByIdAsync(int id)
        {
            return await _db.Requests
                .Include(r => r.Course)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task UpdateAsync(Request request)
        {
            var objFromDb = await _db.Requests.FirstOrDefaultAsync(s => s.Id == request.Id);
            if (objFromDb != null)
            {
                objFromDb.Status = request.Status;
                objFromDb.UpdatedAt = DateTime.Now;
                await _db.SaveChangesAsync();
            }
        }
    }
}