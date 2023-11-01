using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using SneakPeak.Models;

namespace SneakPeak.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly EmailConfiguration _emailConfiguration;    
        public MailService(IOptions<MailSettings> mailSettingsOptions, EmailConfiguration emailConfiguration)
        {
            _mailSettings = mailSettingsOptions.Value;
            _emailConfiguration = emailConfiguration;
        }

        public bool SendEmail(Message message)
        {
            try { 
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
            return true;
            }
            catch (Exception ex) {
            
                return false;
            }
        }


        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("SneakPeak",_emailConfiguration.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = message.Content; // Assuming message.HtmlContent contains your HTML content

            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.Port, false);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfiguration.UserName, _emailConfiguration.Password);

                    client.Send(mailMessage);
                }
                catch
                {
                    //log an error message or throw an exception or both.
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }

        
    }
}
