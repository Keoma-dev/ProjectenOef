﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectenOef.Domain
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ProjectTag> ProjectTags { get; set; }
    }
}
