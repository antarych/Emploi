using System;
using Common;
using UserManagement.Domain;

namespace UserManagement.Application
{
    public interface IAuthorizer
    {
        TimeSpan TokenLifeTime { get; }

        AuthorizationTokenInfo GetTokenInfo(string authorizationToken);

        AuthorizationTokenInfo Authorize(string mail, Password password);

        bool CheckProfileCompleteness(string userMail);

        Account GetUserByMail(string userMail);

        void LogOut(string authorizationToken);

        string SaveProfileCompletenessConfirmationRequest(int userId);
    }
}
