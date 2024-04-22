using StudentStorage.Models;

namespace BookStoreMVC.DataAccess.Repository.IRepository
{
    public interface ICourseRepository : IRepository<Course>
    {
        void Update(Course course);
        public Course? GetById(int id);
    }
}
