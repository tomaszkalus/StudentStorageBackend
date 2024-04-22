using BookStoreMVC.DataAccess.Repository.IRepository;
using StudentStorage.DataAccess.Data;

namespace BookStoreMVC.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICourseRepository Course { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Course = new CourseRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
