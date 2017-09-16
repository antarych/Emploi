using System.Collections.Generic;
using Common;
using Journalist;

namespace OrganizationManagement.Application
{
    public class CreateOrgRequest
    {
        public CreateOrgRequest(int leader, string name, string description, ISet<Tag> tags)
        {
            Require.Positive(leader, nameof(leader));
            Require.NotEmpty(name, nameof(name));
            Require.NotEmpty(description, nameof(description));


            Leader = leader;
            OrganizationName = name;
            OrganizationDescription = description;
            OrganizationTags = tags;
        }

        public virtual int Leader { get; protected set; }

        public virtual string OrganizationName { get; set; }

        public virtual string OrganizationDescription { get; set; }

        public virtual ISet<Tag> OrganizationTags { get; set; }
    }
}
