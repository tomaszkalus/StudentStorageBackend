using StudentStorage.Models;

namespace StudentStorage.Services
{
    public class FileManagerService
    {
        private readonly string _basePath;
        private readonly DirectoryService _directoryService;
        public FileManagerService(IConfiguration configuration, DirectoryService directoryService)
        {
            try
            {
                _basePath = configuration["UserFiles:AbsolutePath"];
            }
            catch (Exception e)
            {
                throw new Exception("Could not read base path from configuration", e);
            }
            _directoryService = directoryService;
        }

        private byte[]? GetUserSolutionFile(Solution solution)
        {
            var path = Path.Combine(_basePath, solution.FilePath);
            if (!File.Exists(path))
            {
                return null;
            }
            return File.ReadAllBytes(path);
        }

        public string? AddAssignmentSolutionFiles(IFormFile file, Assignment assignment, ApplicationUser user)
        {
            string? assignmentDirectory = _directoryService.GetAssignmentDirectory(assignment.CourseId, user.Id, assignment);
            if (assignmentDirectory == null || !Directory.Exists(assignmentDirectory))
            {
                try
                {
                    assignmentDirectory = _directoryService.CreateStudentAssignmentDirectory(user.Id, assignment);
                }
                catch (Exception e)
                {
                    throw new Exception("Could not create assignment directory", e);
                }
            }

            var filePath = Path.Combine(assignmentDirectory, file.FileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return filePath;
        }
    }
}
