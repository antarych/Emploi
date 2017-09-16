using System.Collections.Generic;

namespace Frontend.Models
{
    public class ProjectCreationModel
    {
        
        public string avatar { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public IList<string> tags { get; set; }

        public bool isFromOrganization { get; set; }

        public int organizationId { get; set; }
    }
}