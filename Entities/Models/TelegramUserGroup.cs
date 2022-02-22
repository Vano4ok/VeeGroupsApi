using System;

namespace Entities.Models
{
    public class TelegramUserGroup
    {
        public long TelegramUserId { get; set; }

        public Guid TelegramRoleId { get; set; }

        public Guid GroupId { get; set; }

        public TelegramUser TelegramUser { get; set; }

        public TelegramRole TelegramRole { get; set; }

        public Group Group { get; set; }

        public string UserName { get; set; }
    }
}
