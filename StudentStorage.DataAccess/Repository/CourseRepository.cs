using BookStoreMVC.DataAccess.Repository.IRepository;
using StudentStorage.DataAccess.Data;
using StudentStorage.Models;

namespace BookStoreMVC.DataAccess.Repository
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        private readonly ApplicationDbContext _db;
        public CourseRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public Course? GetById(int id)
        {
            return _db.Course.FirstOrDefault(p => p.Id == id);
        }

        public void Update(Course course)
        {
            var objFromDb = _db.Course.FirstOrDefault(s => s.Id == course.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = course.Name;
                objFromDb.Description = course.Description;
            }
        }
    }
}
