using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unbugit.Models;

namespace Unbugit.Services.Interfaces
{
    public interface IBTCompanyInfoService
    {
        Task<Company> GetCompanyInfoByIdAsync(int? companyId);

        Task<List<BTUser>> GetAllMembersAsync(int companyId);

        Task<List<Project>> GetAllProjectsAsync(int companyId);

        Task<List<Ticket>> GetAllTicketsAsync(int companyId);

        Task<List<Ticket>> GetUnassignedTicketsAsync(int companyId);

        Task<List<BTUser>> GetMembersInRoleAsync(string roleName, int CompanyId);
    }
}
