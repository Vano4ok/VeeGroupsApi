using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public class TelegramUser
    {
        public long TelegramUserId { get; set; }

        public string State { get; set; }

        public string TempGroupName { get; set; }

        public Guid TempGroupId { get; set; }

        public ICollection<TelegramUserGroup> TelegramUserGroups { get; set; } = new List<TelegramUserGroup>();

        public ICollection<TelegramUserTopic> TelegramUserTopics { get; set; } = new List<TelegramUserTopic>();
    }
}
