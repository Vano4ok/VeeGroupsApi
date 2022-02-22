using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotService.States
{
    public class EnterUserNameForMemberState : IUserState
    {
        public string Name => "EnterUserNameForMember";

        public async Task<string> Execute(Message message, ITelegramBotClient client, DataBaseContext db, ITelegramAuthorizationManager telegramAuthorizationManager)
        {
            if (message.Text == "\U0000274C Cancel")
            {
                Message msg = await client.SendTextMessageAsync(message.From.Id, ".", replyMarkup: new ReplyKeyboardRemove());
                await client.DeleteMessageAsync(message.From.Id, msg.MessageId);
                return "StandartState";
            }

            if (message.Text.Length > 40)
            {
                await client.SendTextMessageAsync(message.From.Id, "Your name is too long. Max size is 40. Try again");
                return Name;
            }

            var user = await db.TelegramUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.TelegramUserId == message.From.Id);

            var role = await db.TelegramRoles
                .FirstOrDefaultAsync(u => u.Name == "Member");

            var group = await db.Groups.
                FirstOrDefaultAsync(u => u.InviteCode == user.TempGroupId);


            group.TelegramUserGroups.Add(
                new TelegramUserGroup
                {
                    TelegramUserId = message.From.Id,
                    TelegramRoleId = role.Id,
                    UserName = message.Text
                });

            Message msg1 = await client.SendTextMessageAsync(message.From.Id, ".", replyMarkup: new ReplyKeyboardRemove());
            await client.DeleteMessageAsync(message.From.Id, msg1.MessageId);

            var topics = await db.Topics
                .AsNoTracking()
                .Where(u => u.GroupId == group.Id)
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

            InlineKeyboardButton backButton = new InlineKeyboardButton()
            {
                Text = "Back",
                CallbackData = "ListOfGroups"
            };
            InlineKeyboardButton[] lastRow = new InlineKeyboardButton[1]
            {
                     backButton
            };
            list.Add(lastRow);

            InlineKeyboardMarkup inline = new InlineKeyboardMarkup(list);

            await client.SendTextMessageAsync(message.From.Id, "Welcome to " + group.Name, replyMarkup: inline);

            return "StandartState";
        }
    }
}
