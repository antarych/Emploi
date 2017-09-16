using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.NHibernate;
using Journalist;
using NHibernate.Linq;
using OrganizationManagement.Domain;
using OrganizationManagement.Infastructure;

namespace DataAccess.Repositories
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly ISessionProvider _sessionProvider;

        public OrganizationRepository(ISessionProvider sessionProvider)
        {
            Require.NotNull(sessionProvider, nameof(sessionProvider));
            _sessionProvider = sessionProvider;
        }

        public int CreateOrganization(Organization organization)
        {
            Require.NotNull(organization, nameof(organization));

            var session = _sessionProvider.GetCurrentSession();
            var savedOrgId = (int)session.Save(organization);
            return savedOrgId;
        }

        public Organization GetOrganization(int orgId)
        {
            Require.Positive(orgId, nameof(orgId));

            var session = _sessionProvider.GetCurrentSession();
            var org = session.Get<Organization>(orgId);
            return org;
        }

        public List<Organization> GetAllOrganizations(Func<Organization, bool> predicate = null)
        {
            var session = _sessionProvider.GetCurrentSession();
            return predicate == null
                ? session.Query<Organization>().ToList()
                : session.Query<Organization>().Where(predicate).ToList();
        }

        public Organization UpdateOrganization(Organization organization)
        {
            Require.NotNull(organization, nameof(organization));

            var session = _sessionProvider.GetCurrentSession();
            session.Update(organization);
            return organization;
        }

        public void RemoveOrganization(int orgId)
        {
            Require.Positive(orgId, nameof(orgId));

            var session = _sessionProvider.GetCurrentSession();
            var org = GetOrganization(orgId);
            session.Delete(org);
        }
       
    }
}
