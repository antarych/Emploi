using System.Collections.Generic;
using Common;
using OrganizationManagement.Domain;
using ProjectManagement.Domain;

namespace UserManagement.Domain
{
    public class Profile
    {
        public virtual string Avatar { get; set; }

        public virtual string Name { get; set; }

        public virtual string Surname { get; set; }

        public virtual string Middlename { get; set; }

        public virtual string AboutUser { get; set; }

        public virtual IEnumerable<Contacts> Contacts { get; set; }

        public virtual string Institute { get; set; }

        public virtual string Direction { get; set; }

        public virtual int Course { get; set; }

        public virtual ISet<Tag> Tags { get; set; }

        public virtual IEnumerable<Organization> Organizations { get; set; }

        public virtual IEnumerable<Vacancy> Portfolio { get; set; }
    }
}
