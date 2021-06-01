using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unbugit.Data;
using Unbugit.Models;
using Unbugit.Services.Interfaces;

namespace Unbugit.Services
{
    public class BTHistoryService : IBTHistoryService
    {
        private readonly ApplicationDbContext _context;

        public BTHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task AddHistory(Ticket oldTicket, Ticket newTicket, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<TicketHistory>> GetCompanyTicketHistories(int companyId)
        {
            Company company = await _context.Company.Include(c => c.Projects)
                                                        .ThenInclude(p => p.Tickets)
                                                            .ThenInclude(t => t.TicketHistory).FirstOrDefaultAsync(c=>c.Id == companyId);

            List<Ticket> tickets = company.Projects.SelectMany(t => t.Tickets).ToList();

            List<TicketHistory> histories = tickets.SelectMany(h => h.TicketHistory).ToList();

            return histories;
        }

        public async Task<List<TicketHistory>> GetProjectTicketHistories(int projectId)
        {
            Project project = await _context.Project.Include(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketHistory).FirstOrDefaultAsync(p => p.Id == projectId);

            List<TicketHistory> histories = project.Tickets.SelectMany(h => h.TicketHistory).ToList();

            return histories;
        }
    }
}
