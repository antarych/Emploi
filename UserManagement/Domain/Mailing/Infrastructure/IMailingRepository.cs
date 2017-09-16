using System;

namespace UserManagement.Domain.Mailing.Infrastructure
{
    public interface IMailingRepository
    {
        void SaveMail(NotificationModel notificationModel);

        void DeleteMail(NotificationModel notificationModel);

        NotificationModel PullMail();

        void ExecuteInNHibernateSession(Action action);
    }
}
