using Contracts;
using Entities;
using Entities.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotService.InlineKeyBoards
{
    public class SendMessageInlineKeyBoard : IInlineKeyBoard
    {
        public string Name => InlineKeyBoardsConstants.SendMessage;

        public async Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client, DataBaseContext db, ITelegramAuthorizationManager telegramAuthorizationManager)
        {
            var groupId = new Guid(callbackQuery.Data.Split('_')[1]);

            var group = await db.Groups.FirstOrDefaultAsync(u => u.Id.Equals(groupId));

            if (group == null)
            {
                await client.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId, "This group is deleted");
                return;
            }

            ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup
            {
                Keyboard = new[]
                   {
                        new[]
                        {
                            new KeyboardButton("\U0000274C Cancel")
                        }
                    },
                ResizeKeyboard = true
            };


            await client.SendTextMessageAsync(callbackQuery.From.Id, "Write your message", replyMarkup: keyboard);

            var telegramUser = await db.TelegramUsers.FirstOrDefaultAsync(u => u.TelegramUserId == callbackQuery.From.Id);

            telegramUser.State = StateConstants.EnterGroupMessage;

            telegramUser.TempGroupId = group.Id;

            await db.SaveChangesAsync();
        }
    }
}
