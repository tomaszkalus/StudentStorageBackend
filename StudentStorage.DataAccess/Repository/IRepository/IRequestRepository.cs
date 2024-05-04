using BookStoreMVC.DataAccess.Repository.IRepository;
using StudentStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentStorage.DataAccess.Repository.IRepository
{
    public interface IRequestRepository : IRepository<Request>
    {
        Task<Request?> GetByIdAsync(int id);
        Task UpdateAsync(Request request);
    }
}
