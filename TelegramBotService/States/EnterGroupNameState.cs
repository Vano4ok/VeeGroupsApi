using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotService.States
{
    class EnterGroupNameState : IUserState
    {
        public string Name => "CreationGroup";

        public async Task<string> Execute(Message message, ITelegramBotClient client, DataBaseContext db, ITelegramAuthorizationManager telegramAuthorizationManager)
        {
            if (message.Text == "\U0000274C Cancel")
            {
                Message msg = await client.SendTextMessageAsync(message.From.Id, ".", replyMarkup: new ReplyKeyboardRemove());
                await client.DeleteMessageAsync(message.From.Id, msg.MessageId);
                return "StandartState";
            }

            if (message.Text.Length > 30)
            {
                await client.SendTextMessageAsync(message.From.Id, "Group's name is too long. Max size is 30. Try again");
                return Name;
            }

            if (await db.Groups.AnyAsync(u => u.Name.Equals(message.Text)))
            {
                await client.SendTextMessageAsync(message.From.Id, "This name is already taken.Try again");
                return Name;
            }

            var user = await db.TelegramUsers.
                FirstOrDefaultAsync(u => u.TelegramUserId.Equals(message.From.Id));

            user.TempGroupName = message.Text;

            await client.SendTextMessageAsync(message.From.Id, "Good job! Enter your name");

            return "EnterName";
        }
    }
}
