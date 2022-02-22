using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public class Topic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid GroupId { get; set; }

        public Group Group { get; set; }

        public ICollection<TelegramUserTopic> TelegramUserTopics { get; set; } = new List<TelegramUserTopic>();
    }
}
