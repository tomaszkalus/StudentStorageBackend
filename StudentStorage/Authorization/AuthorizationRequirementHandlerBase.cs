using Microsoft.AspNetCore.Authorization;

namespace StudentStorage.Authorization
{
    /// <summary>
    /// Base class for authorization requirement handlers. It bypasses the requirement if the user is in the "Admin" role.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AuthorizationRequirementHandlerBase<T> : AuthorizationHandler<T> where T : IAuthorizationRequirement
    {
        protected sealed override Task HandleRequirementAsync(AuthorizationHandlerContext context, T requirement)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.FromResult(true);
            }

            return HandleAsync(context, requirement);
        }

        protected abstract Task HandleAsync(AuthorizationHandlerContext context, T requirement);
    }
}
