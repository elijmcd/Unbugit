﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unbugit.Models;

namespace Unbugit.Services.Interfaces
{
    public interface IBTInviteService
    {
        public Task<Invite> GetInviteAsync(Guid token, string email);

        public Task<Invite> GetInviteAsync(int id);

        public Task<bool> AnyInviteAsync(Guid token, string email);

        public Task<bool> ValidateInviteCodeAsync(Guid? token);

        public Task<bool> AcceptInviteAsync(Guid? token, string userId);
    }
}
