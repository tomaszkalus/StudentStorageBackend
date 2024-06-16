using Microsoft.AspNetCore.Authorization;

namespace StudentStorage.Authorization
{
    /// <summary>
    /// Base class for authorization requirement handlers. It bypasses the requirement if the user is in the "Admin" role.
    /// This version supports resource-based authorization.
    /// </summary>
    /// <typeparam name="TRequirement">The type of the authorization requirement.</typeparam>
    /// <typeparam name="TResource">The type of the resource. Use 'object' if no specific resource type is needed.</typeparam>
    public abstract class AuthorizationHandlerBase<TRequirement, TResource> : AuthorizationHandler<TRequirement, TResource>
        where TRequirement : IAuthorizationRequirement
    {
        protected sealed override Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement, TResource resource)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            return HandleAsync(context, requirement, resource);
        }

        protected abstract Task HandleAsync(AuthorizationHandlerContext context, TRequirement requirement, TResource resource);
    }

    /// <summary>
    /// Base class for authorization requirement handlers, without resource-based authorization.
    /// </summary>
    /// <typeparam name="TRequirement">The type of the authorization requirement.</typeparam>
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

