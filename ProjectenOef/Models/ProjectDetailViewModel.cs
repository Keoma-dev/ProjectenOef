using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectenOef.Models
{
    public class ProjectDetailViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ProjectStatus { get; set; }
        public List<string> ProjectTags { get; set; } = new List<string>();
    }
}
