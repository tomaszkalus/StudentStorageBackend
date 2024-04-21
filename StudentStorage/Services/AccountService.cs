using Microsoft.AspNetCore.Identity;
using StudentStorage.Models.Authentication;
using StudentStorage.Models;

namespace StudentStorage.Services
{
    public class AccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        private async Task<IdentityResult> Register(RegisterModel model, string role)
        {
            var userExists = await _userManager.FindByNameAsync(model.Email);
            if (userExists != null)
                return IdentityResult.Failed(new IdentityError { Description = "User already exists!" });

            ApplicationUser user = new()
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreatedAt = DateTime.Now,
                SecurityStamp = Guid.NewGuid().ToString(),

            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return result;

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);

            return IdentityResult.Success;
        }

        public Task<IdentityResult> RegisterStudent(RegisterModel model)
        {
            return Register(model, UserRoles.Student);
        }

        public Task<IdentityResult> RegisterTeacher(RegisterModel model)
        {
            return Register(model, UserRoles.Teacher);
        }

        public Task<IdentityResult> RegisterAdmin(RegisterModel model)
        {
            return Register(model, UserRoles.Admin);
        }
    }

}
