using Accelerate.Features.Onboarding.Models.Views;
using Accelerate.Foundations.Common.Models;
using System.Security.Claims;

namespace Accelerate.Features.Onboarding.Services
{
    public interface IOnboardingContentService
    {
        OnboardingBasePage CreateBasePage(UserProfile profile);

        Task<OnboardingBasePage> CreateSignUpPage(ClaimsPrincipal userClaim);
        Task<OnboardingBasePage> CreateConsumerSignUpPage(ClaimsPrincipal userClaim);
        Task<OnboardingBasePage> CreateBusinessSignUpPage(ClaimsPrincipal userClaim);
        Task<AuthenticateOtpPage> CreateAuthenticateOtpPage(ClaimsPrincipal userClaim, string provider);
        Task<AuthenticateOtpPage> CreateAuthenticateOtpPage(Guid userId, string provider);
        Task<OnboardingBasePage> CreateIdentityCheckPage(ClaimsPrincipal userClaim);
        Task<OnboardingBasePage> CreateIdentityCheckPage(Guid userId);
    }
}
