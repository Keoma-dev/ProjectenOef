using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectenOef.Domain
{
    public class ProjectAppUser : IdentityUser
    {
        public string Gender { get; set; }
    }
   
}
