using Contracts;
using Entities;
using Entities.Constants;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotService.States
{
    public class EnterInviteCodeState : IUserState
    {
        public string Name => StateConstants.EnteringGroup;

        public async Task<string> Execute(Message message, ITelegramBotClient client, DataBaseContext db, ITelegramAuthorizationManager telegramAuthorizationManager)
        {
            if (message.Text == "\U0000274C Cancel")
            {
                Message msg = await client.SendTextMessageAsync(message.From.Id, ".", replyMarkup: new ReplyKeyboardRemove());
                await client.DeleteMessageAsync(message.From.Id, msg.MessageId);
                return StateConstants.StandartState;
            }

            var group = await db.Groups.
                FirstOrDefaultAsync(u => u.InviteCode.ToString().Equals(message.Text));

            if (group == null)
            {
                await client.SendTextMessageAsync(message.From.Id, "Invite code is wrong. Try again");
                return Name;
            }

            await client.SendTextMessageAsync(message.From.Id, "Good job! Enter your name");

            var user = await db.TelegramUsers.
                FirstOrDefaultAsync(u => u.TelegramUserId.Equals(message.From.Id));

            user.TempGroupId = group.InviteCode;

            return StateConstants.EnterUserNameForMember;
        }
    }
}
