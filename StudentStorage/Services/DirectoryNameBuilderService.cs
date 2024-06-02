using StudentStorage.Models;

namespace StudentStorage.Services
{
        /// <summary>
        /// Class that provides methods for building directory names for different entities.
        /// </summary>
        public static class DirectoryNameBuilderService
        {
            public static string RemoveInvalidFilenameCharacters(string filename)
            {
                return string.Join("", filename.Split(Path.GetInvalidFileNameChars()));
            }

            public static string GenerateAssignmentDirectoryName(Assignment assignment)
            {
                string normalizedAssignmentName = assignment.Title.Replace(" ", "_");
                string rawDirName = $"{normalizedAssignmentName}_{assignment.Id}";
                return RemoveInvalidFilenameCharacters(rawDirName);
            }

            public static string GenerateUserDirectoryName(ApplicationUser user)
            {
                string rawDirName = $"{user.LastName}_{user.FirstName}_{user.Id}";
                return RemoveInvalidFilenameCharacters(rawDirName);
            }

            public static string GenerateCourseDirectoryName(Course course)
            {
                string normalizedCourseName = course.Name.Replace(" ", "_");
                string courseCreatedYear = course.CreatedAt.Year.ToString();
                string rawDirName = $"{course.Creator.FirstName}_{course.Creator.LastName}_{normalizedCourseName}_{courseCreatedYear}_{course.Id}";
                return RemoveInvalidFilenameCharacters(rawDirName);
            }
        }
}
