using Contracts;
using Entities;
using Entities.Constants;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotService.InlineKeyBoards
{
    public class ConfirmInlineKeyBoard : TopicKeyBoard, IInlineKeyBoard
    {
        public new string Name => InlineKeyBoardsConstants.Confirm;

        public override async Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client, DataBaseContext db, ITelegramAuthorizationManager telegramAuthorizationManager)
        {
            await Initialize(callbackQuery, client, db, telegramAuthorizationManager);
            // confirm section
            var telegramUserTopics = await db.TelegramUserTopics
                .FirstOrDefaultAsync(u => u.TelegramUserId.Equals(callbackQuery.From.Id) && u.TopicId.Equals(topicId));

            telegramUserTopics.IsConfirm = true;

            await db.SaveChangesAsync();
            await UpdateUsers();

            await client.AnswerCallbackQueryAsync(callbackQuery.Id, "I believe you're alive after that :3");

            var secondUser = users.FirstOrDefault(c => c.IsConfirm == false);
            if (secondUser != null)
            {
                var group = await db.Groups
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id.Equals(topic.GroupId));
                await client.SendTextMessageAsync(secondUser.UserId, "Hey, it is your turn in " + group.Name + " in " + topic.Name);
            }
            // 
            await FinishExecution(callbackQuery, client, db, telegramAuthorizationManager);
        }
    }
}
