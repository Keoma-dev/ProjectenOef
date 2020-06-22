using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ProjectenOef.Domain
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PhotoUrl { get; set; }
        public ProjectStatus ProjectStatus { get; set; }
        public int ProjectStatusId { get; set; }
        public ICollection<ProjectTag> ProjectTags { get; set; }


    }
}
