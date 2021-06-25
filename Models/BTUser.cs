using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Unbugit.Models
{
    public class BTUser : IdentityUser
    {
        [Required]
        [DisplayName("First Name")]
        [StringLength(33, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        [StringLength(33, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string LastName { get; set; }

        [DisplayName("FullName")]
        public string FullName { get { return $"{FirstName} {LastName}";} }

        [NotMapped]
        [DataType(DataType.Upload)]
        //[MaxFileSize(1024 * 1024)]
        //[AllowedExtensions(new string[] {".jpg", ".png"})]
        public IFormFile AvatarFormFile { get; set; }
        [DisplayName("File Name")]
        public string AvatarFileName { get; set; }
        public byte[] AvatarFileData { get; set; }
        [DisplayName("File Extension")]
        public string AvatarContentType { get; set; }

        public int? CompanyId { get; set; }

        //navigational properties
        public virtual Company Company { get; set; }
        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();
    }
}
