using System.Collections.Generic;

namespace Contracts
{
    public interface IStateService
    {
        List<IUserState> Get();
    }
}
