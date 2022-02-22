using Entities.Models;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IAuthenticationManager
    {
        Task<bool> ValidateUser(User user, string passwrod);

        Task<string> CreateToken();
    }
}
