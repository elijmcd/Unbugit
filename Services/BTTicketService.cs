using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Unbugit.Data;
using Unbugit.Models;
using Unbugit.Services.Interfaces;

namespace Unbugit.Services
{
    public class BTTicketService : IBTTicketService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTRoleService _roleService;
        private readonly IBTProjectService _projectService;
        private readonly IBTCompanyInfoService _companyInfoService;

        public BTTicketService(ApplicationDbContext context, IBTProjectService projectService, IBTCompanyInfoService companyInfoService, IBTRoleService roleService)
        {
            _context = context;
            _projectService = projectService;
            _companyInfoService = companyInfoService;
            _roleService = roleService;
        }


        public async Task AssignTicketAsync(int ticketId, string userId)
        {
            BTUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            Ticket ticket = await _context.Ticket.FirstOrDefaultAsync(t => t.Id == ticketId);

            try
            {
                if (user != null)
                {
                    try
                    {
                        if (ticket.DeveloperUser != null)
                        {
                            ticket.DeveloperUser = null;
                        }
                        ticket.DeveloperUser = user;
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"***ERROR *** - No ticket was assign. -> {e.Message}");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"***ERROR *** - No ticket was assign. -> {e.Message}");
            }
        }//--

        public async Task<List<Ticket>> GetAllPMTicketsAsync(string userId)
        {
            BTUser user = await _context.Users.Include(u => u.Projects)
                .ThenInclude(t => t.Tickets)
                .FirstOrDefaultAsync(u => u.Id == userId);

            List<Ticket> tickets = await _context.Project.Include(p => p.Members).Where(Ticket.await _roleService.IsUserInRoleAsync(user, "ProjectManager"));

            return tickets;
        }// ****

        public async Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId)
        {
            Company company = await _context.Company
                .Include(c => c.Projects)
                    .ThenInclude(p => p.Tickets)
                .FirstOrDefaultAsync(c => c.CompanyId == companyId);

            List<Ticket> tickets = company.Projects.SelectMany(t => t.Tickets).ToList();

            return tickets;
        }// --

        public async Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName)
        {

            int priorityId = (await LookupTicketPriorityIdAsync(priorityName)).Value;

            List<Project> projects = await _context.Project.Include(p => p.Tickets).Where(p => p.CompanyId == companyId).ToListAsync();

            List<Ticket> tickets = projects.SelectMany(t => t.Tickets).ToList();

            return tickets.Where(t => t.TicketPriorityId == priorityId).ToList();
        }// -- Is this...

        public Task<List<Ticket>> GetAllTicketsByRoleAsync(string role, string userId)
        {
            throw new NotImplementedException();
        }// ***

        public async Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName)
        {
            List<Ticket> tickets = await _context.Ticket.Where(t => t.TicketStatus.Name == statusName && t.Project.CompanyId == companyId).ToListAsync();

            return tickets;
        }// -- ..better than this?

        public async Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName)
        {
            List<Ticket> tickets = await _context.Ticket.Where(t => t.TicketType.Name == typeName && t.Project.CompanyId == companyId).ToListAsync();

            return tickets;
        }// --

        public async Task<List<Ticket>> GetArchivedTicketsByCompanyAsync(int companyId)
        {
            List<Ticket> tickets = await _context.Ticket.Where(t => t.Project.CompanyId == companyId && t.Archived).ToListAsync();
            return tickets;
        }// --

        public async Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId)
        {
            List<Ticket> tickets = new();
            Project project = await _context.Project
                .Include(p => p.Members)
                .Include(p => p.Tickets)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            throw new NotImplementedException();
        }// ***

        public async Task<BTUser> GetTicketDeveloperAsync(int ticketId)
        {
            BTUser developer = new();
            Ticket ticket = await _context.Ticket.Include(t => t.DeveloperUser).FirstOrDefaultAsync(t => t.Id == ticketId);

            return developer = ticket.DeveloperUser;
        }// --

        public async Task<int?> LookupTicketPriorityIdAsync(string priorityName)
        {
            return (await _context.TicketPriority.FirstOrDefaultAsync(t => t.Name == priorityName)).Id;
        }// --

        public async Task<int?> LookupTicketStatusIdAsync(string statusName)
        {
            return (await _context.TicketStatus.FirstOrDefaultAsync(t => t.Name == statusName)).Id;
        }// --

        public async Task<int?> LookupTicketTypeIdAsync(string typeName)
        {
            return (await _context.TicketType.FirstOrDefaultAsync(t => t.Name == typeName)).Id;
        }// --
    }
}
