using BookStoreMVC.DataAccess.Repository.IRepository;
using StudentStorage.DataAccess.Data;
using StudentStorage.DataAccess.Repository.IRepository;

namespace BookStoreMVC.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICourseRepository Course { get; private set; }
        public IAssignmentRepository Assignment { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Course = new CourseRepository(_db);
            Assignment = new AssignmentRepository(_db);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
