using BookStoreMVC.DataAccess.Repository.IRepository;
using FakeItEasy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using StudentStorage.Authorization;
using StudentStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StudentStorage.Tests
{
    public class CourseMembershipAuthorizationHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;

        public CourseMembershipAuthorizationHandlerTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _authorizationService = A.Fake<IAuthorizationService>();

        }

        [Fact]
        public async Task HandleRequirementAsync_Succeeds_WhenUserIsCourseMember()
        {
            // Arrange
            var fakeUser = new ApplicationUser();
            var fakeCourse = new Course();
            fakeCourse.Students.Add(fakeUser);
            A.CallTo(() => _unitOfWork.Course.GetByIdAsync(A<int>.Ignored)).Returns(fakeCourse);

            A.CallTo(() => _authorizationService.AuthorizeAsync(A<ClaimsPrincipal>.Ignored, fakeCourse, "CourseMembershipPolicy"))
                .Returns(AuthorizationResult.Success());

            // Act
            var result = await _authorizationService.AuthorizeAsync(new ClaimsPrincipal(), fakeCourse, "CourseMembershipPolicy");

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_Fails_WhenUserIsNotCourseMember()
        {
            // Arrange
            var fakeUser = new ApplicationUser();
            var fakeCourse = new Course();
            A.CallTo(() => _unitOfWork.Course.GetByIdAsync(A<int>.Ignored)).Returns(fakeCourse);

            A.CallTo(() => _authorizationService.AuthorizeAsync(A<ClaimsPrincipal>.Ignored, fakeCourse, "CourseMembershipPolicy"))
                .Returns(AuthorizationResult.Success());

            // Act
            var result = await _authorizationService.AuthorizeAsync(new ClaimsPrincipal(), fakeCourse, "CourseMembershipPolicy");

            // Assert
            Assert.False(result.Succeeded);
        }
    }
}
