﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Unbugit.Models
{
    public class TicketPriority
    {
        public int Id { get; set; }

        [DisplayName("Ticket Priority")]
        public string Name { get; set; }

    }
}
