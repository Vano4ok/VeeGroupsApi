using Contracts;
using Entities;
using Entities.Constants;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace VeeGroupsApi.Controllers
{
    [ApiController]
    [Route("api/message/update")]
    public class BotController : Controller
    {
        private readonly ITelegramBotClient telegramBotClient;
        private readonly ILoggerManager loggerManager;
        private readonly IStateService stateService;
        private readonly IInlineKeyBoardService inlineKeyBoardService;
        private readonly ITelegramAuthorizationManager telegramAuthorizationManager;
        private readonly DataBaseContext db;
        private readonly ILogger loginSpy;

        public BotController(DataBaseContext db, ILogger<BotController> loginSpy, ITelegramBotClient telegramBotClient/*, ILoggerManager loggerManager*/, IStateService stateService, IInlineKeyBoardService inlineKeyBoardService, ITelegramAuthorizationManager telegramAuthorizationManager)
        {
            this.loginSpy = loginSpy;
            this.telegramBotClient = telegramBotClient;
            // this.loggerManager = loggerManager;
            this.stateService = stateService;
            this.inlineKeyBoardService = inlineKeyBoardService;
            this.telegramAuthorizationManager = telegramAuthorizationManager;
            this.db = db;
        }

        [HttpGet]
        public IActionResult GetStarted()
        {
            return Ok("Started");
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            try
            {
                if (update == null) return Ok();

                loginSpy.LogDebug("Got a request!!");

                Message message = update.Message;

                CallbackQuery callBack = update.CallbackQuery;

                TelegramUser telegramUser = null;



                if (update.Message != null)
                    telegramUser = await db.TelegramUsers.FirstOrDefaultAsync(u => u.TelegramUserId == message.From.Id);
                else if (update.CallbackQuery != null)
                    telegramUser = await db.TelegramUsers.FirstOrDefaultAsync(u => u.TelegramUserId == callBack.From.Id);


                if (telegramUser == null)
                {
                    TelegramUser newUser = new TelegramUser()
                    {
                        TelegramUserId = message.From.Id,
                        State = StateConstants.StandartState
                    };

                    await db.TelegramUsers.AddAsync(newUser);
                    await db.SaveChangesAsync();

                    telegramUser = await db.TelegramUsers.FirstOrDefaultAsync(u => u.TelegramUserId == message.From.Id);
                }

                if (callBack != null)
                    if (telegramUser.State == StateConstants.StandartState)
                    {
                        foreach (var inlineKeyBoard in inlineKeyBoardService.Get())
                        {
                            if (inlineKeyBoard.Name == callBack.Data.Split('_')[0])
                            {
                                await inlineKeyBoard.Execute(callBack, telegramBotClient, db, telegramAuthorizationManager);

                                return Ok();
                            }
                        }
                    }

                if (message != null)
                    if (message.Text == "/start")
                    {
                        InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(

                            new InlineKeyboardButton[][] {
                            new InlineKeyboardButton[]
                            {
                                new InlineKeyboardButton()
                                {
                                    Text = "\U0001F4C1 Your groups",
                                    CallbackData = InlineKeyBoardsConstants.ListOfGroups
                                }
                            }, new InlineKeyboardButton[]
                            {
                                new InlineKeyboardButton()
                                {
                                    Text = "\U000027A1 Enter the group",
                                    CallbackData = InlineKeyBoardsConstants.EnterGroup
                                },
                                new InlineKeyboardButton()
                                {
                                    Text = "\U0001FA84 Create a new group",
                                    CallbackData = InlineKeyBoardsConstants.CreationGroup
                                }
                            }});

                        await telegramBotClient.SendTextMessageAsync(message.Chat.Id, "Hello! I am VeeGroups Bot", replyMarkup: inlineKeyboard);

                        return Ok();
                    }

                if (message != null)
                    foreach (var state in stateService.Get())
                    {
                        if (state.Name == telegramUser.State && state.Name != StateConstants.StandartState)
                        {
                            telegramUser.State = await state.Execute(message, telegramBotClient, db, telegramAuthorizationManager);
                            await db.SaveChangesAsync();

                            return Ok();
                        }
                    }

                return Ok();
            }
            catch (Exception e)
            {
                loginSpy.LogError($"BotController error: { e.Message }  { e.ToString()}");
                return Ok();
            }
        }
    }
}
