using System.Collections.Generic;
using Common;
using UserManagement.Domain;

namespace Frontend.Models
{
    public class MemberPresentation
    {
        public MemberPresentation(Account userAccount)
        {
            id = userAccount.UserId;
            name = userAccount.Profile.Name;
            surname = userAccount.Profile.Surname;
            avatar = userAccount.Profile.Avatar;
        }

        public virtual int id { get; set; }

        public virtual string name { get; set; }

        public virtual string surname { get; set; }

        public virtual string avatar { get; set; }
    }
}