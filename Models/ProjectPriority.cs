﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Unbugit.Models
{
    public class ProjectPriority
    {
        public int Id { get; set; }

        [DisplayName("Project Priority")]
        public string Name { get; set; }

    }
}
