using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentStorage.Models;
using StudentStorage.Models.Authentication;
using StudentStorage.Models.DTO.Authentication;
using StudentStorage.Models.Enums;
using StudentStorage.Models.Responses;
using StudentStorage.Services;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;

namespace StudentStorage.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MailingService _mailingService;
        private readonly InvitationTokenService _invitationTokenService;

        public AuthenticateController(
            AccountService accountService, UserManager<ApplicationUser> userManager, MailingService mailingService, InvitationTokenService invitationTokenService)
        {
            _accountService = accountService;
            _userManager = userManager;
            _mailingService = mailingService;
            _invitationTokenService = invitationTokenService;
        }

        /// <summary>
        /// Changes the password of the current user.
        /// </summary>
        /// <param name="changePasswordDTO">DTO for changing the password, containing old password, new password and new password confirmation</param>
        /// <returns code="200">The Response object with the message.</returns>
        /// <returns code="400">The Response object with the information about why the request was not fulfilled.</returns>
        // POST: api/v1/authenticate/change-password
        [HttpPost]
        [Route("change-password")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseSuccess), 200)]
        [ProducesResponseType(typeof(ResponseFail), 400)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if(!await _userManager.CheckPasswordAsync(currentUser, changePasswordDTO.OldPassword))
            {
                return BadRequest(new ResponseFail { Data = new string[] { "Old password is incorrect." } });
            }

            if (changePasswordDTO.NewPassword != changePasswordDTO.ConfirmPassword)
                return BadRequest(new ResponseFail { Data = new string[] { "New password and confirm password do not match." } });

            IdentityResult result = await _accountService.ChangePassword(currentUser, changePasswordDTO.NewPassword);
            if (result.Succeeded)
                return Ok(new ResponseSuccess { Data = "The password has been changed successfully"});
            else
            {
                List<string> descriptions = result.Errors.Select(x => x.Description).ToList();
                return BadRequest(new ResponseFail { Data = descriptions });
            }
        }

        /// <summary>
        /// Logs in the user.
        /// </summary>
        /// <param name="loginDTO">DTO for login, containing login and password</param>
        /// <returns code="200">If the user has been logged correctly</returns>
        /// <returns code="400">If the login was not successful</returns>
        // POST: api/v1/authenticate/login
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var (token, user) = await _accountService.Login(loginDTO);
            if (token != null)
            {
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("register-student")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            return await RegisterUser(model, _accountService.RegisterStudent);
        }

        [HttpPost]
        [Route("register-teacher")]
        public async Task<IActionResult> RegisterTeacher([FromBody] RegisterTeacherDTO model)
        {
            var result = await _invitationTokenService.ValidateInvitationToken(model.Email, model.Token);
            if (!result.Success)
                return BadRequest(new ResponseFail { Data = new string[] { result.Message } });

            RegisterDTO registerDTO = new RegisterDTO
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = model.Password
            };
            return await RegisterUser(registerDTO, _accountService.RegisterTeacher);
        }

        [HttpPost]
        [Route("register-admin")]
        //[Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDTO model)
        {
            return await RegisterUser(model, _accountService.RegisterAdmin);
        }

        private async Task<IActionResult> RegisterUser(RegisterDTO model, Func<RegisterDTO, Task<IdentityResult>> registerAction)
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

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        [Route("invitations")]
        public async Task<IActionResult> SendTeacherInvitation([FromBody] string mail)
        {
            EmailAddressAttribute emailValidator = new EmailAddressAttribute();
            if (!emailValidator.IsValid(mail))
                return BadRequest(new ResponseFail { Data = new string[] { "Invalid email address." } });

            var result = await _invitationTokenService.SendInvitationMessage(mail);

            if (!result.Success)
                return BadRequest(new ResponseFail { Data = new string[] { result.Message } });

            return Ok();
        }
    }
}
