using System;
using System.Collections.Generic;
using System.Linq;

namespace TelegramBotService.InlineKeyboardMethods
{
    public class UserInTopic
    {
        public Guid TopicId;
        public long UserId;
        public string Name;
        public DateTime Date;
        public bool IsConfirm;
        public static string GenerateUserList(List<UserInTopic> users)
        {
            string messageText = "Users in line:\n";
            foreach (var user in users)
            {
                if (user.IsConfirm)
                    messageText += "\U00002705  ";
                else if (users.First().IsConfirm)
                    messageText += "        ";
                messageText += user.Name + "\n";
            }

            return messageText;
        }
    }
}