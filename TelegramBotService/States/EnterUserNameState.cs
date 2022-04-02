using Contracts;
using Entities;
using Entities.Constants;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotService.States
{
    class EnterUserNameState : IUserState
    {
        public string Name => StateConstants.EnterName;

        public async Task<string> Execute(Message message, ITelegramBotClient client, DataBaseContext db, ITelegramAuthorizationManager telegramAuthorizationManager)
        {
            if (message.Text == "\U0000274C Cancel")
            {
                Message msg = await client.SendTextMessageAsync(message.From.Id, ".", replyMarkup: new ReplyKeyboardRemove());
                await client.DeleteMessageAsync(message.From.Id, msg.MessageId);
                return StateConstants.StandartState;
            }

            if (message.Text.Length > 40)
            {
                await client.SendTextMessageAsync(message.From.Id, "Your name is too long. Max size is 40. Try again");
                return Name;
            }

            var user = await db.TelegramUsers.FirstOrDefaultAsync(u => u.TelegramUserId == message.From.Id);
            var role = await db.TelegramRoles.FirstOrDefaultAsync(u => u.Name == "Administrator");

            Group newGroup = new Group()
            {
                Name = user.TempGroupName,
                InviteCode = Guid.NewGuid(),
                TelegramUserGroups = new List<TelegramUserGroup>
                {
                    new TelegramUserGroup
                    {
                         TelegramUserId = message.From.Id,
                         TelegramRoleId = role.Id,
                         UserName = message.Text
                    }
                }
            };

            await db.AddAsync(newGroup);

            await db.SaveChangesAsync();

            Message msg2 = await client.SendTextMessageAsync(message.From.Id, ".", replyMarkup: new ReplyKeyboardRemove());
            await client.DeleteMessageAsync(message.From.Id, msg2.MessageId);

            var group = await db.Groups
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Name == newGroup.Name);

            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(
                new InlineKeyboardButton[][] {
                    new InlineKeyboardButton[]
                    {
                        new InlineKeyboardButton()
                        {
                             Text = "\U0001F4AC Send message",
                             CallbackData = InlineKeyBoardsConstants.SendMessage+ "_" + group.Id
                        }

                    },
                    new InlineKeyboardButton[]
                    {
                        new InlineKeyboardButton()
                        {
                            Text = "\U0001FA84 Create new topic",
                            CallbackData = InlineKeyBoardsConstants.CreateTopic+ "_" + group.Id.ToString()
                        },
                    new InlineKeyboardButton()
                        {
                            Text = "\U00002699 Settings",
                            CallbackData = InlineKeyBoardsConstants.GroupSettings + "_"+ group.Id.ToString()
                        }
                    },
                    new InlineKeyboardButton[]
                    {
                        new InlineKeyboardButton()
                        {
                            Text = "\U000021A9 Back",
                            CallbackData = InlineKeyBoardsConstants.StartMenu
                        }
                    }
                });

            await client.SendTextMessageAsync(message.From.Id, "Wellcome to " + group.Name + "! \nIt is invite code of your group:\n" + group.InviteCode, replyMarkup: inlineKeyboard);

            return StateConstants.StandartState;
        }
    }
}
