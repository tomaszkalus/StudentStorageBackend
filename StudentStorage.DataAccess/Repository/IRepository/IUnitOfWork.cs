using StudentStorage.DataAccess.Repository.IRepository;
using StudentStorage.Models;

namespace StudentStorage.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICourseRepository Course { get; }
        IAssignmentRepository Assignment { get; }
        IRequestRepository Request { get; }
        IUserRepository User { get; }

        Task ApproveCourseRequestTransactionAsync(Request request);
        Task SaveAsync();
    }
}
