﻿using StudentStorage.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using StudentStorage.DataAccess.Data;
using StudentStorage.Models;

namespace StudentStorage.DataAccess.Repository
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        private readonly ApplicationDbContext _db;
        public CourseRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _db.Courses
                .Include(c => c.Creator)
                .Include(c => c.Students)
                .Include(c => c.Assignments)
                .Include(c => c.Requests)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(Course course)
        {
            var objFromDb = await _db.Courses.FirstOrDefaultAsync(s => s.Id == course.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = course.Name;
                objFromDb.Description = course.Description;
                await _db.SaveChangesAsync();
            }
        }
    }
}
