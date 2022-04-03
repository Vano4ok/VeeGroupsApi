using Entities.Constants;
using Entities.Models;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotService.InlineKeyboardMethods
{
    public class SpawnTopicKeyboard
    {

        public static void Generate(ref List<InlineKeyboardButton[]> keyboardList, List<UserInTopic> users, Topic topic, CallbackQuery callbackQuery, bool isAdmin)
        {
            if (!users
                .Any(u => u.TopicId.Equals(topic.Id)
                && u.UserId.Equals(callbackQuery.From.Id)))
            {
                keyboardList.Add(
                    new InlineKeyboardButton[]
                    {
                        new InlineKeyboardButton()
                        {
                            Text ="\U000027A1 Stand in",
                            CallbackData =InlineKeyBoardsConstants.StandInLine+ "_"+ topic.Id
                        }
                    });
            }
            else if (users != null)
            {
                var confirmUser = users.FirstOrDefault(x => x.IsConfirm == false);

                var key = new InlineKeyboardButton()
                {
                    Text = "\U0001F9F3 Get out",
                    CallbackData = InlineKeyBoardsConstants.UnConfirm + "_" + topic.Id
                };

                if (confirmUser != null && confirmUser.UserId == callbackQuery.From.Id)
                {
                    keyboardList.Add(
                        new InlineKeyboardButton[]
                        {
                            new InlineKeyboardButton()
                            {
                                Text ="\U00002705 Confirm",
                                CallbackData =InlineKeyBoardsConstants.Confirm+ "_"+ topic.Id
                            },
                            key
                        });
                }

                else if (users.FirstOrDefault(x => x.UserId == callbackQuery.From.Id && !x.IsConfirm) != null)
                {
                    keyboardList.Add(new InlineKeyboardButton[] { key });
                }
            }

            keyboardList.Add(
                new InlineKeyboardButton[]
                {
                    new InlineKeyboardButton()
                    {
                        Text = "\U000021A9 Back",
                        CallbackData = InlineKeyBoardsConstants.ListOfTopics+ "_"+topic.GroupId
                    },
                    ( isAdmin ?
                        new InlineKeyboardButton()
                        {
                            Text = "\U00002699 Settings",
                            CallbackData = InlineKeyBoardsConstants.TopicSettings+ "_"+ topic.Id
                        }
                            :
                    null)
                });

        }

    }
}