using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Unbugit.Models
{
    public class TicketComment
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Member Comment")]
        public string Comment { get; set; }

        [DisplayName("Date")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset Created { get; set; }

        [Required]
        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        [DisplayName("Team Member")]
        public string UserId { get; set; }

        //navigational properties
        public virtual Ticket Ticket { get; set; }
        public virtual BTUser User { get; set; }

    }
}
