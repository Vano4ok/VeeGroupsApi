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
    public class TopicKeyBoard : IInlineKeyBoard
    {
        public string Name => "Topic";
        
        protected Topic topic;
        protected Guid topicId;
        protected bool isAdmin;
        protected DataBaseContext db;
        protected List<UserInTopic> users;

        /// <summury>Ment update users list and omit stupid errors</summury>
        protected async Task UpdateUsers(){
            users = await db.TelegramUserGroups.Where(x => x.GroupId.Equals(topic.GroupId))
                .Join(db.TelegramUserTopics, u => u.TelegramUserId, c => c.TelegramUserId, (u, c) => new UserInTopic
                {
                    TopicId = c.TopicId,
                    UserId = u.TelegramUserId,
                    Name = u.UserName,
                    Date = c.Date,
                    IsConfirm = c.IsConfirm,
                })
                .Where(x => x.TopicId.Equals(topic.Id))
                .OrderBy(u => u.Date)
                .ToListAsync();
        }

        protected async Task Initialize(CallbackQuery callbackQuery, ITelegramBotClient client, DataBaseContext db, ITelegramAuthorizationManager telegramAuthorizationManager){
            topicId = new Guid(callbackQuery.Data.Split('_')[1]);

            topic = await db.Topics
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == topicId);

            if (topic == null)
            {
                await client.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId, "This topic is deleted");
                throw new Exception($"Topic {topicId} was not found");
            }

            isAdmin = await telegramAuthorizationManager.IsAdmin(callbackQuery.From.Id, topic.GroupId, db);

            this.db = db;
            await UpdateUsers();
        }

        public virtual async Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client, DataBaseContext db, ITelegramAuthorizationManager telegramAuthorizationManager)
        {
            await Initialize(callbackQuery, client, db, telegramAuthorizationManager);
            await FinishExecution(callbackQuery, client, db, telegramAuthorizationManager);
        }

        protected async Task FinishExecution(CallbackQuery callbackQuery, ITelegramBotClient client, DataBaseContext db, ITelegramAuthorizationManager telegramAuthorizationManager){
            string messageText = $"Welcome to {topic.Name}\n";
            await UpdateUsers();
            messageText += UserInTopic.GenerateUserList(users);

            List<InlineKeyboardButton[]> keyboardList = new List<InlineKeyboardButton[]>();
            SpawnTopicKeyboard.Generate(ref keyboardList, users, topic, callbackQuery, isAdmin);
            InlineKeyboardMarkup inline = new InlineKeyboardMarkup(keyboardList);

            await client.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId, messageText, replyMarkup: inline);
        }
    }
}
