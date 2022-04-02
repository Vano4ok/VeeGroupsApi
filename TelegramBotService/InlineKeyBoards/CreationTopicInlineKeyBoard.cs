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
    public class CreationTopicInlineKeyBoard : IInlineKeyBoard
    {
        public string Name => InlineKeyBoardsConstants.CreateTopic;

        public async Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client, DataBaseContext db, ITelegramAuthorizationManager telegramAuthorizationManager)
        {
            var groupId = new Guid(callbackQuery.Data.Split('_')[1]);

            var user = await db.TelegramUsers.FirstOrDefaultAsync(u => u.TelegramUserId == callbackQuery.From.Id);

            user.TempGroupId = groupId;

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


            await client.SendTextMessageAsync(callbackQuery.From.Id, "Enter topic's name", replyMarkup: keyboard);

            user.State = StateConstants.CreationTopic;

            await db.SaveChangesAsync();
        }
    }
}
