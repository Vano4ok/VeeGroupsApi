using Contracts;
using System.Collections.Generic;
using TelegramBotService.States;

namespace TelegramBotService
{
    public class StateService : IStateService
    {
        private readonly List<IUserState> states;

        public StateService()
        {
            states = new List<IUserState>
            {
               new EnterGroupNameState(),
               new EnterUserNameState(),
               new CreationTopicState(),
               new EnterInviteCodeState(),
               new EnterUserNameForMemberState(),
               new WriteGroupMessageState()
            };
        }
        public List<IUserState> Get()
        {
            return states;
        }
    }
}
