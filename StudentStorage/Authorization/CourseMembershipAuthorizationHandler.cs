using BookStoreMVC.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using StudentStorage.Models;
using System.Security.Claims;

namespace StudentStorage.Authorization
{
    public class CourseMembershipAuthorizationHandler : AuthorizationHandler<CourseMembershipAuthorizationRequirement, Course>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CourseMembershipAuthorizationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        protected async override Task<Task> HandleRequirementAsync(AuthorizationHandlerContext context,
            CourseMembershipAuthorizationRequirement requirement,
            Course course)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null && (await _unitOfWork.User.IsCourseMemberAsync(userId, course.Id) 
                || await _unitOfWork.User.IsCourseAuthorAsync(userId, course.Id)))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            return Task.CompletedTask;

        }
    }
}
