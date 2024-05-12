using System.Security.Claims;

namespace StudentStorage.Tests
{
    public class ClaimsPrincipalWrapper
    {
        public virtual string FindFirstValue(ClaimsPrincipal principal, string claimType)
        {
            return principal.FindFirstValue(claimType);
        }
    }
}
