using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unbugit.Models.ViewModels
{
    public class AssignPMViewModel
    {
        public Project Project { get; set; } = new();
        public SelectList Users { get; set; }
        public string PMId { get; set; }

    }
}
