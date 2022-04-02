using Contracts;
using Entities;
using Entities.Constants;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotService.InlineKeyBoards
{
    public class CreationGroupInlineKeyBoard : IInlineKeyBoard
    {
        public string Name => InlineKeyBoardsConstants.CreationGroup;

        public async Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client, DataBaseContext db, ITelegramAuthorizationManager telegramAuthorizationManager)
        {
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


            await client.SendTextMessageAsync(callbackQuery.From.Id, "Enter group's name", replyMarkup: keyboard);

            var telegramUser = await db.TelegramUsers.FirstOrDefaultAsync(u => u.TelegramUserId == callbackQuery.From.Id);

            telegramUser.State = StateConstants.CreationGroup;

            await db.SaveChangesAsync();
        }
    }
}
