using StudentStorage.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using StudentStorage.Models;
using System.Security.Claims;

namespace StudentStorage.Authorization
{
    public class CourseCreatorAuthorizationHandler : AuthorizationHandler<CourseCreatorAuthorizationRequirement, Course>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CourseCreatorAuthorizationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
            CourseCreatorAuthorizationRequirement requirement, 
            Course resource)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null && (userId == resource.CreatorId))
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
