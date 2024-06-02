using Microsoft.EntityFrameworkCore.Storage;
using StudentStorage.DataAccess.Data;
using StudentStorage.DataAccess.Repository.IRepository;

namespace StudentStorage.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public ICourseRepository Course { get; private set; }
        public IAssignmentRepository Assignment { get; private set; }
        public IRequestRepository Request { get; private set; }
        public IUserRepository User { get; private set; }
        public ISolutionRepository Solution { get; private set; }

        private IDbContextTransaction? _transaction;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Course = new CourseRepository(_db);
            Assignment = new AssignmentRepository(_db);
            Request = new RequestRepository(_db);
            User = new UserRepository(_db);
            Solution = new SolutionRepository(_db);
        }

        public async Task CommitAsync()
        {
            try
            {
                await _db.SaveChangesAsync();

                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                if (_transaction != null)
                {
                    await Rollback();
                }

                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _db.Database.BeginTransactionAsync();
        }

        public async Task Rollback()
        {
            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }
    }
}
