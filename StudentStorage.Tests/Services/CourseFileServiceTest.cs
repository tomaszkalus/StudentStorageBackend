using FakeItEasy;
using StudentStorage.Models;
using StudentStorage.Services;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace StudentStorage.Tests.Services
{
    public class CourseFileServiceTest
    {

        [Fact]
        public void CreateCourseDictionary_ShouldCreateDirectory()
        {
            // Arrange
            var courseId = 9;
            var courseName = "Współczesne Apki Webowe";
            var courseCreator = new ApplicationUser { Id = "1", UserName = "test", FirstName = "Andrzej", LastName = "Nowak" };
            var course = new Course { Id = courseId, Name = courseName, CreatedAt = DateTime.Now, Creator = courseCreator};

            var path = "C:\\Users\\tomas\\source\\repos\\StudentStorage\\UserFiles";
            var config = A.Fake<IConfiguration>();
            A.CallTo(() => config["CourseFiles:BasePath"]).Returns(path);


            var fileService = new CourseFileService(config);
            var courseDirName = fileService.GenerateCourseDirectoryName(course);
            var expectedPath = Path.Combine(path, courseDirName);

            var sut = new CourseFileService(config);

            // Act
            sut.CreateCourseDirectory(course);

            // Assert
            Assert.True(Directory.Exists(expectedPath));

            // Clean up
            Directory.Delete(expectedPath);
        }
        [Fact]
        public void CreateStudentDirectory_ShouldCreateDirectory()
        {
            // Arrange
            var courseId = 9;
            var courseName = "Współczesne Apki Webowe";
            var courseCreator = new ApplicationUser { Id = "1", UserName = "test", FirstName = "Andrzej", LastName = "Nowak" };
            var course = new Course { Id = courseId, Name = courseName, CreatedAt = DateTime.Now, Creator = courseCreator };

            var path = "C:\\Users\\tomas\\source\\repos\\StudentStorage\\UserFiles";
            var config = A.Fake<IConfiguration>();
            A.CallTo(() => config["CourseFiles:BasePath"]).Returns(path);


            var fileService = new CourseFileService(config);
            var courseDirName = fileService.GenerateCourseDirectoryName(course);
            var expectedPath = Path.Combine(path, courseDirName, $"{courseCreator.LastName}_{courseCreator.FirstName}_{courseCreator.Id}");

            
            fileService.CreateCourseDirectory(course);

            // Act
            var sut = new CourseFileService(config);
            sut.CreateStudentDirectory(course, courseCreator);

            // Assert
            Assert.True(Directory.Exists(expectedPath));

            // Clean up
            Directory.Delete(expectedPath);
        }
    }
}
