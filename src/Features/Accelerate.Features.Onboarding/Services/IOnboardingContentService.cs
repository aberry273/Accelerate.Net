using Accelerate.Features.Onboarding.Models.Views;
using Accelerate.Foundations.Common.Models;
using System.Security.Claims;

namespace Accelerate.Features.Onboarding.Services
{
    public interface IOnboardingContentService
    {
        OnboardingBasePage CreateBasePage(UserProfile profile);
        Task<OnboardingBasePage> CreateSignUpPage(ClaimsPrincipal userClaim);
        Task<OnboardingBasePage> CreateIdentityCheckPage(ClaimsPrincipal userClaim);
    }
}
