using Accelerate.Foundations.Users.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Accelerate.Foundations.Users.Claims
{
    public class AdditionalUserClaimsPrincipalFactory :
       UserClaimsPrincipalFactory<UsersUser, UsersRole>
    {
        public AdditionalUserClaimsPrincipalFactory(
            UserManager<UsersUser> userManager,
            RoleManager<UsersRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        public async override Task<ClaimsPrincipal> CreateAsync(UsersUser user)
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
