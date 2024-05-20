using StudentStorage.Models;

namespace StudentStorage.Services
{
    public class CourseFileService
    {
        private readonly string _basePath;
        public CourseFileService(IConfiguration configuration)
        {
            try
            {
                _basePath = configuration["CourseFiles:BasePath"];
            }
            catch (Exception e)
            {
                throw new Exception("Could not read base path from configuration", e);
            }
        }

        public string GenerateCourseDirectoryName(Course course)
        {
            string normalizedCourseName = course.Name.Replace(" ", "_");
            string courseCreatedYear = course.CreatedAt.Year.ToString();
            return $"{course.Creator.FirstName}_{course.Creator.LastName}_{normalizedCourseName}_{courseCreatedYear}_{course.Id}";
        }

        public ServiceResult CreateCourseDirectory(Course course)
        {
            string courseFolderName = GenerateCourseDirectoryName(course);
            string path = Path.Combine(_basePath, courseFolderName);
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception e)
                {
                    return new ServiceResult(false, e.Message);
                }
            }
            return new ServiceResult(false, "Directory already exists");
        }

        private string? GetCourseDirectoryById(int courseId)
        {
            string[] directories = Directory.GetDirectories(_basePath);
            return directories.FirstOrDefault(dir => Path.GetFileName(dir).EndsWith(courseId.ToString()));
        }

        public ServiceResult DeleteCourseDirectory(Course course)
        {
            string courseDirectory = GetCourseDirectoryById(course.Id);
            if (courseDirectory != null)
            {
                try
                {
                    Directory.Delete(courseDirectory, true);
                }
                catch (Exception e)
                {
                    return new ServiceResult(false, e.Message);
                }
            }
            return new ServiceResult(false, "Course directory does not exist.");
        }

        public ServiceResult CreateStudentDirectory(Course course, ApplicationUser user)
        {
            string courseDirectory = GetCourseDirectoryById(course.Id);
            if (courseDirectory != null)
            {
                string directoryName = $"{user.LastName}_{user.FirstName}_{user.Id}";
                string studentDirectory = Path.Combine(courseDirectory, directoryName);
                if (!Directory.Exists(studentDirectory))
                {
                    try
                    {
                        Directory.CreateDirectory(studentDirectory);
                    }
                    catch (Exception e)
                    {
                        return new ServiceResult(false, e.Message);
                    }
                }
                return new ServiceResult(true, "Student directory created successfully.");
            }
            return new ServiceResult(false, "Course directory does not exist.");
        }

    }
}
