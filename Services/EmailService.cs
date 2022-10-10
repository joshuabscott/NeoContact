using Microsoft.AspNetCore.Identity.UI.Services;
using NeoContact.Models;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace NeoContact.Services
{
    //ADD Lesson #40 Email Contact - Building The Mail Service
    public class EmailService : IEmailSender

    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailSender = _mailSettings.Email;
            MimeMessage newEmail = new();
            newEmail.Sender = MailboxAddress.Parse(emailSender);
            foreach(var emailAddress in email.Split(';'))
            {
                newEmail.To.Add(MailboxAddress.Parse(emailAddress));
            }

            newEmail.Subject = subject;

            BodyBuilder emailbody = new ();
            emailbody.HtmlBody = htmlMessage;

            newEmail.Body = emailbody.ToMessageBody();

            //at this point log into smtp client
            using SmtpClient smtpClient = new();
            try
            {
                var host = _mailSettings.Host;
                var port = _mailSettings.Port;
                var password = _mailSettings.Password;

                await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(emailSender,password);
               
                await smtpClient.SendAsync(newEmail);
                await smtpClient.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                throw;
            }
        }
    }
}
