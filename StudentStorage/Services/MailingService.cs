using System.Net.Mail;
using System.Net;

namespace StudentStorage.Services
{
    public class MailingService
    {
        public void SendMail(string emailBody, string emailTopic, string recipentEmail)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("studentstoragepolsl@gmail.com", Environment.GetEnvironmentVariable("gmail_password")),
                EnableSsl = true
            };
            client.Send("studentstoragepolsl@gmail.com", recipentEmail, emailTopic, emailBody);
        }
    }
}
