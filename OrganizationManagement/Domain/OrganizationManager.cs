using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Journalist;
using NHibernate;
using OrganizationManagement.Application;
using OrganizationManagement.Infastructure;
using TagManagement.Application;

namespace OrganizationManagement.Domain
{
    public class OrganizationManager : IOrganizationManager
    {
        private readonly IOrganizationRepository _orgRepository;
        private readonly ITagManager _tagManager;

        public OrganizationManager(IOrganizationRepository orgRepository,
            ITagManager tagManager)
        {
            _orgRepository = orgRepository;
            _tagManager = tagManager;
        }

        public int CreateOrganization(CreateOrgRequest organization)
        {
            Require.NotNull(organization, nameof(organization));

            var newOrganization = new Organization(organization.Leader,
                organization.OrganizationName,
                organization.OrganizationDescription,
                organization.OrganizationTags);
            return _orgRepository.CreateOrganization(newOrganization);
        }

        public Organization GetOrganization(int orgId)
        {
            Require.Positive(orgId, nameof(orgId));

            var org = _orgRepository.GetOrganization(orgId);

            return org;
        }

        public IEnumerable<Organization> GetOrganizations(Func<Organization, bool> predicate = null)
        {
            return _orgRepository.GetAllOrganizations(predicate);
        }

        public Organization UpdateOrg(Organization org)
        {
            return _orgRepository.UpdateOrganization(org);
        }

        public void RemoveOrganization(int orgId)
        {
            _orgRepository.RemoveOrganization(orgId);
        }

        public void AddTagsToOrganization(IList<string> tags, int orgId)
        {
            Require.Positive(orgId, nameof(orgId));

            Require.NotNull(tags, nameof(tags));

            var org = GetOrganization(orgId);
            if (org == null)
            {
                throw new OrgExceptions.OrganizationNotFoundException("Organization not found");
            }
            ISet<Tag> orgTags = new HashSet<Tag>();
            foreach (var tag in tags)
            {
                int tagId;
                if (_tagManager.TryFindTag(tag, out tagId))
                {
                    var tagToAdd = _tagManager.GetTag(t => t.TagId == tagId).Single();
                    orgTags.Add(tagToAdd);
                }
                else
                {
                    var newTag = _tagManager.CreateTag(tag);
                    orgTags.Add(newTag);
                }
            }
            org.OrganizationTags = orgTags;
            _orgRepository.UpdateOrganization(org);
        }
    }
}
