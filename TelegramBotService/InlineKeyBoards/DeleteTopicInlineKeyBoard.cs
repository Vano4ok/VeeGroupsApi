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
    public class DeleteTopicinlineKeyBoard : IInlineKeyBoard
    {
        public string Name => InlineKeyBoardsConstants.DeleteTopic;

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

            db.Topics.Remove(topic);

            await db.SaveChangesAsync();

            await client.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId, "This topic is deleted");
        }
    }
}
