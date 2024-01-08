﻿    using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.Extensions.Options;
    using MimeKit;
    using MimeKit.Text;
    using NETCore.MailKit.Infrastructure.Internal;

    namespace GroupSpace2022.Services
    {
        public class MailKitEmailSender : IEmailSender
        {
            public MailKitEmailSender(IOptions<MailKitOptions> options)
            {
                this.Options = options.Value;
            }

            public MailKitOptions Options { get; set; }

            public Task SendEmailAsync(string email, string subject, string message)
            {
                return Execute(email, subject, message);
            }

            public Task Execute(string to, string subject, string message)
            {
                // create message
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(Options.SenderEmail);
                if (!string.IsNullOrEmpty(Options.SenderName))
                    email.Sender.Name = Options.SenderName;
                email.From.Add(email.Sender);
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html) { Text = message };

                // send email
                using (var smtp = new SmtpClient())
                {
                    SecureSocketOptions Security = SecureSocketOptions.None;
                    if (Options.Security)
                    {
                        if (Options.Port == 587)
                            Security = SecureSocketOptions.StartTls;
                        else
                            Security = SecureSocketOptions.SslOnConnect;
                    }
                    smtp.Connect(Options.Server, Options.Port, Security);
                    smtp.Authenticate(Options.Account, Options.Password);
                    smtp.Send(email);
                    smtp.Disconnect(true);
                }
                return Task.FromResult(true);
            }
        }
    }

