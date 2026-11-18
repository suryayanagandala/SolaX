using ApplicationLayer.Interfaces;
using CoreModels.DTO_S;
//using MailKit.Net.Smtp;
//using MailKit.Security;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
//using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbAccessaLayer.Infrastructure
{
    public class EmailServiceRepository : IEmailService
    {
        private readonly EmailSettings emailSettings;

        public EmailServiceRepository(IOptions<EmailSettings> options)
        {
            emailSettings = options.Value;
        }

        public async Task SendEmail(MailRequest mailRequest)
        {
            //if (string.IsNullOrWhiteSpace(mailRequest.Email))
            // throw new Exception("Recipient email is required");

            //var email = new MimeMessage();

            //// FROM
            //email.From.Add(MailboxAddress.Parse(emailSettings.Email));

            //// TO  
            //email.To.Add(MailboxAddress.Parse(mailRequest.Email));

            //// SUBJECT
            //email.Subject = mailRequest.Subject;

            //// BODY
            //var builder = new BodyBuilder
            //{
            //    HtmlBody = mailRequest.EmailBody
            //};
            //email.Body = builder.ToMessageBody();

            //using var smtp = new SmtpClient();

            //await smtp.ConnectAsync(emailSettings.Host,emailSettings.Port,SecureSocketOptions.StartTls);

            //await smtp.AuthenticateAsync(emailSettings.Email,emailSettings.Password);
            //await smtp.SendAsync(email);
            //await smtp.DisconnectAsync(true);
        }
    }

}
