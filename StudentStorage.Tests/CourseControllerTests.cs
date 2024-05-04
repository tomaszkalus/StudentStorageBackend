using AutoMapper;
using BookStoreMVC.DataAccess.Repository.IRepository;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentStorage.Controllers;
using StudentStorage.Models;
using StudentStorage.Models.DTO;
using System.Security.Claims;

namespace StudentStorage.Tests
{
    public class CourseControllerTests
    {
        private readonly CoursesController _controller;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public CourseControllerTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _userManager = A.Fake<UserManager<ApplicationUser>>();
            _mapper = A.Fake<IMapper>();
            _controller = new CoursesController(_unitOfWork, _userManager, _mapper);
        }

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
        public async Task Get_ReturnsBadRequestResponse_WhenCourseIsNull()
        {
            // Arrange
            int fakeId = 3;
            A.CallTo(() => _unitOfWork.Course.GetByIdAsync(fakeId)).Returns(Task.FromResult<Course>(null));

            // Act
            var result = await _controller.Get(fakeId);

            // Assert
            var badResponseResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResults_WithCourseResponseDtos()
        {
            // Arrange
            var fakeCoursesNumber = 5;
            var fakeCourses = new List<Course>(new Course[fakeCoursesNumber]);
            A.CallTo(() => _unitOfWork.Course.GetAllAsync("Creator")).Returns(fakeCourses);

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

        [Fact]
        public async Task Post_ReturnsOkResult_WithCourseResponseDTO()
        {
            // Arrange
            var fakeCourseRequestDTO = new CourseRequestDTO();
            var fakeUserId = "test-user-id";
            var fakeUserName = "test-user-name";
            var fakeClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity([], "TestAuthentication"));

            var fakeContext = new DefaultHttpContext() { User = fakeClaimsPrincipal };

            var fakeApplicationUser = new ApplicationUser { Id = fakeUserId, UserName = fakeUserName };
            A.CallTo(() => _userManager.GetUserAsync(fakeClaimsPrincipal)).Returns(fakeApplicationUser);
            _controller.ControllerContext.HttpContext = fakeContext;

            // Act
            var result = await _controller.Post(fakeCourseRequestDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<CourseResponseDTO>(okResult.Value);
        }
    }
}