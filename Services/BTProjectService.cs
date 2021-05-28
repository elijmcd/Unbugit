using Microsoft.AspNetCore.Identity;
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
    public class BTProjectService : IBTProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public BTProjectService(ApplicationDbContext context,
                                UserManager<BTUser> userManager,
                                RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            
        }

        public Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BTUser>> DevelopersOnProjectAsync(int projectId)
        {
            List<BTUser> developers = await _context.Project.Include(p => p.Members)
                                                            .Include(p => p.Tickets)
                                                                .ThenInclude(t => t.DeveloperUser)
                                                            .Where(p => p.Id == projectId).ToListAsync();

            return developers;
        }

        public async Task<List<Project>> GetAllProjectsByCompany(int companyId)
        {
            List<Project> projects = new();

            projects = await _context.Project
                                     .Include(p => p.CompanyId == companyId).ToListAsync();

            return projects;
        }

        public async Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
        {
            List<Company> company = new();
            List<Project> projects = new();

            company = await _context.Company.Include(c => c.Projects)
                                            .Where(c => c.CompanyId == companyId).ToListAsync();
            projects = company.FindAll(p => p.Projects).Where(p => p.ProjectPriority.Name == priorityName).ToList();
           
            return projects;
        }

        public Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<List<BTUser>> GetMembersWithoutPMAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<BTUser> GetProjectManagerAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsUserOnProject(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Project>> ListUserProjectAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveProjectManagerAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUsersFromProjectByRoleAsync(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<List<BTUser>> SubmittersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<List<BTUser>> UsersNotOnProjectAsync(int projectId, int companyId)
        {
            throw new NotImplementedException();
        }
    }
}
