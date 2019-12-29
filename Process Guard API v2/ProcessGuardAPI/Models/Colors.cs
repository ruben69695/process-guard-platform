using System;
using System.Collections.Generic;

namespace ProcessGuardAPI.Models
{
    public partial class Colors
    {
        public Colors()
        {
            ProcessList = new HashSet<ProcessList>();
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<ProcessList> ProcessList { get; set; }
    }
}
