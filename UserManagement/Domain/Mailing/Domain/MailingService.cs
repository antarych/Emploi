using System;
using Common;
using Journalist;
using UserManagement.Application;
using UserManagement.Domain.Mailing;
using UserManagement.Domain.Mailing.Infrastructure;
using UserManagement.Infrastructure;

namespace UserManagement.Domain
{
    public class MailingService : IMailingService
    {
        public MailingService(
            IUserRepository userRepository,
            IMailer mailer,
            IConfirmationRepository confirmationRepository,
            IMailingRepository mailingRepository)
        {
            _userRepository = userRepository;
            _mailer = mailer;
            _confirmationRepository = confirmationRepository;
            _mailingRepository = mailingRepository;
        }

        public void SetupEmailConfirmation(int userId)
        {
            Require.Positive(userId, nameof(userId));

            var token = TokenGenerator.GenerateToken();
            var request = new ConfirmationRequest(userId, token, ConfirmationType.MailConfirmation);
            _confirmationRepository.SaveConfirmationRequest(request);

            var confirmationLink = new Uri(new Uri("http://emploi.lod-misis.ru/confirm/"), token);

            _mailingRepository.SaveMail(new NotificationModel(new [] {userId}, "Подтверждение регистрации на Emploi", confirmationLink.ToString()));
        }


        private readonly IConfirmationRepository _confirmationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMailer _mailer;
        private readonly IMailingRepository _mailingRepository;
    }
}
