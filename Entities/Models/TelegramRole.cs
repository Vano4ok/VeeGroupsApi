using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public class TelegramRole
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<TelegramUserGroup> TelegramUserGroups { get; set; } = new List<TelegramUserGroup>();
    }
}
