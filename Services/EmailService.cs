using Microsoft.AspNetCore.Identity.UI.Services;
using NeoContact.Models;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace NeoContact.Services
{
    //ADD #40 Email Contact - Building The Mail Service
    public class EmailService : IEmailSender

    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //MODIFY #57 Email Service Cleanup
            var emailSender = _mailSettings.Email ?? Environment.GetEnvironmentVariable("Email");
            
            MimeMessage newEmail = new();
            newEmail.Sender = MailboxAddress.Parse(emailSender);

            foreach (var emailAddress in email.Split(';'))
            {
                newEmail.To.Add(MailboxAddress.Parse(emailAddress));
            }
            newEmail.Subject = subject;
            BodyBuilder emailbody = new();
            emailbody.HtmlBody = htmlMessage;
            newEmail.Body = emailbody.ToMessageBody();

            //At this point log into smtp client
            using SmtpClient smtpClient = new();

            try
            {
                //MODIFY #57 Email Service Cleanup
                var host = _mailSettings.EmailHost ?? Environment.GetEnvironmentVariable("EmailHost");
                var port = _mailSettings.EmailPort != 0 ? _mailSettings.EmailPort : int.Parse(Environment.GetEnvironmentVariable("EmailPort")!);
                var password = _mailSettings.EmailPassword ?? Environment.GetEnvironmentVariable("EmailPassword");

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