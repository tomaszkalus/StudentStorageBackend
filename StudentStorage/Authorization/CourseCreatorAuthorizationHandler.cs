using Microsoft.AspNetCore.Authorization;
using StudentStorage.Models;
using System.Security.Claims;

namespace StudentStorage.Authorization
{
    /// <summary>
    /// Authorization handler for enforcing that the current user is the creator of the target course.
    /// </summary>
    public class CourseCreatorAuthorizationHandler : AuthorizationHandlerBase<CourseCreatorAuthorizationRequirement, Course>
    {
        protected override Task HandleAsync(AuthorizationHandlerContext context, CourseCreatorAuthorizationRequirement requirement, Course resource)
        {
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
