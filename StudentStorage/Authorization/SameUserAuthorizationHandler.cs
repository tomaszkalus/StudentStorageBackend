using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace StudentStorage.Authorization
{
    /// <summary>
    /// Authorization handler for enforcing that the current user is the same as the target user.
    /// </summary>
    public class SameUserAuthorizationHandler : AuthorizationRequirementHandlerBase<SameUserAuthorizationRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SameUserAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleAsync(AuthorizationHandlerContext context, SameUserAuthorizationRequirement requirement)
        {
            var currentUser = context.User;
            int currentUserId = Int32.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
            int targetUserId = Int32.Parse(_httpContextAccessor.HttpContext.Request.RouteValues["id"].ToString());

            if (currentUserId == targetUserId)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}
