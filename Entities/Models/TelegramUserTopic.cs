using System;

namespace Entities.Models
{
    public class TelegramUserTopic
    {
        public long TelegramUserId { get; set; }

        public Guid TopicId { get; set; }

        public TelegramUser TelegramUser { get; set; }

        public Topic Topic { get; set; }

        public DateTime Date { get; set; }

        public bool IsConfirm { get; set; }
    }
}
