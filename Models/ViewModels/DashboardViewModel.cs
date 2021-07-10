using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unbugit.Models.ViewModels
{
    public class DashboardViewModel
    {
        public List<Project> Projects { get; set; }
        public List<Ticket> Tickets { get; set; }
        public List<BTUser> Users { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual Project Project { get; set; }
    }
}
