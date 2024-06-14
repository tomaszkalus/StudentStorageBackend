using StudentStorage.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using StudentStorage.Models;
using System.Security.Claims;

namespace StudentStorage.Authorization
{
    /// <summary>
    /// Authorization handler for enforcing that the current user is the creator of the target course.
    /// </summary>
    public class CourseCreatorAuthorizationHandler : AuthorizationHandler<CourseCreatorAuthorizationRequirement, Course>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CourseCreatorAuthorizationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        protected async override Task<Task> HandleRequirementAsync(AuthorizationHandlerContext context, 
            CourseCreatorAuthorizationRequirement requirement, 
            Course resource)
        {
            string? userIdString = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            int userId = Int32.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userId == resource.CreatorId)
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
