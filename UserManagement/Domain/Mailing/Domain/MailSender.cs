using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using UserManagement.Application;
using UserManagement.Domain.Mailing.Infrastructure;
using UserManagement.Infrastructure;

namespace UserManagement.Domain.Mailing.Domain
{
    public class MailSender
    {
        public MailSender(
            IUserRepository userRepository,
            IMailingRepository mailingRepository,
            IMailer mailer)
        {
            _userRepository = userRepository;
            _mailingRepository = mailingRepository;
            _mailer = mailer;
        }

        public void StartSending()
        {
            Task.Factory.StartNew(() =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    var emailProcessed = TryProcessMail();
                    if (emailProcessed == false)
                    {
                       Task.Delay(TimeSpan.Parse("0.0:10")).Wait(_cancellationTokenSource.Token);
                    }
                }
            },
            _cancellationTokenSource.Token,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Current);
            Trace.WriteLine("Email sending started");
        }

        public void StopSending()
        {
            var cancellationTokenSnapshot = _cancellationTokenSource;
            _cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSnapshot.Cancel();
            cancellationTokenSnapshot.Dispose();
        }

        private bool TryProcessMail()
        {
            var mail = _mailingRepository.PullMail();
            var usersToresend = new List<int>();
            if (mail == null)
            {
                return false;
            }

            try
            {
                _mailingRepository.ExecuteInNHibernateSession(() => SendNotificationMail(mail));
                _mailingRepository.DeleteMail(mail);
            }
            catch (Exception ex)
            {
                var allUsers = mail.UsersId.Split(',');
                usersToresend.AddRange(allUsers.Select(int.Parse));
                Trace.WriteLine($"Failed to send email {mail.Id}, because of {ex.Message}");
                _mailingRepository.SaveMail(new NotificationModel(usersToresend.ToArray(), mail.Subject, mail.Message));
                return false;
            }
            return true;
        }

        private void SendNotificationMail(NotificationModel notificationModel)
        {
            var mail = new MailMessage
            {
                Body = notificationModel.Message
            };
            var client = new SmtpClient()
            {
                EnableSsl = true
            };
            var allUsers = notificationModel.UsersId.Split(',');
            foreach (var emailAdress in allUsers.Select(userId => _userRepository.GetAccount(int.Parse(userId)).Email))
            {
                mail.To.Add(emailAdress);

                client.Send(mail);

                mail.To.Clear();
            }

            mail.Dispose();
        }

        private readonly IUserRepository _userRepository;
        private readonly IMailingRepository _mailingRepository;
        private readonly IMailer _mailer;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    }
}
