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
    public class ListOfTopicsKeyBoard : IInlineKeyBoard
    {
        public string Name => "ListOfTopics";

        public async Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client, DataBaseContext db, ITelegramAuthorizationManager telegramAuthorizationManager)
        {
            var groupId = new Guid(callbackQuery.Data.Split('_')[1]);

            var group = await db.Groups.FirstOrDefaultAsync(u => u.Id.Equals(groupId));

            if (group == null)
            {
                await client.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId, "This group is deleted");
                return;
            }

            var topics = await db.Topics
                .AsNoTracking()
                .Where(u => u.GroupId == groupId)
                .OrderBy(u => u.Name)
                .ToListAsync();

            List<InlineKeyboardButton[]> list = new List<InlineKeyboardButton[]>();
            foreach (var topic in topics)
            {
                InlineKeyboardButton button = new InlineKeyboardButton()
                {
                    Text = topic.Name,
                    CallbackData = "Topic_" + topic.Id,
                };
                InlineKeyboardButton[] row = new InlineKeyboardButton[1]
                {
                     button
                };
                list.Add(row);
            }

            var isAdmin = telegramAuthorizationManager.IsAdmin(callbackQuery.From.Id, groupId, db);

            if (await isAdmin)
            {

                InlineKeyboardButton[] keyboardButton = new InlineKeyboardButton[]
               {
                    new InlineKeyboardButton()
                    {
                        Text = "\U0001F4AC Send message",
                        CallbackData = "SendMessage_" + groupId
                    }
               };

                list.Add(keyboardButton);

                InlineKeyboardButton[] keyboardButtons = new InlineKeyboardButton[]
                {
                    new InlineKeyboardButton()
                    {
                        Text = "\U0001FA84 Create new topic",
                        CallbackData = "CreateTopic_" + groupId
                    },
                    new InlineKeyboardButton()
                    {
                        Text = "\U00002699 Settings",
                        CallbackData = "GroupSettings_"+ groupId
                    }
                };

                list.Add(keyboardButtons);
            }

            InlineKeyboardButton backButton = new InlineKeyboardButton()
            {
                Text = "\U000021A9 Back",
                CallbackData = "ListOfGroups"
            };
            InlineKeyboardButton[] lastRow = new InlineKeyboardButton[1]
            {
                     backButton
            };
            list.Add(lastRow);


            InlineKeyboardMarkup inline = new InlineKeyboardMarkup(list);

            await client.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId, "Welcome to " + group.Name, replyMarkup: inline);

        }
    }
}
