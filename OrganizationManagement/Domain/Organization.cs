using System.Collections.Generic;
using Common;

namespace OrganizationManagement.Domain
{
    public class Organization
    {
        public Organization(int leader, string name, string description, ISet<Tag> tags )
        {
            Leader = leader;
            OrganizationName = name;
            OrganizationDescription = description;
            OrganizationTags = tags;
        }

        protected Organization()
        {
            
        }

        public virtual int OrganizationId { get; set; }

        public virtual int Leader { get;  set; }

        public virtual string OrganizationName { get; set; }

        public virtual string OrganizationDescription { get; set; }

        public virtual ISet<Tag> OrganizationTags { get; set; }

    }
}
