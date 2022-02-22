using Entities;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Contracts
{
    public interface IUserState
    {
        public abstract string Name { get; }

        public abstract Task<string> Execute(Message message, ITelegramBotClient client, DataBaseContext db, ITelegramAuthorizationManager telegramAuthorizationManager);
    }
}
