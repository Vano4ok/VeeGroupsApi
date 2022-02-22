using Entities;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Contracts
{
    public interface IInlineKeyBoard
    {
        public abstract string Name { get; }

        public abstract Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client, DataBaseContext db, ITelegramAuthorizationManager telegramAuthorizationManager);
    }
}
