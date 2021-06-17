using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    public class BTProjectService : IBTProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IBTRoleService _roleService;
        private readonly ILogger<BTProjectService> _logger;

        public BTProjectService(ApplicationDbContext context,
                                UserManager<BTUser> userManager,
                                RoleManager<IdentityRole> roleManager,
                                IBTRoleService roleService,
                                ILogger<BTProjectService> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _roleService = roleService;
            _logger = logger;
        }

        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            BTUser currentPM = await GetProjectManagerAsync(projectId);

            if (currentPM != null)
            {
                try
                {
                    await RemoveProjectManagerAsync(projectId);
                }
                catch (Exception e)
                {
                    _logger.LogInformation($"Error removing current Project Manager - Error: {e.Message}");
                    return false;
                }
            }

            try
            {
                await AddUserToProjectAsync(userId, projectId);

                return true;
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Error adding user as Project Manager - Error: {e.Message}");
                return false;
            }

            throw new NotImplementedException();
        }//

        public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            try
            {
                BTUser user = await _userManager.FindByIdAsync(userId);
                Project project = await _context.Project.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);

                project.Members.Add(user);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }//

        public async Task<List<Project>> GetAllProjectsByCompany(int companyId)
        {
            List<Project> projects = new();

            projects = await _context.Project
                                     .Include(p => p.Members)
                                     .Include(p => p.Tickets)
                                        .ThenInclude(t => t.OwnerUser)
                                     .Include(p => p.Tickets)
                                        .ThenInclude(t => t.DeveloperUser)
                                     .Include(p => p.Tickets)
                                        .ThenInclude(t => t.Comments)
                                     .Include(p => p.Tickets)
                                        .ThenInclude(t => t.Attachments)
                                     .Include(p => p.Tickets)
                                        .ThenInclude(t => t.TicketHistory)
                                     .Include(p => p.Tickets)
                                        .ThenInclude(t => t.TicketPriority)
                                     .Include(p => p.Tickets)
                                        .ThenInclude(t => t.TicketStatus)
                                     .Include(p => p.Tickets)
                                        .ThenInclude(t => t.TicketType)
                                     .Where(p => p.CompanyId == companyId)
                                     .ToListAsync();

            return projects;
        } //

        public async Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
        {
            List<Project> projects = new();

            int priorityId = await LookupProjectPriorityId(priorityName);
            projects = await _context.Project.Where(p => p.CompanyId == companyId && p.ProjectPriorityId == priorityId).ToListAsync();
            return projects;
        }//

        public async Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
        {
            List<Project> projects = new();

            projects = await GetAllProjectsByCompany(companyId);

            return projects.Where(p => p.Archived == true).ToList();
        }//

        public async Task<List<BTUser>> GetMembersWithoutPMAsync(int projectId)
        {
            List<BTUser> developers = await GetProjectMembersByRoleAsync(projectId, "Developer");
            List<BTUser> submitters = await GetProjectMembersByRoleAsync(projectId, "Submitter");
            List<BTUser> admins = await GetProjectMembersByRoleAsync(projectId, "Admin");

            List<BTUser> members = developers.Concat(submitters).Concat(admins).ToList();

            return members;
        }//

        public async Task<BTUser> GetProjectManagerAsync(int projectId)
        {
            Project project = await _context.Project.Include(p => p.Members)
                .FirstOrDefaultAsync(u => u.Id == projectId);

            foreach (BTUser member in project?.Members)
            {
                if (await _roleService.IsUserInRoleAsync(member, "ProjectManager"))
                {
                    return member;
                }
            }
            return null;
        }//

        public async Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            Project project = await _context.Project
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            List<BTUser> members = new();

            foreach (var user in project.Members)
            {
                if (await _roleService.IsUserInRoleAsync(user, role))
                {
                    members.Add(user);
                }
            }
            return members;
        }//

        public async Task<bool> IsUserOnProject(string userId, int projectId)
        {
            Project project = await _context.Project.FirstOrDefaultAsync(p => p.Id == projectId);

            bool result = project.Members.Any(u => u.Id == userId);

            return result;
        }//

        public async Task<List<Project>> ListUserProjectsAsync(string userId)
        {
            try
            {
                List<Project> userProjects = (await _context.Users
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Company)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Members)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.OwnerUser)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.DeveloperUser)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.TicketPriority)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.TicketStatus)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.TicketType)
                    .FirstOrDefaultAsync(u => u.Id == userId)).Projects.ToList();

                return userProjects;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"*** ERROR *** - Error Getting list of user projects. -> {e.Message}");
                throw;
            }
        }//

        public async Task RemoveProjectManagerAsync(int projectId)
        {
            Project project = await _context.Project
                                            .Include(p => p.Members)
                                            .FirstOrDefaultAsync(p => p.Id == projectId);

            try
            {
                foreach (var member in project.Members)
                {
                    if (await _roleService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                    {
                        await RemoveUserFromProjectAsync(member.Id, project.Id);
                    }
                }
            }
            catch
            {
                throw;
            }
        }//

        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            try
            {
                BTUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                Project project = await _context.Project.FirstOrDefaultAsync(p => p.Id == projectId);

                if (await IsUserOnProject(userId, projectId))
                {
                    try
                    {
                        project.Members.Remove(user);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"*** ERROR *** - Error removing user from project -> {e.Message}");
            }

        }//

        public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            try
            {
                List<BTUser> members = await GetProjectMembersByRoleAsync(projectId, role);
                Project project = await _context.Project.FirstOrDefaultAsync(p => p.Id == projectId);

                foreach (BTUser btUser in members)
                {
                    try
                    {
                        project.Members.Remove(btUser);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"*** ERROR *** - Error Removing Users From Project. -> {e.Message}");
            }
        }//

        public async Task<List<BTUser>> UsersNotOnProjectAsync(int projectId, int companyId)
        {
            List<BTUser> users = await _context.Users.Where(u => u.Projects.All(p => p.Id != projectId) && u.CompanyId == companyId).ToListAsync();

            return users;
            throw new NotImplementedException();
        }//

        public async Task<string> LookupProjectPriorityName(int priorityId)
        {
            return (await _context.ProjectPriority.Include(p=>p.Name).FirstOrDefaultAsync(p => p.Id == priorityId)).ToString();
        }
        public async Task<int> LookupProjectPriorityId(string priorityName)
        {
            return (await _context.ProjectPriority.FirstOrDefaultAsync(p => p.Name == priorityName)).Id;
        }//

        //public async Task<List<BTUser>> SubmittersOnProjectAsync(int projectId)
        //{
        //    Project project = await _context.Project
        //        .Include(p => p.Members)
        //        .FirstOrDefaultAsync(u => u.Id == projectId);

        //    List<BTUser> submitters = new();

        //    foreach (var user in project.Members)
        //    {
        //        if (await _roleService.IsUserInRoleAsync(user, "Submitter"))
        //        {
        //            submitters.Add(user);
        //        }
        //    }
        //    return submitters;
        //}//

        public async Task<List<BTUser>> DevelopersOnProjectAsync(int projectId)
        {
            Project project = await _context.Project
                .Include(p => p.Members)
                .FirstOrDefaultAsync(u => u.Id == projectId);

            List<BTUser> developers = new();

            foreach (var user in project.Members)
            {
                if (await _roleService.IsUserInRoleAsync(user, "Developer"))
                {
                    developers.Add(user);
                }
            }
            return developers;
        }
    }
}
