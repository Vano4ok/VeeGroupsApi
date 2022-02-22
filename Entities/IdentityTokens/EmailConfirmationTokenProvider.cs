using Entities.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Entities.IdentityTokens
{
    public class EmailConfirmationTokenProvider : TotpSecurityStampBasedTokenProvider<User>
    {
        public const string ProviderKey = "EmailConfirmation";

        public override Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<User> manager, User user)
        {
            return Task.FromResult(false);
        }
    }
}
