using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.NHibernate;
using Journalist;
using NHibernate.Linq;
using UserManagement.Domain;
using UserManagement.Infrastructure;

namespace DataAccess.Repositories
{
    public class ConfirmationRepository : IConfirmationRepository
    {
        public ConfirmationRepository(ISessionProvider sessionProvider)
        {
            Require.NotNull(sessionProvider, nameof(sessionProvider));
            _sessionProvider = sessionProvider;
        }

        private readonly ISessionProvider _sessionProvider;

        public void SaveConfirmationRequest(ConfirmationRequest request)
        {
            Require.NotNull(request, nameof(request));

            var session = _sessionProvider.GetCurrentSession();
            session.Save(request);
        }

        public ConfirmationRequest GetConfirmationRequest(string token)
        {
            Require.NotEmpty(token, nameof(token));

            var session = _sessionProvider.GetCurrentSession();
            var request = session.Get<ConfirmationRequest>(token);
            return request;
        }

        public List<ConfirmationRequest> GetAllConfirmationRequests(Func<ConfirmationRequest, bool> predicate = null)
        {
            var session = _sessionProvider.GetCurrentSession();
            return predicate == null
                ? session.Query<ConfirmationRequest>().ToList()
                : session.Query<ConfirmationRequest>().Where(predicate).ToList();
        }

        public void DeleteConfirmationToken(ConfirmationRequest request)
        {
            Require.NotNull(request, nameof(request));

            var session = _sessionProvider.GetCurrentSession();
            session.Delete(request);
        }
    }
}
