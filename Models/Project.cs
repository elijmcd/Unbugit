﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Unbugit.Models
{
    public class Project
    {
        public int Id { get; set; }

        [DisplayName("Company")]
        public int CompanyId { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Project Name")]
        public string Name { get; set; }
        [DisplayName("Project Description")]
        public string Description { get; set; }

        [DisplayName("Start Date")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset StartDate { get; set; }
        [DisplayName("End Date")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset? EndDate { get; set; }


        [DisplayName("Priority")]
        public int? ProjectPriorityId { get; set; }

        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile ImageFormFile { get; set; }
        [DisplayName("File Name")]
        public string ImageFileName { get; set; }
        public byte[] ImageFileData { get; set; }
        [DisplayName("File Extension")]
        public string ImageContentType { get; set; }

        [DisplayName("Archived")]
        public bool Archived { get; set; }

        //navigational properties
        public virtual Company Company { get; set; }
        public virtual ProjectPriority ProjectPriority { get; set; }

        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();

    }
}
