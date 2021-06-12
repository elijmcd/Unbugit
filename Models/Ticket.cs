using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Unbugit.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Title")]
        public string Title { get; set; }

        [Required]
        [StringLength(300)]
        [DisplayName("Description")]
        public string Description { get; set; }


        [DataType(DataType.DateTime)]
        [DisplayName("Date Created")]
        public DateTimeOffset Created { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayName("Date Updated")]
        public DateTimeOffset? Updated { get; set; }


        [DataType(DataType.DateTime)]
        [DisplayName("Date Archived")]
        public DateTimeOffset? ArchivedDate { get; set; }

        [DisplayName("Archived")]
        public bool Archived { get; set; }


        [DisplayName("Project")]
        public int ProjectId { get; set; }
        [DisplayName("Ticket Type")]
        public int TicketTypeId { get; set; }
        [DisplayName("Ticket Priority")]
        public int TicketPriorityId { get; set; }
        [DisplayName("Ticket Status")]
        public int TicketStatusId { get; set; }

        [DisplayName("Ticket Owner")]
        public string OwnerUserId { get; set; }
        [DisplayName("Ticket Developer")]
        public string DeveloperUserId { get; set; }

        //navigational properties
        public virtual Project Project { get; set; }
        public virtual TicketType TicketType { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual BTUser OwnerUser { get; set; }
        public virtual BTUser DeveloperUser { get; set; }

        public virtual ICollection<TicketComment> Comments { get; set; } = new HashSet<TicketComment>();
        public virtual ICollection<TicketAttachment> Attachments { get; set; } = new HashSet<TicketAttachment>();
        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
        public virtual ICollection<TicketHistory> TicketHistory { get; set; } = new HashSet<TicketHistory>(); 

    }
}
