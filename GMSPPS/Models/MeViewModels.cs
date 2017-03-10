using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GMSPPS.Models
{
    // Models returned by MeController actions.
    public class GetViewModel
    {
        public string Email { get; set; }
    }
    public class VersionViewModel
    {
        public string Device { get; set; }
        public  int Version { get; set; }
    }
}


