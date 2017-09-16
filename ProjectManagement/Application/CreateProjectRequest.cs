using System.Collections.Generic;
using Common;
using Journalist;

namespace ProjectManagement.Application
{
    public class CreateProjectRequest
    {
        public CreateProjectRequest(
            int leaderId,
            string prjImage,
            string name,
            string description,
            bool isFromOrganization = false,
            int organizationId = 0)
        {
            Require.Positive(leaderId, nameof(leaderId));
            Require.NotEmpty(name, nameof(name));
            Require.NotEmpty(description, nameof(description));
            Require.NotNull(isFromOrganization, nameof(isFromOrganization));


            LeaderId = leaderId;
            PrjImage = prjImage;
            ProjectName = name;
            ProjectDescription = description;
            IsFromOrganization = isFromOrganization;
            OrganizationId = organizationId;
        }

        public int LeaderId { get; private set; }

        public string PrjImage { get; private set; }

        public string ProjectName { get; private set; }

        public string ProjectDescription { get; private set; }

        public bool IsFromOrganization { get; private set; }

        public int OrganizationId { get; private set; }
    }
}
