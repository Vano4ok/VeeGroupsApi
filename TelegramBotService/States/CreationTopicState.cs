using Contracts;
using Entities;
using Entities.Constants;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotService.States
{
    public class CreationTopicState : IUserState
    {
        public string Name => StateConstants.CreationTopic;

        public async Task<string> Execute(Message message, ITelegramBotClient client, DataBaseContext db, ITelegramAuthorizationManager telegramAuthorizationManager)
        {
            if (message.Text == "\U0000274C Cancel")
            {
                Message msg = await client.SendTextMessageAsync(message.From.Id, ".", replyMarkup: new ReplyKeyboardRemove());
                await client.DeleteMessageAsync(message.From.Id, msg.MessageId);
                return StateConstants.StandartState;
            }

            if (message.Text.Length > 30)
            {
                await client.SendTextMessageAsync(message.From.Id, "Topic's name is too long. Max size is 30. Try again");
                return Name;
            }

            var user = await db.TelegramUsers
                .AsNoTracking().
                FirstOrDefaultAsync(u => u.TelegramUserId.Equals(message.From.Id));

            if (await db.Topics.Where(c => c.GroupId.Equals(user.TempGroupId)).AnyAsync(u => u.Name.Equals(message.Text)))
            {
                await client.SendTextMessageAsync(message.From.Id, "This name is already taken.Try again");
                return Name;
            }

            Topic newTopic = new Topic()
            {
                Name = message.Text,
                GroupId = user.TempGroupId
            };

            await db.AddAsync(newTopic);

            await db.SaveChangesAsync();

            var topic = await db.Topics
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Name.Equals(newTopic.Name));

            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(
               new InlineKeyboardButton[][] {
                    new InlineKeyboardButton[]
                    {
                        new InlineKeyboardButton()
                        {
                            Text ="\U000027A1 Stand in",
                            CallbackData =InlineKeyBoardsConstants.StandInLine+ "_"+ topic.Id
                        }
                    },
                    new InlineKeyboardButton[]
                    {
                        new InlineKeyboardButton()
                        {
                            Text = "\U000021A9 Back",
                            CallbackData = InlineKeyBoardsConstants.ListOfTopics+ "_"+user.TempGroupId
                        },
                        new InlineKeyboardButton()
                        {
                            Text = "\U00002699 Settings",
                            CallbackData = InlineKeyBoardsConstants.TopicSettings+ "_"+ topic.Id
                        }
                    }
               });

            Message msg2 = await client.SendTextMessageAsync(message.From.Id, ".", replyMarkup: new ReplyKeyboardRemove());
            await client.DeleteMessageAsync(message.From.Id, msg2.MessageId);

            await client.SendTextMessageAsync(message.From.Id, "Welcome to " + topic.Name, replyMarkup: inlineKeyboard);

            return StateConstants.StandartState;
        }
    }
}
