using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotService.InlineKeyBoards
{
    public class GroupSettingsInlineKeyBoard : IInlineKeyBoard
    {
        public string Name => "GroupSettings";

        public async Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client, DataBaseContext db, ITelegramAuthorizationManager telegramAuthorizationManager)
        {
            var groupId = new Guid(callbackQuery.Data.Split('_')[1]);

            var group = await db.Groups
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == groupId);

            if (group == null)
            {
                await client.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId, "This group is deleted");
                return;
            }

            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(

                new InlineKeyboardButton[][] {

                new InlineKeyboardButton[]
                {
                    new InlineKeyboardButton()
                    {
                        Text = "\U00002328 Show invite code",
                        CallbackData = "ShowInviteCode_"+group.Id
                    },
                    new InlineKeyboardButton()
                    {
                        Text = "\U0001F504 Refresh invite code",
                        CallbackData = "RefreshInviteCode_"+group.Id
                    }
                },
                new InlineKeyboardButton[]
                {
                    new InlineKeyboardButton()
                    {
                        Text = "\U0001F9E8 Delete group",
                        CallbackData = "DeleteGroup_"+group.Id
                    },
                    new InlineKeyboardButton()
                    {
                        Text = "\U000021A9 Back",
                        CallbackData = "ListOfTopics_"+group.Id
                    }
                }});

            await client.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId, "Settings of group : " + group.Name, replyMarkup: inlineKeyboard);
        }
    }
}
