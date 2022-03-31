using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotService.InlineKeyboardMethods
{
    public class SpawnTopicKeyboard
    {
        
        public static void Generate(ref List<InlineKeyboardButton[]> keyboardList, List<UserInTopic> users, Topic topic, CallbackQuery callbackQuery, bool isAdmin){
            if (!users
                .Any(u => u.TopicId.Equals(topic.Id)
                && u.UserId.Equals(callbackQuery.From.Id)))
            {
                keyboardList.Add(
                    new InlineKeyboardButton[]
                    {
                        new InlineKeyboardButton()
                        {
                            Text ="\U000027A1 Stand in line",
                            CallbackData ="StandInLine_"+ topic.Id
                        }
                    });
            }
            else if (users != null)
            {
                var confirmUser = users.FirstOrDefault(x => x.IsConfirm == false);

                var key = new InlineKeyboardButton() {
                    Text = "Get out of here",
                    CallbackData = "UnConfirm_"+ topic.Id
                };

                if (confirmUser != null && confirmUser.UserId == callbackQuery.From.Id)
                {
                    keyboardList.Add(
                        new InlineKeyboardButton[]
                        {
                            new InlineKeyboardButton()
                            {
                                Text ="\U00002705 Confirm",
                                CallbackData ="Confirm_"+ topic.Id
                            },
                            key
                        });
                }

                else if( users.FirstOrDefault( x => x.UserId == callbackQuery.From.Id && !x.IsConfirm ) != null ){
                    keyboardList.Add( new InlineKeyboardButton[] { key });
                }
            }

            if (isAdmin)
                keyboardList.Add(
                    new InlineKeyboardButton[]
                    {
                        new InlineKeyboardButton()
                        {
                            Text = "\U00002699 Settings",
                            CallbackData = "TopicSettings_"+ topic.Id
                        }
                    });

            keyboardList.Add(
                new InlineKeyboardButton[]
                {
                    new InlineKeyboardButton()
                    {
                        Text = "\U000021A9 Back",
                        CallbackData = "ListOfTopics_"+topic.GroupId
                    }
                });

        }

    }
}