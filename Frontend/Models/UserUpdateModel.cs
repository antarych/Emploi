using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UserManagement.Domain;

namespace Frontend.Models
{
    public class UserUpdateModel
    {
        public string Avatar { get; set; }

        [MaxLength(60)]
        public string Name { get; set; }

        [MaxLength(60)]
        public string Surname { get; set; }

        [MaxLength(60)]
        public string Middlename { get; set; }

        public Institutes Institute { get; set; }

        public int Course { get; set; }

        public string AboutUser { get; set; }

        public IEnumerable<Contacts> Contacts { get; set; }

        [MinLength(6)]
        public string Direction { get; set; }

        public IList<string> Tags { get; set; }
    }
}