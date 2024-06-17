using StudentStorage.DataAccess.Repository.IRepository;
using StudentStorage.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace StudentStorage.Services
{
    public class InvitationTokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AccountService _accountService;
        private readonly MailingService _mailingService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InvitationTokenService(IUnitOfWork unitOfWork, AccountService accountService, MailingService mailingService, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _accountService = accountService;
            _mailingService = mailingService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GenerateInvitationToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }

        private string GenerateInvitationMessage(string token)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            var path = $"{baseUrl}/api/v1/authenticate/register?token={token}";

            return $"<html><body>You have been invited to join our platform." +
                $"Please click the following link to create an account: <br>" +
                $"<a href=\"{path}\">{path}</a>" +
                $"</html></body>";
        }

        public async Task<ServiceResult> SendInvitationMessage(string email)
        {
            EmailAddressAttribute emailValidator = new EmailAddressAttribute();
            if (!emailValidator.IsValid(email))
            {
                return new ServiceResult(false, "Invalid Email Address");
            }

            string token = GenerateInvitationToken();
            InvitationToken invitationToken = new()
            {
                Token = token,
                Email = email,
                ExpirationDate = DateTime.Now.AddDays(1),
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.InvitationToken.AddAsync(invitationToken);
            await _unitOfWork.CommitAsync();

            string message = GenerateInvitationMessage(token);
            _mailingService.SendMail(message, "StudentStorage - Invitation for creating a teacher account", email);
            return new ServiceResult(true, "");
        }

        public async Task<ServiceResult> ValidateInvitationToken(string email, string token)
        {
            List<InvitationToken> tokens = await _unitOfWork.InvitationToken.GetByEmail(email);
            if (tokens.Count == 0)
            {
                return new ServiceResult(false, "No invitation tokens found for this email");
            }
            InvitationToken? latestToken = tokens.MaxBy(u => u.CreatedAt);
            if (latestToken == null || latestToken.Token != token)
            {
                return new ServiceResult(false, "Invalid token");
            }
            if (latestToken.ExpirationDate < DateTime.Now)
            {
                return new ServiceResult(false, "Token expired");
            }
            return new ServiceResult(true, "");
        }
    }
}
