﻿using StudentStorage.Models;

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
                    SizeMb = (int)(file.Length / 1024 / 1024)
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
    }
}
