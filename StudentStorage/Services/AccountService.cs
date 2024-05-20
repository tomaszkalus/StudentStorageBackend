using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using StudentStorage.Models;
using StudentStorage.Models.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentStorage.Services
{
    public class AccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<(JwtSecurityToken? token, ApplicationUser? user)> Login(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));
                var token = GetToken(authClaims);
                return (token, user);
            }
            return (null, null);
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
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

            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

            await AssignRole(user, role);

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

        public async Task AssignRole(ApplicationUser user, string userRole)
        {
            List<string> roles = new();

            switch (userRole)
            {
                case UserRoles.Admin:
                    roles.Add(UserRoles.Admin);
                    roles.Add(UserRoles.Teacher);
                    roles.Add(UserRoles.Student);
                    break;
                case UserRoles.Teacher:
                    roles.Add(UserRoles.Teacher);
                    roles.Add(UserRoles.Student);
                    break;
                case UserRoles.Student:
                    roles.Add(UserRoles.Student);
                    break;
            }

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole<int>(role));
                await _userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
