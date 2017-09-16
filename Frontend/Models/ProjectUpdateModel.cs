using System.Collections.Generic;

namespace Frontend.Models
{
    public class ProjectUpdateModel
    {
        public string PrjImage { get; set; }

        public string ProjectName { get; set; }

        public string ProjectDescription { get; set; }

        public IList<string> Tags { get; set; }
    }
}