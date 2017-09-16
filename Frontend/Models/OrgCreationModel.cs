using System.Collections.Generic;
using Common;

namespace Frontend.Models
{
    public class OrgCreationModel
    {
        public virtual int Leader { get; set; }

        public virtual string OrganizationName { get; set; }

        public virtual string OrganizationDescription { get; set; }

        public virtual ISet<Tag> OrganizationTags { get; set; }
    }
}