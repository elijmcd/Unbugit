﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unbugit.Models;

namespace Unbugit.Services.Interfaces
{
    public interface IBTNotificationService
    {
        public Task SaveNotificationAsync(Notification notification);

        public Task AdminsNotificationAsync(Notification notification, int companyId);

        public Task MembersNotificationAsync(Notification notification, List<BTUser> members);

        public Task EmailNotificationAsync(Notification notification, string emailSubject);

        public Task SMSNotificationAsync(string phone, Notification notification);

        public Task<List<Notification>> GetReceivedNotificationsAsync(string userId);

        public Task<List<Notification>> GetSentNotificationsAsync(string userId);
    }
}
