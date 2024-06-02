using FakeItEasy;
using StudentStorage.Models;
using StudentStorage.Services;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace StudentStorage.Tests.Services
{
    public class FileManagerServiceTest
    {

        //[Fact]
        //public void CreateCourseDictionary_ShouldCreateDirectory()
        //{
        //    // Arrange
        //    var courseName = "Algebra i logika";
        //    var courseCreator = new ApplicationUser { Id = 532, UserName = "test", FirstName = "Janusz", LastName = "Kowalczyk" };
        //    var course = new Course { Id = 9, Name = courseName, CreatedAt = DateTime.Now, Creator = courseCreator};

        //    var path = "C:\\Users\\tomas\\source\\repos\\StudentStorage\\UserFiles";
        //    var config = A.Fake<IConfiguration>();
        //    A.CallTo(() => config["CourseFiles:BasePath"]).Returns(path);


        //    var fileService = new FileManagerService(config);
        //    var courseDirName = fileService.GenerateCourseDirectoryName(course);
        //    var expectedPath = Path.Combine(path, courseDirName);

        //    var sut = new FileManagerService(config);

        //    // Act
        //    sut.CreateCourseDirectory(course);

        //    // Assert
        //    Assert.True(Directory.Exists(expectedPath));

        //    // Clean up
        //    Directory.Delete(expectedPath);
        //}
        //[Fact]
        //public void CreateStudentDirectory_ShouldCreateDirectory()
        //{
        //    // Arrange
        //    var courseName = "Współczesne Apki Webowe";
        //    var courseCreator = new ApplicationUser { Id = 5, UserName = "test", FirstName = "Andrzej", LastName = "Nowak" };
        //    var courseMember = new ApplicationUser { Id = 21, UserName = "test", FirstName = "Jan", LastName = "Kowalski" };
        //    var course = new Course { Id = 15, Name = courseName, CreatedAt = DateTime.Now, Creator = courseCreator };
        //    course.Students.Add(courseMember);

        //    var userFilesPath = "C:\\Users\\tomas\\source\\repos\\StudentStorage\\UserFiles";
        //    var fakeConfig = A.Fake<IConfiguration>();
        //    A.CallTo(() => fakeConfig["CourseFiles:BasePath"]).Returns(userFilesPath);

        //    var fileService = new FileManagerService(fakeConfig);
        //    var courseDirName = fileService.GenerateCourseDirectoryName(course);
        //    string userDirectoryName = $"{courseMember.LastName}_{courseMember.FirstName}_{courseMember.Id}";
        //    var expectedUserDirPath = Path.Combine(userFilesPath, courseDirName, userDirectoryName);
        //    var expectedCourseDirPath = Path.Combine(userFilesPath, courseDirName);

            
        //    fileService.CreateCourseDirectory(course);

        //    // Act
        //    var sut = new FileManagerService(fakeConfig);
        //    sut.CreateStudentDirectory(course, courseMember);

        //    // Assert
        //    Assert.True(Directory.Exists(expectedUserDirPath));

        //    // Clean up
        //    Directory.Delete(expectedUserDirPath);
        //    Directory.Delete(expectedCourseDirPath);
        //}
    }
}
