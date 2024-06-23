using StudentStorage.Models;
using System.IO.Compression;

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
        private string? SaveFile(IFormFile file, string directory)
        {
            var fileName = DirectoryNameBuilderService.RemoveInvalidFilenameCharacters(file.FileName);
            var filePath = Path.Combine(directory, fileName);

            if (file.Length == 0)
            {
                throw new Exception("File is empty");
            }
            if (File.Exists(filePath))
            {
                throw new Exception("File already exists");
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {    
                file.CopyTo(fileStream);
            }
            return filePath;
        }

        private string GetOrCreateAssignmentDirectory(Assignment assignment, ApplicationUser user)
        {
            string? assignmentDirectory = _directoryService.GetAssignmentDirectory(assignment.CourseId, user.Id, assignment);
            if (assignmentDirectory == null || !Directory.Exists(assignmentDirectory))
            {
                assignmentDirectory = _directoryService.CreateStudentAssignmentDirectory(user.Id, assignment);
            }
            return assignmentDirectory;
        }

        public IEnumerable<Solution> SaveSolutionFiles(IEnumerable<IFormFile> files, Assignment assignment, ApplicationUser user)
        {
            string assignmentDirectory;
            try
            {
                assignmentDirectory = GetOrCreateAssignmentDirectory(assignment, user);
            }
            catch (Exception)
            {
                throw new Exception("Could not create assignment directory");
            }

            var solutions = new List<Solution>();
            foreach (var file in files)
            {
                string filePath;
                try
                {
                    filePath = SaveFile(file, assignmentDirectory);
                }
                catch (Exception)
                {
                    continue;
                }

                solutions.Add(new Solution
                {
                    AssignmentId = assignment.Id,
                    CreatorId = user.Id,
                    FilePath = Path.GetRelativePath(_basePath, filePath),
                    CreatedAt = DateTime.Now,
                    SizeBytes = (int)(file.Length)
                });
            }
            
            return solutions;
        }

        public void DeleteSolutionFile(Solution solution)
        {
            var path = Path.Combine(_basePath, solution.FilePath);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public IFormFile? GetSolutionFile(Solution solution)
        {
            var path = Path.Combine(_basePath, solution.FilePath);
            if (!File.Exists(path))
            {
                return null;
            }
            var length = new FileInfo(path).Length;
            var fileName = Path.GetFileName(path);
            return new FormFile(new MemoryStream(File.ReadAllBytes(path)), 0, length, fileName, fileName);
        }

        public IFormFile? CreateArchive(IEnumerable<IFormFile> files, string fileName)
        {
            var archivePath = Path.GetTempFileName();

            using (var archiveStream = new FileStream(archivePath, FileMode.Create))
            using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create))
            {
                foreach (var file in files)
                {
                    if (file == null)
                    {
                        continue;
                    }

                    var entry = archive.CreateEntry(file.FileName);
                    using (var entryStream = entry.Open())
                    {
                        file.CopyTo(entryStream);
                    }
                }
            }
            var fileInfo = new FileInfo(archivePath);
            var fileStream = new FileStream(archivePath, FileMode.Open);
            FormFile formFile = new FormFile(fileStream, 0, fileInfo.Length, fileName, fileName);
            
            if (File.Exists(archivePath))
            {
                File.Delete(archivePath);
            }
            return formFile;
        }
    }
}
