using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Unbugit.Data;
using Unbugit.Models;
using Unbugit.Models.Enums;
using Unbugit.Services.Interfaces;

namespace Unbugit.Services
{
    public class BTTicketService : IBTTicketService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTRoleService _roleService;
        private readonly IBTProjectService _projectService;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly UserManager<BTUser> _userManager;

        public BTTicketService(ApplicationDbContext context, IBTProjectService projectService, IBTCompanyInfoService companyInfoService, IBTRoleService roleService, UserManager<BTUser> userManager)
        {
            _context = context;
            _projectService = projectService;
            _companyInfoService = companyInfoService;
            _roleService = roleService;
            _userManager = userManager;
        }


        public async Task AssignTicketAsync(int ticketId, string userId)
        {
            Ticket ticket = await _context.Ticket.FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket != null)
            {
                try
                {
                    ticket.TicketStatusId = (await LookupTicketStatusIdAsync("Development")).Value;
                    ticket.DeveloperUserId = userId;
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public async Task CloseTicketAsync(int ticketId, string userId)
        {
            Ticket ticket = await _context.Ticket.FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket != null)
            {
                try
                {
                    ticket.TicketStatusId = (await LookupTicketStatusIdAsync("Resolved")).Value;
                    ticket.DeveloperUserId = null;
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        //--
        public async Task<BTUser> GetTicketDeveloperAsync(int ticketId)
        {
            BTUser developer = new();
            Ticket ticket = await _context.Ticket.Include(t => t.DeveloperUser).FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket?.DeveloperUserId != null)
            {
                developer = ticket.DeveloperUser;
            }
            return developer;
        }// --
        public async Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId)
        {
            try
            {
                List<Ticket> tickets = await _context.Project.Include(p => p.Company)
                                                              .Where(p => p.CompanyId == companyId)
                                                              .SelectMany(p => p.Tickets)
                                                                  .Include(t => t.Attachments)
                                                                  .Include(t => t.Comments)
                                                                  .Include(t => t.TicketHistory)
                                                                  .Include(t => t.DeveloperUser)
                                                                  .Include(t => t.OwnerUser)
                                                                  .Include(t => t.TicketPriority)
                                                                  .Include(t => t.TicketStatus)
                                                                  .Include(t => t.TicketType)
                                                                  .ToListAsync();
                return tickets;
            }
            catch
            {
                throw;
            }
        }// --
        public async Task<List<Ticket>> GetArchivedTicketsByCompanyAsync(int companyId)
        {
            try
            {
                List<Ticket> tickets = await _context.Project.Include(p => p.Company)
                                              .Where(p => p.CompanyId == companyId)
                                              .SelectMany(p => p.Tickets)
                                                  .Include(t => t.Attachments)
                                                  .Include(t => t.Comments)
                                                  .Include(t => t.TicketHistory)
                                                  .Include(t => t.DeveloperUser)
                                                  .Include(t => t.OwnerUser)
                                                  .Include(t => t.TicketPriority)
                                                  .Include(t => t.TicketStatus)
                                                  .Include(t => t.TicketType)
                                                  .Include(t => t.Project)
                                                  .Where(t => t.Archived == true)
                                                  .ToListAsync();
                return tickets;
            }
            catch
            {
                throw;
            }
            //List<Ticket> tickets = await _context.Ticket.Where(t => t.Project.CompanyId == companyId && t.Archived).ToListAsync();
            //return tickets;
        }// --
        public async Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName)
        {
            int priorityId = (await LookupTicketPriorityIdAsync(priorityName)).Value;
            List<Ticket> tickets = new();

            try
            {
                tickets = await _context.Project.Where(p => p.CompanyId == companyId)
                    .SelectMany(p => p.Tickets)
                        .Include(t => t.Attachments)
                        .Include(t => t.Comments)
                        .Include(t => t.DeveloperUser)
                        .Include(t => t.OwnerUser)
                        .Include(t => t.TicketPriority)
                        .Include(t => t.TicketStatus)
                        .Include(t => t.TicketType)
                        .Include(t => t.Project)
                    .Where(t => t.TicketPriorityId == priorityId).ToListAsync();
            }
            catch
            {
                throw;
            }
            return tickets;


            //try
            //{
            //    List<Project> projects = await _context.Project.Include(p => p.Tickets).Where(p => p.CompanyId == companyId).ToListAsync();

            //    List<Ticket> tickets = projects.SelectMany(t => t.Tickets).Where(t => t.TicketPriorityId == priorityId).ToList();

            //    return tickets.Where(t => t.TicketPriorityId == priorityId).ToList();
            //}
            //catch
            //{
            //    throw;
            //}
        }// -- Is this...
        public async Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName)
        {
            int statusId = (await LookupTicketStatusIdAsync(statusName)).Value;
            List<Ticket> tickets = new();

            try
            {
                tickets = await _context.Project.Where(p => p.CompanyId == companyId)
                    .SelectMany(p => p.Tickets)
                        .Include(t => t.Attachments)
                        .Include(t => t.Comments)
                        .Include(t => t.DeveloperUser)
                        .Include(t => t.OwnerUser)
                        .Include(t => t.TicketPriority)
                        .Include(t => t.TicketStatus)
                        .Include(t => t.TicketType)
                        .Include(t => t.Project)
                    .Where(t => t.TicketStatusId == statusId).ToListAsync();
            }
            catch
            {
                throw;
            }
            return tickets;

            //try
            //{
            //    List<Project> projects = await _context.Project.Include(p => p.Tickets).Where(p => p.CompanyId == companyId).ToListAsync();

            //    List<Ticket> tickets = projects.SelectMany(t => t.Tickets).Where(t => t.TicketStatusId == statusId).ToList();

            //    return tickets.Where(t => t.TicketStatusId == statusId).ToList();
            //}
            //catch
            //{
            //    throw;
            //}

            //List<Ticket> tickets = await _context.Ticket.Where(t => t.TicketStatus.Name == statusName && t.Project.CompanyId == companyId).ToListAsync();
        }// -- ..better than this?
        public async Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName)
        {
            int typeId = (await LookupTicketTypeIdAsync(typeName)).Value;
            List<Ticket> tickets = new();

            try
            {
                tickets = await _context.Project.Where(p => p.CompanyId == companyId)
                    .SelectMany(p => p.Tickets)
                        .Include(t => t.Attachments)
                        .Include(t => t.Comments)
                        .Include(t => t.DeveloperUser)
                        .Include(t => t.OwnerUser)
                        .Include(t => t.TicketPriority)
                        .Include(t => t.TicketStatus)
                        .Include(t => t.TicketType)
                        .Include(t => t.Project)
                    .Where(t => t.TicketTypeId == typeId).ToListAsync();
            }
            catch
            {
                throw;
            }
            return tickets;

            //try
            //{
            //    List<Project> projects = await _context.Project.Include(p => p.Tickets).Where(p => p.CompanyId == companyId).ToListAsync();

            //    List<Ticket> tickets = projects.SelectMany(t => t.Tickets).Where(t => t.TicketTypeId == typeId).ToList();

            //    return tickets.Where(t => t.TicketTypeId == typeId).ToList();
            //}
            //catch
            //{
            //    throw;
            //}
            //List<Ticket> tickets = await _context.Ticket.Where(t => t.TicketType.Name == typeName && t.Project.CompanyId == companyId).ToListAsync();
            //return tickets;
        }// --

        public async Task<List<Ticket>> GetAllPMTicketsAsync(string userId)
        {
            try
            {
                List<Project> projects = await _projectService.ListUserProjectsAsync(userId);
                List<Ticket> tickets = projects.SelectMany(t => t.Tickets).ToList();

                return tickets;
            }
            catch
            {
                throw;
            }

            //BTUser user = await _context.Users.Include(u => u.Projects)
            //                                    .ThenInclude(t => t.Tickets)
            //                                  .FirstOrDefaultAsync(u => u.Id == userId);

            //List<Ticket> tickets = new();

            //try
            //{
            //    if (await _roleService.IsUserInRoleAsync(user, "ProjectManager"))
            //    {
            //        tickets = user.Projects.SelectMany(p => p.Tickets).ToList();
            //    }
            //    return null;
            //}
            //catch (Exception e)
            //{
            //    Debug.WriteLine($"***ERROR *** - User has no tickets in Project Manager role. -> {e.Message}");
            //}
            //return tickets;
        }// ****
        public async Task<List<Ticket>> GetAllTicketsByRoleAsync(string role, string userId)
        {
            List<Ticket> tickets = new();

            if (string.Compare(role, Roles.Developer.ToString()) == 0)
            {
                //try catch
                tickets = await _context.Ticket.Include(t => t.Attachments)
                    .Include(t => t.Comments)
                    .Include(t => t.DeveloperUser)
                    .Include(t => t.OwnerUser)
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.Project)
                        .ThenInclude(p => p.Members)
                    .Include(t => t.Project)
                        .ThenInclude(p => p.ProjectPriority)
                    .Where(t => t.DeveloperUserId == userId).ToListAsync();
            }
            else if (string.Compare(role, Roles.Submitter.ToString()) == 0)
            {
                //try catch
                tickets = await _context.Ticket.Include(t => t.Attachments)
                    .Include(t => t.Comments)
                    .Include(t => t.DeveloperUser)
                    .Include(t => t.OwnerUser)
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.Project)
                        .ThenInclude(p => p.Members)
                    .Include(t => t.Project)
                        .ThenInclude(p => p.ProjectPriority)
                    .Where(t => t.DeveloperUserId == userId).ToListAsync();
            }
            else if (string.Compare(role, Roles.ProjectManager.ToString()) == 0)
            {
                //try catch
                tickets = await GetAllPMTicketsAsync(userId);
            }
            /// add admin role?
            /// 
            return tickets;

            //BTUser user = await _context.Users
            //                            .Include(u => u.Projects)
            //                              .ThenInclude(p => p.Tickets)
            //                            .FirstOrDefaultAsync(u => u.Id == userId);
            //List<Ticket> tickets = new();
            //try
            //{
            //    if ((await _roleService.ListUserRolesAsync(user)).Contains(role))
            //    {
            //        tickets = user.Projects.SelectMany(p => p.Tickets).ToList();
            //    }
            //    return null;
            //}
            //catch (Exception e)
            //{
            //    Debug.WriteLine($"***ERROR *** - User has no tickets in Project Manager role. -> {e.Message}");
            //}
            //return tickets;
        }// ****
        public async Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId)
        {
            List<Ticket> tickets = new();

            tickets = (await GetAllTicketsByRoleAsync(role, userId)).Where(t => t.ProjectId == projectId).ToList();

            return tickets;
        }
        public async Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId)
        {

            List<Ticket> tickets = new();

            tickets = (await GetAllTicketsByPriorityAsync(companyId, priorityName)).Where(t => t.ProjectId == projectId).ToList();

            return tickets;
        }
        public async Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId)
        {

            List<Ticket> tickets = new();

            tickets = (await GetAllTicketsByStatusAsync(companyId, statusName)).Where(t => t.ProjectId == projectId).ToList();

            return tickets;
        }
        public async Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId)
        {

            List<Ticket> tickets = new();

            tickets = (await GetAllTicketsByTypeAsync(companyId, typeName)).Where(t => t.ProjectId == projectId).ToList();

            return tickets;
            //Project project = await _context.Project
            //    .Include(p => p.Members)
            //    .Include(p => p.Tickets)
            //    .FirstOrDefaultAsync(p => p.Id == projectId);
            //BTUser user = await _context.Users.Include(u => u.Projects)
            //    .ThenInclude(p => p.Tickets)
            //    .FirstOrDefaultAsync();
            //List<Ticket> tickets = new();
            //try
            //{
            //    if ((await _roleService.ListUserRolesAsync(user)).Contains(role))
            //    {
            //        tickets = _context.Ticket.Include(t => t.ProjectId == projectId && (project.Members.Contains(user))).ToList();
            //    }
            //    return null;
            //}
            //catch (Exception e)
            //{
            //    Debug.WriteLine($"***ERROR *** - User has no tickets in selected role. -> {e.Message}");
            //}
            //return tickets;
        }// *** INCOMPLETE??

        public async Task<int?> LookupTicketPriorityIdAsync(string priorityName)
        {
            try
            {
                TicketPriority priority = await _context.TicketPriority.FirstOrDefaultAsync(p => p.Name == priorityName);
                return priority?.Id;
            }
            catch
            {
                throw;
            }
        }// --
        public async Task<int?> LookupTicketStatusIdAsync(string statusName)
        {
            try
            {
                TicketStatus status = await _context.TicketStatus.FirstOrDefaultAsync(s => s.Name == statusName);
                return status?.Id;
            }
            catch
            {
                throw;
            }
        }// --
        public async Task<int?> LookupTicketTypeIdAsync(string typeName)
        {
            try
            {
                return (await _context.TicketType.FirstOrDefaultAsync(t => t.Name == typeName)).Id;
            }
            catch
            {
                throw;
            }


        }// --

    }
}
