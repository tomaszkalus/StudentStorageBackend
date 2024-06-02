using StudentStorage.DataAccess.Repository.IRepository;
using FakeItEasy;
using Microsoft.AspNetCore.Authorization;
using StudentStorage.Authorization;
using StudentStorage.Models;
using System.Security.Claims;

namespace StudentStorage.Tests;

public class CourseMembershipAuthorizationHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly CourseMembershipAuthorizationHandler _handler;
    private readonly ClaimsPrincipal _user;
    private readonly Course _course;

    public CourseMembershipAuthorizationHandlerTests()
    {
        _unitOfWork = A.Fake<IUnitOfWork>();
        _handler = new CourseMembershipAuthorizationHandler(_unitOfWork);

        _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "25"),
        }));
        _course = new Course { Id = 5 };
    }

    [Fact]
    public async Task HandleRequirementAsync_Succeeds_WhenUserIsCourseMember()
    {
        // Arrange
        A.CallTo(() => _unitOfWork.User.IsCourseMemberAsync(Int32.Parse(_user.FindFirstValue(ClaimTypes.NameIdentifier)), _course.Id))
            .Returns(true);

        var authContext = new AuthorizationHandlerContext(new[] { new CourseMembershipAuthorizationRequirement() }, _user, _course);

        // Act
        await _handler.HandleAsync(authContext);

        // Assert
        Assert.True(authContext.HasSucceeded);
    }

    [Fact]
    public async Task HandleRequirementAsync_Fails_WhenUserIsNotCourseMember()
    {
        // Arrange
        A.CallTo(() => _unitOfWork.User.IsCourseMemberAsync(Int32.Parse(_user.FindFirstValue(ClaimTypes.NameIdentifier)), _course.Id))
            .Returns(false);

        var authContext = new AuthorizationHandlerContext(new[] { new CourseMembershipAuthorizationRequirement() }, _user, _course);

        // Act
        await _handler.HandleAsync(authContext);

        // Assert
        Assert.False(authContext.HasSucceeded);
    }

    [Fact]
    public async Task HandleRequirementAsync_Succeeds_WhenUserIsCourseAuthor()
    {
        // Arrange
        ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "25"),
        }));

        Course course = new Course { Id = 5, CreatorId = 25 };
        
        A.CallTo(() => _unitOfWork.User.IsCourseMemberAsync(Int32.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)), course.Id))
            .Returns(false);

        var authContext = new AuthorizationHandlerContext(new[] { new CourseMembershipAuthorizationRequirement() }, user, course);

        // Act
        await _handler.HandleAsync(authContext);

        // Assert
        Assert.True(authContext.HasSucceeded);

    }
}
