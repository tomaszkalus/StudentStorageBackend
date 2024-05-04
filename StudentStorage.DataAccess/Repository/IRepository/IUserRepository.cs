using BookStoreMVC.DataAccess.Repository.IRepository;
using StudentStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentStorage.DataAccess.Repository.IRepository
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        Task<IEnumerable<Course>> GetCourses(string userId);
        Task<IEnumerable<Request>> GetRequests(string userId);
    }
}
