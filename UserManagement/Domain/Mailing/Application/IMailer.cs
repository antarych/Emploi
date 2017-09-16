using System;
using System.Net.Mail;
using UserManagement.Domain.Mailing.Domain;

namespace UserManagement.Application
{
    public interface IMailer
    {
        void SendConfiramtionEmail(Uri confirmationLink, MailAddress userEmail);

        void SendMail(SingleMailModel model);
    }
}
