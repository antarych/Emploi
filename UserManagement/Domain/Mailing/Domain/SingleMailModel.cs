using System.Net.Mail;

namespace UserManagement.Domain.Mailing.Domain
{
    public class SingleMailModel
    {
        public SingleMailModel(MailAddress mailAddress, string subject, string message)
        {
            MailAddress = mailAddress;

            Subject = subject;

            Message = message;
        }

        public MailAddress MailAddress { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }
    }
}
