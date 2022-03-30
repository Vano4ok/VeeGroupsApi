using Contracts;
using Entities;
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
    public class UnConfirmInlineKeyBoard : TopicKeyBoard, IInlineKeyBoard
    {
        public new string Name => "UnConfirm";

        public override async Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client, DataBaseContext db, ITelegramAuthorizationManager telegramAuthorizationManager)
        {

            await Initialize(callbackQuery, client, db, telegramAuthorizationManager);
// removal section
            var userko = await db.TelegramUserTopics
                .FirstOrDefaultAsync(u => u.TelegramUserId.Equals(callbackQuery.From.Id) && u.TopicId.Equals(topicId));

            db.TelegramUserTopics.Remove( userko );
            await db.SaveChangesAsync();
            await UpdateUsers();

            if(userko != null){
                await client.AnswerCallbackQueryAsync(callbackQuery.Id, "You are out of scope now c:");

                var secondUser = users.FirstOrDefault(c => c.IsConfirm == false);
                if (secondUser != null && users.IndexOf( users.FirstOrDefault( u => u.UserId == userko.TelegramUserId ) ) + 1 >= users.IndexOf(secondUser))
                {
                    var group = await db.Groups
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id.Equals(topic.GroupId));
                    await client.SendTextMessageAsync(secondUser.UserId, "Hey, it is your turn in " + group.Name + " in " + topic.Name);
                }
            }else
            await client.AnswerCallbackQueryAsync(callbackQuery.Id, "You've already been out of scope :0");

// 

            await FinishExecution(callbackQuery, client, db, telegramAuthorizationManager);
        }
    }
}
