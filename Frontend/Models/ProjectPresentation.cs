using System.Collections.Generic;
using System.Linq;
using ProjectManagement.Domain;
using UserManagement.Domain;

namespace Frontend.Models
{
    public class ProjectPresentation
    {
        public ProjectPresentation(Project project, IList<Account> allMembers)
        {
            id = project.ProjectId;

            if (!project.FromOrganization)
            {
                leader = new MemberPresentation(allMembers.First(user => user.UserId == project.Leader));
            }

            name = project.ProjectName;

            description = project.ProjectDescription;

            avatar = project.ProjectImage;

            var prjTags = new List<string>();
            foreach (var tag in project.ProjectTags)
            {
                prjTags.Add(tag.TagName);
            }
            tags = prjTags;

            var listOfVacancies = new List<VacancyPresentation>();
            foreach (var vacancy in project.Vacancies)
            {
                var member = new VacancyPresentation(vacancy, allMembers);
                listOfVacancies.Add(member);
            }
            team = listOfVacancies;
        }

        public int id { get; set; }

        public string avatar { get; set; }

        public MemberPresentation leader { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public IEnumerable<string> tags { get; set; }

        public IList<VacancyPresentation> team { get; set; }
    }
}