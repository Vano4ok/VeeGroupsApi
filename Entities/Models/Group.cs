using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public class Group
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid InviteCode { get; set; }

        public ICollection<Topic> Topics { get; set; } = new List<Topic>();

        public ICollection<TelegramUserGroup> TelegramUserGroups { get; set; } = new List<TelegramUserGroup>();
    }
}
