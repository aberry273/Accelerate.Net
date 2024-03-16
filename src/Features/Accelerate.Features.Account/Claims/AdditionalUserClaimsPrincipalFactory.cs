using Accelerate.Features.Account.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Accelerate.Features.Account.Claims
{
    public class AdditionalUserClaimsPrincipalFactory :
       UserClaimsPrincipalFactory<AccountUser, AccountRole>
    {
        public AdditionalUserClaimsPrincipalFactory(
            UserManager<AccountUser> userManager,
            RoleManager<AccountRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        public async override Task<ClaimsPrincipal> CreateAsync(AccountUser user)
        {
            var principal = await base.CreateAsync(user);
            var identity = (ClaimsIdentity)principal.Identity;

            var claims = new List<Claim>();

            if (user.TwoFactorEnabled)
            {
                claims.Add(new Claim("amr", "mfa"));
            }
            else
            {
                claims.Add(new Claim("amr", "pwd"));
            }

            identity.AddClaims(claims);
            return principal;
        }
    }
}
