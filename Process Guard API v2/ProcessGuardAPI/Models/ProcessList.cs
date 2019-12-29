﻿using System;
using System.Collections.Generic;

namespace ProcessGuardAPI.Models
{
    public partial class ProcessList
    {
        public string Exe { get; set; }
        public string Filename { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }

        public Colors ColorNavigation { get; set; }
    }
}
