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

namespace TelegramBotService.States
{
    public class WriteGroupMessageState : IUserState
    {
        public string Name => StateConstants.EnterGroupMessage;

        public async Task<string> Execute(Message message, ITelegramBotClient client, DataBaseContext db, ITelegramAuthorizationManager telegramAuthorizationManager)
        {
            if (message.Text == "\U0000274C Cancel")
            {
                Message msg = await client.SendTextMessageAsync(message.From.Id, ".", replyMarkup: new ReplyKeyboardRemove());
                await client.DeleteMessageAsync(message.From.Id, msg.MessageId);
                return StateConstants.StandartState;
            }

            Message msg1 = await client.SendTextMessageAsync(message.From.Id, ".", replyMarkup: new ReplyKeyboardRemove());
            await client.DeleteMessageAsync(message.From.Id, msg1.MessageId);

            var telegramUser = await db.TelegramUsers.FirstOrDefaultAsync(u => u.TelegramUserId == message.From.Id);

            var group = await db.Groups.FirstOrDefaultAsync(u => u.Id.Equals(telegramUser.TempGroupId));

            var usersId = await db.TelegramUserGroups
                .Where(x => x.GroupId.Equals(group.Id))
                .ToListAsync();

            List<Task> tasks = new List<Task>();

            foreach (var userId in usersId)
            {
                if (userId.TelegramUserId != telegramUser.TelegramUserId)
                {
                    var task = client.SendTextMessageAsync(userId.TelegramUserId, message.Text);
                    tasks.Add(task);
                }
            }

            await Task.WhenAny(tasks);

            await client.SendTextMessageAsync(telegramUser.TelegramUserId, "The message was sent to all members of the group!");

            return StateConstants.StandartState;
        }
    }
}
