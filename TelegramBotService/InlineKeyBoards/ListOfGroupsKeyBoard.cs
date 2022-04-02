using Contracts;
using Entities;
using Entities.Constants;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotService.InlineKeyBoards
{
    public class ListOfGroupsKeyBoard : IInlineKeyBoard
    {
        public string Name => InlineKeyBoardsConstants.ListOfGroups;

        public async Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client, DataBaseContext db, ITelegramAuthorizationManager telegramAuthorizationManager)
        {
            var groups = await db.Groups
            .AsNoTracking()
            .Where(u => u.TelegramUserGroups.Any(us => us.TelegramUserId.Equals(callbackQuery.From.Id)))
            .OrderBy(u => u.Name)
            .ToListAsync();

            List<InlineKeyboardButton[]> list = new List<InlineKeyboardButton[]>();
            foreach (var group in groups)
            {
                InlineKeyboardButton button = new InlineKeyboardButton()
                {
                    Text = group.Name,
                    CallbackData = InlineKeyBoardsConstants.ListOfTopics + "_" + group.Id,
                };
                InlineKeyboardButton[] row = new InlineKeyboardButton[1]
                {
                    button
                };
                list.Add(row);
            }


            InlineKeyboardButton backButton = new InlineKeyboardButton()
            {
                Text = "\U000021A9 Back",
                CallbackData = InlineKeyBoardsConstants.StartMenu
            };
            InlineKeyboardButton[] lastRow = new InlineKeyboardButton[1]
            {
                    backButton
            };
            list.Add(lastRow);

            var inline = new InlineKeyboardMarkup(list);

            await client.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId, "Groups:", replyMarkup: inline);
        }
    }
}
