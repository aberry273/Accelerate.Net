namespace Accelerate.Features.Onboarding.Models.Data
{
    public class OnboardingAuthenticationApiResendCodeRequest
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Provider { get; set; }
    }
}
