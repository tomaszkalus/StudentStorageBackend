using BookStoreMVC.DataAccess.Repository;
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
    public class RequestRepository : Repository<Request>, IRequestRepository
    {
        private readonly ApplicationDbContext _db;
        public RequestRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Request?> GetByIdAsync(int id)
        {
            return await _db.Requests.FindAsync(id);
        }

        public async Task UpdateAsync(Request request)
        {
            Request? requestFromDb = await _db.Requests.FindAsync(request.Id);
            if (requestFromDb != null)
            {
                requestFromDb.Status = request.Status;
                requestFromDb.UpdatedAt = DateTime.Now;
                await _db.SaveChangesAsync();
            }
        }
    }
}