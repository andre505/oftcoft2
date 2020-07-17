using MimeKit;
using SendGrid;
using SendGrid.Helpers.Mail;
using OFTENCOFTAPI.Services.SendMail;
using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Org.BouncyCastle.Crypto.Tls;

namespace OFTENCOFTAPI.Services

{
    public interface IEmailService
    {
        Task ExecuteAsync(string email, string subject, string message, string htmlMessage);

        Task SendEmailAsync(
            string FromDisplayName,
            string FromEmailAddress,
            string toName,
            string toEmailAddress,
            string subject,
            string message,
            params SendMail.Attachment[] attachments
        );

        Task SendEmailToSupportAsync(string subject, string message, string toEmail);
    }
    public class EmailSender : IEmailService
    {       
        public async Task ExecuteAsync(string to, string subject, string plaintext, string Html)
        {
            var apiKey = Environment.GetEnvironmentVariable("SendGridAPIKey");
            var client = new SendGridClient(apiKey);
            var emailfrom = new EmailAddress("noreply@nationalgiveaway.com", "National Giveaway");
            var emailsubject = subject;
            var emailto = new EmailAddress(to, "Example User");
            var plainTextContent = plaintext;
            //var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var htmlContent = Html;
            var msg = MailHelper.CreateSingleEmail(emailfrom, emailto, emailsubject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

        public async Task SendEmailAsync(
            string FromDisplayName,
            string FromEmailAddress,
            string toName,
            string toEmailAddress,
            string subject,
            string message,
            params SendMail.Attachment[] attachments)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(FromDisplayName, FromEmailAddress));
            email.To.Add(new MailboxAddress(toName, toEmailAddress));
            email.Subject = subject;

            var body = new BodyBuilder
            {
                HtmlBody = message
            };

            foreach (var attachment in attachments)
            {
                using(var stream = await attachment.ContentToStreamAsync())
                {
                    body.Attachments.Add(attachment.FileName, stream);
                }
            }

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (sender, certificate, certChainType, errors) => true;
                client.AuthenticationMechanisms.Remove("XOAUTH");

                await client.ConnectAsync("smtp.gmail.com", 587, false).ConfigureAwait(false);
                await client.AuthenticateAsync("alagbaladamilola@gmail.com", "Passworddamiself4123*").ConfigureAwait(false);

                await client.SendAsync(email).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            };

        }

        public async Task SendEmailToSupportAsync(string subject, string message, string toEmail)
        {
            await this.SendEmailAsync("No Reply", "no-reply@nationaluptake.com", "Support", toEmail, subject, message);
        }
    }
}