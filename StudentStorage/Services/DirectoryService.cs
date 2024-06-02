using StudentStorage.Models;

namespace StudentStorage.Services
{
    public class DirectoryService
    {
        private readonly string _basePath;
        public DirectoryService(IConfiguration configuration)
        {
            try
            {
                _basePath = configuration["UserFiles:AbsolutePath"];
            }
            catch (Exception e)
            {
                throw new Exception("Could not read base path from configuration", e);
            }
        }

        public string? GetDirectoryById(string basePath, int id)
        {
            string[] directories = Directory.GetDirectories(basePath);
            return directories.FirstOrDefault(dir => Path.GetFileName(dir).EndsWith(id.ToString()), null);
        }

        public string? GetCourseDirectory(int courseId)
        {
            return GetDirectoryById(_basePath, courseId);
        }

        public string? GetStudentDirectory(int courseId, int studentId)
        {
            string? courseDirectory = GetCourseDirectory(courseId);

            if (courseDirectory == null)
            {
                return null;
            }
            var coursePath = Path.Combine(_basePath, courseDirectory);

            if (!Directory.Exists(coursePath))
            {
                return null;
            }

            return GetDirectoryById(coursePath, studentId);
        }

        public string? GetAssignmentDirectory(int courseId, int studentId, Assignment assignment)
        {
            string? studentDirectory = GetStudentDirectory(courseId, studentId);

            if (studentDirectory == null)
            {
                return null;
            }

            return GetDirectoryById(studentDirectory, assignment.Id);
        }

        public void DeleteCourseDirectory(Course course)
        {
            string? courseDirectory = GetCourseDirectory(course.Id);
            Directory.Delete(courseDirectory, true);
        }

        public void DeleteStudentDirectory(ApplicationUser user, Course course)
        {
            string? studentDirectory = GetStudentDirectory(course.Id, user.Id);
            Directory.Delete(studentDirectory, true);
        }

        public ServiceResult CreateCourseDirectory(Course course)
        {
            string courseDirectoryName = DirectoryNameBuilderService.GenerateCourseDirectoryName(course);
            string path = Path.Combine(_basePath, courseDirectoryName);

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
                return new ServiceResult(true, "Directory created successfully");
            }
            return new ServiceResult(false, "Directory already exists");
        }

        public void CreateStudentDirectory(Course course, ApplicationUser user)
        {
            string courseDirectory = GetCourseDirectory(course.Id);
            if (courseDirectory != null)
            {
                string directoryName = DirectoryNameBuilderService.GenerateUserDirectoryName(user);
                string studentDirectory = Path.Combine(courseDirectory, directoryName);
                Directory.CreateDirectory(studentDirectory);
            }
        }

        public string CreateStudentAssignmentDirectory(int userId, Assignment assignment)
        {
            string? studentDirectory = GetStudentDirectory(assignment.CourseId, userId);

            if (studentDirectory == null)
            {
                throw new Exception("Student directory does not exist.");
            }

            string assignmentDirectoryName = DirectoryNameBuilderService.GenerateAssignmentDirectoryName(assignment);
            string assignmentDirectoryPath = Path.Combine(studentDirectory, assignmentDirectoryName);

            Directory.CreateDirectory(assignmentDirectoryPath);
            return assignmentDirectoryPath;
        }
    }
}
