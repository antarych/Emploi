using System.Collections.Generic;
using System.Linq;
using Frontend.Models;
using OrganizationManagement.Domain;

namespace UserManagement.Domain
{
    public class UserPresentation
    {
        public UserPresentation(int userId, string email, Profile profile, IList<ProjectPresentation> userPortfolio)
        {
            id = userId;
            name = profile.Name;
            surname = profile.Surname;
            middleName = profile.Middlename;
            mail = email;
            institute = profile.Institute;
            course = profile.Course;
            direction = profile.Direction;
            contacts = profile.Contacts;
            if (profile.Avatar != null)
            {
                avatar = profile.Avatar;
            }
            var userTags = new List<string>();
            foreach (var tag in profile.Tags)
            {
                userTags.Add(tag.TagName);
            }
            tags = userTags;
            portfolio = userPortfolio;

            organizations = profile.Organizations;
        }

        public UserPresentation(Account account, IList<ProjectPresentation> userPortfolio)
        {
            id = account.UserId;
            name = account.Profile.Name;
            surname = account.Profile.Surname;
            middleName = account.Profile.Middlename;
            mail = account.Email.ToString();
            institute = account.Profile.Institute;
            course = account.Profile.Course;
            direction = account.Profile.Direction;
            contacts = account.Profile.Contacts;
            if (account.Profile.Avatar != null)
            {
                avatar = account.Profile.Avatar;
            }
            var userTags = new List<string>();
            foreach (var tag in account.Profile.Tags.ToList())
            {
                userTags.Add(tag.TagName);
            }
            tags = userTags;
            portfolio = userPortfolio;

            organizations = account.Profile.Organizations;
        }

        protected UserPresentation()
        {
            
        }

        public virtual int id { get; set; }

        public virtual string name { get; set; }

        public virtual string surname { get; set; }

        public virtual string middleName { get; set; }

        public virtual string mail { get; set; }

        public virtual string institute { get; set; }

        public virtual int course { get; set; }

        public virtual string direction { get; set; }

        public virtual string aboutMe { get; set; }

        public virtual IEnumerable<Contacts> contacts { get; set; }

        public virtual string avatar { get; set; }

        public virtual IEnumerable<string> tags { get; set; }

        public virtual IList<ProjectPresentation> portfolio { get; set; }

        public virtual IEnumerable<Organization> organizations { get; set; }

    }
}