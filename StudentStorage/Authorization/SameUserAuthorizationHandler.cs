using Microsoft.AspNetCore.Authorization;
using StudentStorage.Models.Enums;
using System.Security.Claims;

namespace StudentStorage.Authorization
{
    public class SameUserAuthorizationHandler : AuthorizationHandler<SameUserAuthorizationRequirement, int>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserAuthorizationRequirement requirement, int userId)
        {
            var currentUser = context.User;
            int currentUserId = Int32.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (currentUserId == userId || currentUser.IsInRole(UserRoles.Admin))
            {
                context.Succeed(requirement);
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}
