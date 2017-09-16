using System;
using System.Net.Mail;
using Journalist;
using UserManagement.Application;
using UserManagement.Domain.Mailing;
using UserManagement.Domain.Mailing.Domain;


namespace UserManagement.Domain
{
    public class Mailer : IMailer
    {
        public void SendConfiramtionEmail(Uri confirmationLink, MailAddress userEmail)
        {
            Require.NotNull(confirmationLink, nameof(confirmationLink));
            Require.NotNull(userEmail, nameof(userEmail));

            var mail = InitMail(userEmail);
            mail.Subject = "Подтверждение регистрации на Emploi"; 
            mail.Body = confirmationLink.ToString();
            var client = new SmtpClient()
            {
                EnableSsl = true
            };
            client.Send(mail);
            client.Dispose();
        }

        public void SendMail(SingleMailModel model)
        {
            Require.NotNull(model, nameof(model));

            var mail = InitMail(model.MailAddress);
            mail.Body = model.Message;
            var client = new SmtpClient()
            {
                EnableSsl = true
            };
            client.Send(mail);
            client.Dispose();
        }

        private MailMessage InitMail(MailAddress emailAddress)
        {
            var mail = new MailMessage();
            mail.To.Add(emailAddress);
            return mail;
        }
    }
}