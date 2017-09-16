using System;
using System.Net.Mail;

namespace UserManagement.Domain.Mailing
{
    public class NotificationModel
    {
        public NotificationModel(int[] usersId, string subject, string message)
        {
            UsersId = String.Join(",", usersId);

            Subject = subject;

            Message = message;
        }

        protected NotificationModel()
        {
            
        }

        public virtual int Id { get; set; }

        public virtual string UsersId { get; set; }

        public virtual string Subject { get; set; }

        public virtual string Message { get; set; }
    }
}
