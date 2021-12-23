using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

using Common;
using Services.Email;
using Services.Seeding;

namespace Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly SendGridClient sender;
        private readonly IConfiguration configuration;
        private readonly EmailConfigModel emailConfig;

        private Response response;

        public EmailSenderService(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.emailConfig = this.configuration.GetSection("EmailSender").Get<EmailConfigModel>();

            this.sender = new SendGridClient(emailConfig.Key);
        }

        public async Task<Response> SendEmailAsync(EmailSenderModel model)
        {
            if (string.IsNullOrWhiteSpace(model.FromAddress) && string.IsNullOrWhiteSpace(model.ToAddress))
            {
                throw new ArgumentException(DevValidationMessages.EmptryEmailSenderAddresses);
            }

            var from = new EmailAddress(model.FromAddress, model.FromName);
            var to = new EmailAddress(model.ToAddress);
            var mail = MailHelper.CreateSingleEmail(from, to, model.Subject, null, model.Content);

            try
            {
                this.response = await this.sender.SendEmailAsync(mail);
            }
            catch (Exception err)
            {
                this.response = null;
                Console.WriteLine(err);
            }

            return this.response;
        }

        public async Task<Response> EmailConfirmation(EmailSenderModel model)
        {
            var admin = this.configuration.GetSection("Admin").Get<UserSeederModel>();
            EmailSenderModel mailModel = new EmailSenderModel(admin.Email, model.ToAddress)
            {
                FromName = admin.FirstName + " " + admin.LastName,
                Subject = "Email confirmation message",
                Content = "Please confirm your email by clicking the link below."
            };

            return await this.SendEmailAsync(mailModel);
        }

        public string GetTemplate(string fileName)
        {
            try
            {
                return File.ReadAllText(Paths.HtmlTemplatesDirectory + fileName);
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
