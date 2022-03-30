using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotService.InlineKeyboardMethods;

namespace TelegramBotService.InlineKeyBoards
{
    public class StandInLineInlineKeyBoard : TopicKeyBoard, IInlineKeyBoard
    {
        public new string Name => "StandInLine";

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
