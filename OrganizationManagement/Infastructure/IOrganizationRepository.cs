using System;
using System.Collections.Generic;
using OrganizationManagement.Domain;

namespace OrganizationManagement.Infastructure
{
    public interface IOrganizationRepository
    {
        int CreateOrganization(Organization organization);

        Organization GetOrganization(int orgId);

        List<Organization> GetAllOrganizations(Func<Organization, bool> predicate = null);

        Organization UpdateOrganization(Organization organization);

        void RemoveOrganization(int orgId);
    }
}
