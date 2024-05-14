using StudentStorage.DataAccess.Repository.IRepository;
using StudentStorage.DataAccess.Data;
using StudentStorage.DataAccess.Repository;
using StudentStorage.DataAccess.Repository.IRepository;
using StudentStorage.Models;

namespace StudentStorage.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICourseRepository Course { get; private set; }
        public IAssignmentRepository Assignment { get; private set; }
        public IRequestRepository Request { get; private set; }
        public IUserRepository User { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Course = new CourseRepository(_db);
            Assignment = new AssignmentRepository(_db);
            Request = new RequestRepository(_db);
            User = new UserRepository(_db);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task ApproveCourseRequestTransactionAsync(Request request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request cannot be null");
            }

            var transaction = _db.Database.BeginTransaction();
            try
            {
                _db.Requests.Update(request);
                _db.Courses.Update(request.Course);
                await transaction.CommitAsync();
                
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("An error occurred while approving the course request", ex);
            }
            
        }
    }
}
