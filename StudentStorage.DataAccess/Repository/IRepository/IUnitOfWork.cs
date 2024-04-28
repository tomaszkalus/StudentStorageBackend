using StudentStorage.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreMVC.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICourseRepository Course { get; }
        IAssignmentRepository Assignment { get; }

        Task SaveAsync();
    }
}
