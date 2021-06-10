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

        public async Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId)
        {
            //create a new ticket
            if(oldTicket == null && newTicket != null)
            {
                TicketHistory history = new()
                {
                    TicketId = newTicket.Id,
                    Property = "",
                    OldValue = "",
                    NewValue = "",
                    Created = DateTimeOffset.Now,
                    UserId = userId,
                    Description = "New Ticket Created"
                };
                await _context.TicketHistory.AddAsync(history);
                await _context.SaveChangesAsync();
            }
            else
            {   
                //check title
                if(oldTicket.Title != newTicket.Title)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Title",
                        OldValue = oldTicket.Title,
                        NewValue = newTicket.Title,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket title: {newTicket.Title}"
                    };
                    await _context.TicketHistory.AddAsync(history);
                }
                //check description
                if(oldTicket.Description != newTicket.Description)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Description",
                        OldValue = oldTicket.Description,
                        NewValue = newTicket.Description,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket description: {newTicket.Description}"
                    };
                    await _context.TicketHistory.AddAsync(history);
                }
                //check type
                if(oldTicket.TicketTypeId != newTicket.TicketTypeId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Ticket Type",
                        OldValue = oldTicket.TicketType.Name,
                        NewValue = newTicket.TicketType.Name,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket type: {newTicket.TicketType.Name}"
                    };
                    await _context.TicketHistory.AddAsync(history);
                }
                //check status
                if(oldTicket.TicketStatusId != newTicket.TicketStatusId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Title",
                        OldValue = oldTicket.TicketStatus.Name,
                        NewValue = newTicket.TicketStatus.Name,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket status: {newTicket.TicketStatus.Name}"
                    };
                    await _context.TicketHistory.AddAsync(history);
                }
                //check priority
                if(oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Title",
                        OldValue = oldTicket.TicketPriority.Name,
                        NewValue = newTicket.TicketPriority.Name,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket priority: {newTicket.TicketPriority.Name}"
                    };
                    await _context.TicketHistory.AddAsync(history);
                }
                if(oldTicket.DeveloperUserId is null || newTicket.DeveloperUserId is null)
                {
                    Console.WriteLine("Uh Oh!");
                }
                //Check developer
                if(oldTicket.DeveloperUserId != newTicket.DeveloperUserId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Title",
                        OldValue = oldTicket.DeveloperUser?.FullName ?? "Not Assigned",
                        NewValue = newTicket.DeveloperUser?.FullName,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket developer: {newTicket.DeveloperUser.FullName}"
                    };
                    await _context.TicketHistory.AddAsync(history);
                }

                //save the TicketHistory set to the database
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<TicketHistory>> GetCompanyTicketHistoriesAsync(int companyId)
        {
            Company company = await _context.Company.Include(c => c.Projects)
                                                        .ThenInclude(p => p.Tickets)
                                                            .ThenInclude(t => t.TicketHistory).FirstOrDefaultAsync(c=>c.Id == companyId);

            List<Ticket> tickets = company.Projects.SelectMany(t => t.Tickets).ToList();

            List<TicketHistory> histories = tickets.SelectMany(h => h.TicketHistory).ToList();

            return histories;
        }//

        public async Task<List<TicketHistory>> GetProjectTicketHistoriesAsync(int projectId)
        {
            Project project = await _context.Project.Include(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketHistory).FirstOrDefaultAsync(p => p.Id == projectId);

            List<TicketHistory> histories = project.Tickets.SelectMany(t => t.TicketHistory).ToList();

            return histories;
        }//
    }
}
