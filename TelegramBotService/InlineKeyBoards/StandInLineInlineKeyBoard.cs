using Contracts;
using Entities;
using Entities.Models;
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
    public class StandInLineInlineKeyBoard : IInlineKeyBoard
    {
        public string Name => "StandInLine";

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

            var newTelegramUserTopic = new TelegramUserTopic
            {
                TopicId = topicId,
                TelegramUserId = callbackQuery.From.Id,
                Date = DateTime.Now,
                IsConfirm = false
            };

            await db.TelegramUserTopics.AddAsync(newTelegramUserTopic);

            await db.SaveChangesAsync();

            await client.AnswerCallbackQueryAsync(callbackQuery.Id, "Now you are in line!", true);

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

            if (users != null)
            {
                var confirmUser = users.FirstOrDefault(x => x.IsConfirm == false);

                if (confirmUser == null)
                {
                    if (callbackQuery.From.Id == users.ElementAt(0).UserId)
                        list.Add(
                            new InlineKeyboardButton[]
                                {
                                    new InlineKeyboardButton()
                                    {
                                        Text ="\U00002705 Confirm",
                                        CallbackData ="Confirm_"+ topic.Id
                                    }
                            });
                }
                else if (confirmUser.UserId == callbackQuery.From.Id)
                {
                    list.Add(
                           new InlineKeyboardButton[]
                               {
                                    new InlineKeyboardButton()
                                    {
                                        Text ="\U00002705 Confirm",
                                        CallbackData ="Confirm_"+ topic.Id
                                    }
                           });
                }
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

            await client.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId, messageText, replyMarkup: inline);
        }
    }
}
