using System;
using System.Collections.Generic;
using UserManagement.Domain;

namespace UserManagement.Infrastructure
{
    public interface IConfirmationRepository
    {
        void SaveConfirmationRequest(ConfirmationRequest request);

        ConfirmationRequest GetConfirmationRequest(string token);

        List<ConfirmationRequest> GetAllConfirmationRequests(Func<ConfirmationRequest, bool> predicate = null);

        void DeleteConfirmationToken(ConfirmationRequest request);
    }
}
