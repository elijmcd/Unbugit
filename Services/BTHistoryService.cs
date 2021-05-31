using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unbugit.Models;
using Unbugit.Services.Interfaces;

namespace Unbugit.Services
{
    public class BTHistoryService : IBTHistoryService
    {
        public Task AddHistory(Ticket oldTicket, Ticket newTicket, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<TicketHistory>> GetCompanyTicketHistories(int companyId)
        {
            Company company = await _context.Company.Include(c=>c.Tickets)
            throw new NotImplementedException();
        }

        public Task<List<TicketHistory>> GetProjectTicketHistories(int projectId)
        {
            throw new NotImplementedException();
        }
    }
}
