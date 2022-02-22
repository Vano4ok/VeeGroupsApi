using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotService.InlineKeyBoards
{
    public class ConfirmInlineKeyBoard : IInlineKeyBoard
    {
        public string Name => "Confirm";

        public async Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client, DataBaseContext db, ITelegramAuthorizationManager telegramAuthorizationManager)
        {
            var topicId = new Guid(callbackQuery.Data.Split('_')[1]);

            var topic = await db.Topics
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == topicId);

            if (topic == null)
            {
                await client.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId, "This topic is deleted");
                return;
            }

            var telegramUserTopics = await db.TelegramUserTopics
                .FirstOrDefaultAsync(u => u.TelegramUserId.Equals(callbackQuery.From.Id) && u.TopicId.Equals(topicId));

            telegramUserTopics.IsConfirm = true;

            await db.SaveChangesAsync();

            await client.AnswerCallbackQueryAsync(callbackQuery.Id, "Good job!", true);

            var isAdmin = await telegramAuthorizationManager.IsAdmin(callbackQuery.From.Id, topic.GroupId, db);

            List<InlineKeyboardButton[]> list = new List<InlineKeyboardButton[]>();

            var users = await db.TelegramUserGroups.Where(x => x.GroupId.Equals(topic.GroupId))
                .Join(db.TelegramUserTopics, u => u.TelegramUserId, c => c.TelegramUserId,
              (u, c) => new
              {
                  TopicId = c.TopicId,
                  UserId = u.TelegramUserId,
                  Name = u.UserName,
                  Date = c.Date,
                  IsConfirm = c.IsConfirm,
              }).Where(x => x.TopicId.Equals(topic.Id))
              .OrderBy(u => u.Date)
              .ToListAsync();

            string messageText = "Welcome to " + topic.Name + "\nUsers in line:\n";

            foreach (var user in users)
            {
                if (user.IsConfirm)
                    messageText += "\U00002705  ";
                messageText += user.Name + "\n";
            }

            if (isAdmin)
                list.Add(
                    new InlineKeyboardButton[]
                    {
                        new InlineKeyboardButton()
                        {
                            Text = "\U00002699 Settings",
                            CallbackData = "TopicSettings_"+ topic.Id
                        }
                    });

            list.Add(
                new InlineKeyboardButton[]
                {
                    new InlineKeyboardButton()
                    {
                        Text = "\U000021A9 Back",
                        CallbackData = "ListOfTopics_"+topic.GroupId
                    }
                });

            InlineKeyboardMarkup inline = new InlineKeyboardMarkup(list);

            var secondUser = users.FirstOrDefault(c => c.IsConfirm == false);

            if (secondUser != null)
            {
                var group = await db.Groups
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id.Equals(topic.GroupId));
                await client.SendTextMessageAsync(secondUser.UserId, "Hey, it is your turn in " + group.Name + " in " + topic.Name);
            }


            await client.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId, messageText, replyMarkup: inline);
        }
    }
}
