using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Unbugit.Models.ViewModels
{
    public class InviteViewModel : Invite
    {
        [Display(Name = "Company")]
        public new Company Company { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Project")]
        public new int ProjectId { get; set; }

        public SelectList ProjectsList { get; set; }

        public string Message { get; set; }
    }
}
