using System.Collections.Generic;
using Common;

namespace ProjectManagement.Domain
{
    public class Project
    {
        public Project(
            int leader,
            string projectImage,
            string projectName,
            string projectDescription,
            bool fromOrganization,
            int organizationId = 0)
        {
            Leader = leader;
            ProjectImage = projectImage;
            ProjectName = projectName;
            ProjectDescription = projectDescription;
            ProjectTags = new HashSet<Tag>();
            Vacancies = new HashSet<Vacancy>();
            FromOrganization = fromOrganization;
            OrganizationId = organizationId;            
        }

        protected Project()
        {
            
        }

        public virtual int ProjectId { get; protected set; }
        public virtual int Leader { get; protected set; }
        public virtual string ProjectImage { get; set; }
        public virtual string ProjectName { get; set; }
        public virtual string ProjectDescription { get; set; }
        public virtual ISet<Tag> ProjectTags { get; set; }
        public virtual IEnumerable<Vacancy> Vacancies { get; set; }
        public virtual bool FromOrganization { get; protected set; }
        public virtual int OrganizationId { get; protected set; }
    }
}
