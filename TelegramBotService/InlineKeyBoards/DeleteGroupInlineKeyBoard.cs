using Contracts;
using Entities;
using Entities.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotService.InlineKeyBoards
{
    public class DeleteGroupInlineKeyBoard : IInlineKeyBoard
    {
        public string Name => InlineKeyBoardsConstants.DeleteGroup;

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

            db.Groups.Remove(group);

            await db.SaveChangesAsync();

            await client.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId, "This group is deleted");
        }
    }
}
