using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TelegramBotService
{
    public class TelegramAuthorizationManager : ITelegramAuthorizationManager
    {
        public async Task<bool> IsAdmin(long telegramUserId, Guid groupId, DataBaseContext db)
        {
            if (await db.TelegramUserGroups
                .Where(u => u.GroupId.Equals(groupId) && u.TelegramUserId.Equals(telegramUserId))
                .AnyAsync(u => u.TelegramRole.Name.Equals("Administrator")))
                return true;
            return false;
        }

        public async Task<bool> IsMember(long telegramUserId, Guid groupId, DataBaseContext db)
        {
            if (await db.TelegramUserGroups
                .Where(u => u.GroupId.Equals(groupId) && u.TelegramUserId.Equals(telegramUserId))
                .AnyAsync(u => u.TelegramRole.Name.Equals("Member")))
                return true;
            return false;
        }
    }
}
