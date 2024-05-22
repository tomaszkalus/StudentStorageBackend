using StudentStorage.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using StudentStorage.Models;
using System.Security.Claims;
using System.Reflection;

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
            string? userIdString = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            int userId = Int32.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)); 
            if (await _unitOfWork.User.IsCourseMemberAsync(userId, course.Id) || userId == course.CreatorId)
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
