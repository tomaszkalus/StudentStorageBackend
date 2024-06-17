using System.Net.Mail;
using System.Net;

namespace StudentStorage.Services
{
    public class MailingService
    {
        public void SendMail(string emailBody, string emailTopic, string recipientEmail)
        {
            using (var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("studentstoragepolsl@gmail.com", Environment.GetEnvironmentVariable("gmail_password")),
                EnableSsl = true
            })
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("studentstoragepolsl@gmail.com"),
                    Subject = emailTopic,
                    Body = emailBody,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(recipientEmail);

                client.Send(mailMessage);
            }
        }
    }
}
