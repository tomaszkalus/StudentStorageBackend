using AutoMapper;
using StudentStorage.DataAccess.Repository.IRepository;
using FakeItEasy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentStorage.Controllers;
using StudentStorage.Models;
using System.Security.Claims;
using StudentStorage.Models.DTO.Course;

namespace StudentStorage.Tests
{
    public class CourseControllerTests
    {
        private readonly CoursesController _controller;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;
        //private readonly ClaimsPrincipalWrapper _claimsPrincipalWrapper;

        public CourseControllerTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _userManager = A.Fake<UserManager<ApplicationUser>>();
            _mapper = A.Fake<IMapper>();
            _authorizationService = A.Fake<IAuthorizationService>();
            //_controller = new CoursesController(_unitOfWork, _userManager, _mapper, _authorizationService);

            // Create a ClaimsPrincipal with the claims you want
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "test-user-id") };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            // Mock GetUserAsync to return the ClaimsPrincipal
            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>.Ignored)).Returns(Task.FromResult(new ApplicationUser { UserName = "TestUser", Id = 4 }));
        }

        #region GetAll
        [Fact]
        public async Task GetAll_ReturnsOkResults_WithCourseResponseDtos()
        {
            // Arrange
            var fakeCoursesNumber = 5;
            var fakeCourses = new List<Course>(new Course[fakeCoursesNumber]);
            A.CallTo(() => _unitOfWork.Course.GetAllAsync(null, "Creator")).Returns(fakeCourses);

            var fakeCourseResponseDTOs = fakeCourses.Select(c => new CourseResponseDTO()).ToList();
            A.CallTo(() => _mapper.Map<CourseResponseDTO>(A<Course>.Ignored)).Returns(fakeCourseResponseDTOs.First()).Once()
                .Then
                .Returns(fakeCourseResponseDTOs.Last()).Once();

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<CourseResponseDTO>>(okResult.Value);
            Assert.Equal(fakeCourseResponseDTOs.Count, returnValue.Count());
        }
        #endregion

        #region Get
        [Fact]
        public async Task Get_ReturnsOkResult_WithCourseResponseDTO()
        {
            // Arrange
            var fakeCourse = new Course();
            int fakeId = 3;
            A.CallTo(() => _unitOfWork.Course.GetByIdAsync(fakeId)).Returns(fakeCourse);

            var fakeCourseResponseDTO = new CourseResponseDTO();
            A.CallTo(() => _mapper.Map<CourseResponseDTO>(A<Course>.Ignored)).Returns(fakeCourseResponseDTO);

            // Act
            var result = await _controller.Get(fakeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CourseResponseDTO>(okResult.Value);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenCourseDoesNotExist()
        {
            // Arrange
            int fakeId = 3;
            A.CallTo(() => _unitOfWork.Course.GetByIdAsync(fakeId)).Returns(Task.FromResult<Course>(null));


            // Act
            var result = await _controller.Get(fakeId);

            // Assert
            var badResponseResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Get_ReturnsForbid_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var fakeCourse = new Course();
            int fakeId = 3;
            A.CallTo(() => _unitOfWork.Course.GetByIdAsync(fakeId)).Returns(fakeCourse);

            A.CallTo(() => _authorizationService.AuthorizeAsync(A<ClaimsPrincipal>.Ignored, fakeCourse, "CourseMembershipPolicy"))
                .Returns(Task.FromResult(AuthorizationResult.Failed()));

            // Act
            var result = await _controller.Get(fakeId);

            // Assert
            var badResponseResult = Assert.IsType<ForbidResult>(result);
        }
        #endregion

        #region Post
        //[Fact]
        //public async Task PostCourse_ReturnsOkResult_WithCourseResponseDTO()
        //{
        //    // Arrange
        //    var fakeUserId = 3;
        //    ClaimsPrincipal user = A.Fake<ClaimsPrincipal>();
        //    int x = Int32.Parse(_userManager.GetUserId(user));
        //    A.CallTo(() => _userManager.GetUserId(user)).Returns(fakeUserId);

        //    var fakeCourseRequestDTO = new CourseRequestDTO
        //    {
        //        Name = "test-course-name",
        //        Description = "test-course-description"
        //    };

        //    Course fakeCourse = new Course
        //    {
        //        CreatorId = fakeUserId,
        //        Name = fakeCourseRequestDTO.Name,
        //        Description = fakeCourseRequestDTO.Description,
        //        CreatedAt = DateTime.Now
        //    };
        //    CourseResponseDTO fakeCourseResponseDTO = _mapper.Map<CourseResponseDTO>(fakeCourse);

        //    DateTime dateTime = DateTime.Now;

        //    // Act

        //    var result = await _controller.Post(fakeCourseRequestDTO);

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    var returnValueType = Assert.IsAssignableFrom<CourseResponseDTO>(okResult.Value);
        //}
        #endregion
    }
}