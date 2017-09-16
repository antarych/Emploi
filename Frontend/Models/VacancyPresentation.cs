using System.Collections.Generic;
using System.Linq;
using ProjectManagement.Domain;
using UserManagement.Domain;

namespace Frontend.Models
{
    public class VacancyPresentation
    {
        public VacancyPresentation(Vacancy vacancy, IList<Account> members)
        {
            id = vacancy.VacancyId;

            profession = vacancy.Name;

            description = vacancy.Description;

            var vacTags = new List<string>();
            foreach (var tag in vacancy.VacancyTags)
            {
                vacTags.Add(tag.TagName);
            }
            tags = vacTags;

            var account = members.FirstOrDefault(user => user.Profile.Portfolio.Contains(vacancy));
            if (account != null)
            {
                member = new MemberPresentation(account);
            }
        }

        public int id { get; set; }

        public MemberPresentation member { get; set; }

        public string profession { get; set; }

        public string description { get; set; }

        public IEnumerable<string> tags { get; set; }
    }
}