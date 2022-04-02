﻿using Contracts;
using Entities;
using Entities.Constants;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotService.InlineKeyBoards
{
    public class StartMenuKeyBoard : IInlineKeyBoard
    {
        public string Name => InlineKeyBoardsConstants.StartMenu;

        public async Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client, DataBaseContext db, ITelegramAuthorizationManager telegramAuthorizationManager)
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

            await client.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId, "Hello! I am VeeGroups Bot", replyMarkup: inlineKeyboard);
        }
    }
}
