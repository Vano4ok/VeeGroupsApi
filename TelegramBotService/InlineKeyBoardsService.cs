using Contracts;
using System.Collections.Generic;
using TelegramBotService.InlineKeyBoards;

namespace TelegramBotService
{
    public class InlineKeyBoardsService : IInlineKeyBoardService
    {
        private readonly List<IInlineKeyBoard> inlineButtons;

        public InlineKeyBoardsService()
        {
            inlineButtons = new List<IInlineKeyBoard>
            {
               new StartMenuKeyBoard(),
               new ListOfGroupsKeyBoard(),
               new ListOfTopicsKeyBoard(),
               new CreationGroupInlineKeyBoard(),
               new CreationTopicInlineKeyBoard(),
               new TopicKeyBoard(),
               new EnterGroupKeyBoard(),
               new StandInLineInlineKeyBoard(),
               new ConfirmInlineKeyBoard(),
               new UnConfirmInlineKeyBoard(),
               new TopicSettingsInlineKeyBoard(),
               new DeleteTopicinlineKeyBoard(),
               new GroupSettingsInlineKeyBoard(),
               new DeleteGroupInlineKeyBoard(),
               new ShowInviteCodeInlineKeyBoard(),
               new RefreshInviteCodeInlineKeyBoard(),
               new SendMessageInlineKeyBoard()
            };
        }

        public List<IInlineKeyBoard> Get()
        {
            return inlineButtons;
        }
    }
}
