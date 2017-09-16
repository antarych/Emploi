using System;
using System.Linq;
using System.Net.Mail;
using DataAccess.NHibernate;
using Journalist;
using NHibernate;
using NHibernate.Linq;
using UserManagement.Domain.Mailing;
using UserManagement.Domain.Mailing.Infrastructure;

namespace DataAccess.Repositories
{
    public class MailingRepository : IMailingRepository
    {
        public MailingRepository(SessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        private readonly SessionProvider _sessionProvider;

        public void SaveMail(NotificationModel notificationModel)
        {
            Require.NotNull(notificationModel, nameof(notificationModel));

            _sessionProvider.OpenSession();
            Session.Save(notificationModel);
            _sessionProvider.CloseSession();
        }

        public NotificationModel PullMail()
        {
            _sessionProvider.OpenSession();
            var email = Session.Query<NotificationModel>().FirstOrDefault();
            _sessionProvider.CloseSession();
            return email;
        }

        public void DeleteMail(NotificationModel notificationModel)
        {
            Require.NotNull(notificationModel, nameof(notificationModel));

            _sessionProvider.OpenSession();
            Session.Delete(notificationModel);
            _sessionProvider.CloseSession();
        }

        public void ExecuteInNHibernateSession(Action action)
        {
            _sessionProvider.ProcessInNHibernateSession(action);
        }

        private ISession Session => _sessionProvider.GetCurrentSession();
    }
}
