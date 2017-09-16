using System;
using System.Collections.Generic;
using OrganizationManagement.Domain;

namespace OrganizationManagement.Application
{
    public interface IOrganizationManager
    {
        int CreateOrganization(CreateOrgRequest organization);

        Organization GetOrganization(int orgId);

        IEnumerable<Organization> GetOrganizations(Func<Organization, bool> predicate = null);

        Organization UpdateOrg(Organization org);

        void RemoveOrganization(int orgId);

        void AddTagsToOrganization(IList<string> tags, int orgId);
    }
}
