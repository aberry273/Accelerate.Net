using Accelerate.Features.Onboarding.Models.Views;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Users.Models.Entities;
using System.Security.Claims;

namespace Accelerate.Features.Onboarding.Services
{
    public interface IOnboardingContentService
    {
        Task<OnboardingBasePage> CreateBasePage(UsersUser user);

        Task<OnboardingBasePage> CreateSignUpPage(ClaimsPrincipal userClaim);
        Task<OnboardingBasePage> CreateConsumerSignUpPage(ClaimsPrincipal userClaim);
        Task<OnboardingBasePage> CreateBusinessSignUpPage(ClaimsPrincipal userClaim);
        Task<AuthenticateOtpPage> CreateAuthenticateOtpPage(ClaimsPrincipal userClaim, string provider);
        Task<AuthenticateOtpPage> CreateAuthenticateOtpPage(Guid userId, string provider);
        Task<OnboardingBasePage> CreateFinalizeBusinessAccountPage(UsersUser user);
        Task<OnboardingBasePage> CreateFinalizeIndividualAccountPage(UsersUser user);
    }
}
