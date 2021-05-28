using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unbugit.Models;

namespace Unbugit.Services.Interfaces
{
    public interface IBTRoleService
    {
        public Task<bool> IsUserInRoleAsync(BTUser user, string roleName);
        public Task<IEnumerable<string>> ListUserRolesAsync(BTUser user);
        public Task<bool> AddUserToRoleAsync(BTUser user, string roleName);
        public Task<bool> RemoveUserFromRoleAsync(BTUser user, string roleName);
        public Task<bool> RemoveUserFromRolesAsync(BTUser user, IEnumerable<string> roles);
        public Task<List<BTUser>> UsersNotInRoleAsync(string roleName, int companyId);
        public Task<string> GetRoleNameByIdAsync(string roleId);

    }
}
