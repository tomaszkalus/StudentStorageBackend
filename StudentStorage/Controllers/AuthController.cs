using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudentStorage.Models;
using StudentStorage.Models.Authentication;
using StudentStorage.Models.Responses;
using StudentStorage.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentStorage.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly AccountService _accountService;

        public AuthenticateController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            AccountService accountService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _accountService = accountService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("register-student")]
        public async Task<IActionResult> RegisterStudent([FromBody] RegisterModel model)
        {
            var result = await _accountService.RegisterStudent(model);
            if (result.Succeeded)
                return Ok(new ResponseSuccess { Status = "Success", Message = "User created successfully!" });
            else
            {
                var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                var descriptions = result.Errors.Select(x => x.Description).ToList();
                return BadRequest(new ResponseFail { Status = "Error", Message = $"User creation failed", Data = descriptions });
            }
        }

        [HttpPost]
        [Route("register-teacher")]
        //[Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> RegisterTeacher([FromBody] RegisterModel model)
        {
            var result = await _accountService.RegisterTeacher(model);
            if (result.Succeeded)
                return Ok(new ResponseSuccess { Status = "Success", Message = "User created successfully!" });
            else
                return BadRequest(new ResponseFail { Status = "Error", Message = "User creation failed!" });
        }

        [HttpPost]
        [Route("register-admin")]
        //[Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var result = await _accountService.RegisterAdmin(model);
            if (result.Succeeded)
                return Ok(new ResponseSuccess { Status = "Success", Message = "User created successfully!" });
            else
                return BadRequest(new ResponseFail { Status = "Error", Message = "User creation failed!" });
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
    }
}