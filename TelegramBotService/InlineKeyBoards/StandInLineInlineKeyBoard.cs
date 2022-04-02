using Contracts;
using Entities;
using Entities.Constants;
using Entities.Models;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotService.InlineKeyBoards
{
    public class StandInLineInlineKeyBoard : TopicKeyBoard, IInlineKeyBoard
    {
        public new string Name => InlineKeyBoardsConstants.ShowInviteCode;

        public override async Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client, DataBaseContext db, ITelegramAuthorizationManager telegramAuthorizationManager)
        {
            await Initialize(callbackQuery, client, db, telegramAuthorizationManager);
            // stand in line section
            var newTelegramUserTopic = new TelegramUserTopic
            {
                TopicId = topicId,
                TelegramUserId = callbackQuery.From.Id,
                Date = DateTime.Now,
                IsConfirm = false
            };

            await db.TelegramUserTopics.AddAsync(newTelegramUserTopic);
            await db.SaveChangesAsync();
            await UpdateUsers();

            await client.AnswerCallbackQueryAsync(callbackQuery.Id, "Look out snipers are watching 0_0");
            // 

            await FinishExecution(callbackQuery, client, db, telegramAuthorizationManager);
        }
    }
}
