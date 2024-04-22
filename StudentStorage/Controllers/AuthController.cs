using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentStorage.Models.Authentication;
using StudentStorage.Models.Responses;
using StudentStorage.Services;
using System.IdentityModel.Tokens.Jwt;

namespace StudentStorage.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AuthenticateController(
            AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var (token, user) = await _accountService.Login(model);
            if (token != null)
            {
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
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            return await RegisterUser(model, _accountService.RegisterStudent);
        }

        [HttpPost]
        [Route("register-teacher")]
        //[Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> RegisterTeacher([FromBody] RegisterModel model)
        {
            return await RegisterUser(model, _accountService.RegisterTeacher);
        }

        [HttpPost]
        [Route("register-admin")]
        //[Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            return await RegisterUser(model, _accountService.RegisterAdmin);
        }

        private async Task<IActionResult> RegisterUser(RegisterModel model, Func<RegisterModel, Task<IdentityResult>> registerAction)
        {
            IdentityResult result = await registerAction(model);
            if (result.Succeeded)
                return Ok(new ResponseSuccess());
            else
            {
                List<string> descriptions = result.Errors.Select(x => x.Description).ToList();
                return BadRequest(new ResponseFail { Data = descriptions });
            }
        }
    }
}
