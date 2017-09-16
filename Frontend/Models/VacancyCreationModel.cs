using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Frontend.Models
{
    public class VacancyCreationModel
    {
        [Required]
        public string profession { get; set; }

        [Required]
        public string description { get; set; }

        public IList<string> tags { get; set; }
    }
}