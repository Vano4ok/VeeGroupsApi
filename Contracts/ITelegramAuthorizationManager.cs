using Entities;
using System;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ITelegramAuthorizationManager
    {
        Task<bool> IsAdmin(long telegramUserId, Guid groupId, DataBaseContext db);

        Task<bool> IsMember(long telegramUserId, Guid groupId, DataBaseContext db);
    }
}
