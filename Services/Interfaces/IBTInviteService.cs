using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unbugit.Models;

namespace Unbugit.Services
{
    public interface IBTInviteService
    {
        public Task<Invite> GetInviteAsync(Guid token, string email);

        public Task<Invite> GetInviteAsync(int id);

        public Task<bool> ValidateInviteCodeAsync(Guid? token);

        public Task<bool> AcceptInviteAsync(Guid? token, string userId);
    }
}
