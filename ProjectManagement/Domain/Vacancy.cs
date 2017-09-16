using System.Collections.Generic;
using Common;

namespace ProjectManagement.Domain
{
    public class Vacancy 
    {
        public Vacancy(string name)
        {
            Name = name;
            VacancyTags = new HashSet<Tag>();
        }

        protected Vacancy()
        {
            
        }

        public virtual int VacancyId { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual ISet<Tag> VacancyTags { get; set; }

        public virtual int MemberId { get; set; }
    }
}
