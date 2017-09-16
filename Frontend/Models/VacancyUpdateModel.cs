using System.Collections.Generic;

namespace Frontend.Models
{
    public class VacancyUpdateModel
    {
        
        public string profession { get; set; }

        
        public string description { get; set; }

        public IList<string> tags { get; set; }
    }
}