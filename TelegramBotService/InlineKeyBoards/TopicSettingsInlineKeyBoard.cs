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
    public class TopicSettingsInlineKeyBoard : IInlineKeyBoard
    {
        public string Name => InlineKeyBoardsConstants.TopicSettings;

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

            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(

                new InlineKeyboardButton[][] {
                new InlineKeyboardButton[]
                {
                    new InlineKeyboardButton()
                    {
                        Text = "\U0001F9E8 Delete topic",
                        CallbackData = InlineKeyBoardsConstants.DeleteTopic+ "_"+topic.Id
                    },
                    new InlineKeyboardButton()
                    {
                        Text = "\U000021A9 Back",
                        CallbackData = InlineKeyBoardsConstants.Topic+ "_"+topic.Id
                    }
                }});

            await client.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId, "Settings of topic : " + topic.Name, replyMarkup: inlineKeyboard);

        }
    }
}
