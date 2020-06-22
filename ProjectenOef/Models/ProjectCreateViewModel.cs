using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectenOef.Models
{
    public class ProjectCreateViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<SelectListItem> ProjectStatuses { get; set; } = new List<SelectListItem>();
        public int SelectedProjectStatus { get; set; }
        public List<SelectListItem> Tags { get; set; } = new List<SelectListItem>();
        public int[] SelectedTags { get; set; }
    }
}
