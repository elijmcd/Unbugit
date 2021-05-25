using Unbugit.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Unbugit.Data
{
    public class ApplicationDbContext : IdentityDbContext<BTUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Unbugit.Models.Company> Company { get; set; }
        public DbSet<Unbugit.Models.Invite> Invite { get; set; }
        public DbSet<Unbugit.Models.Notification> Notification { get; set; }
        public DbSet<Unbugit.Models.Project> Project { get; set; }
        public DbSet<Unbugit.Models.ProjectPriority> ProjectPriority { get; set; }
        public DbSet<Unbugit.Models.Ticket> Ticket { get; set; }
        public DbSet<Unbugit.Models.TicketAttachment> TicketAttachment { get; set; }
        public DbSet<Unbugit.Models.TicketComment> TicketComment { get; set; }
        public DbSet<Unbugit.Models.TicketHistory> TicketHistory { get; set; }
        public DbSet<Unbugit.Models.TicketPriority> TicketPriority { get; set; }
        public DbSet<Unbugit.Models.TicketStatus> TicketStatus { get; set; }
        public DbSet<Unbugit.Models.TicketType> TicketType { get; set; }
    }
}
